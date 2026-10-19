<script lang="ts">
  import { onMount } from 'svelte';
  import { getEvents, toggleVote } from '../../lib/api/events';
  import { currentUser } from '../../lib/stores/userStore';
  import type { Event as CampusEvent } from '../../lib/types/event';

  let events = $state<CampusEvent[]>([]);
  let searchQuery = $state('');
  let selectedCategory = $state('All');
  let isLoading = $state(true);
  let errorMessage = $state('');

  const categories = ['All', 'Social', 'Technology', 'Academic', 'Sport', 'Other'];

  async function loadEvents() {
    isLoading = true;
    errorMessage = '';
    try {
      events = await getEvents();
    } catch (err: any) {
      errorMessage = 'Failed to load campus events. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  async function handleVote(eventId: number) {
    if (!$currentUser) return;
    try {
      const res = await toggleVote(eventId, $currentUser.id);
      events = events.map((ev) =>
        ev.id === eventId
          ? { ...ev, upvotes: res.upvotes, userVoted: res.userVoted }
          : ev
      );
    } catch (err) {
      console.error('Failed to toggle vote:', err);
    }
  }

  // Filter events dynamically based on search query and category selection
  let filteredEvents = $derived(
    events.filter((ev) => {
      const q = searchQuery.toLowerCase();
      const matchesSearch =
        ev.title.toLowerCase().includes(q) ||
        ev.description.toLowerCase().includes(q) ||
        (ev.location && ev.location.toLowerCase().includes(q)) ||
        (ev.clubName && ev.clubName.toLowerCase().includes(q));

      const matchesCategory =
        selectedCategory === 'All' ||
        String(ev.type || '').toLowerCase() === selectedCategory.toLowerCase();

      return matchesSearch && matchesCategory;
    })
  );

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'TBA';
    const d = new Date(dateStr);
    return isNaN(d.getTime())
      ? 'TBA'
      : d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  onMount(loadEvents);
</script>

<div class="space-y-6">
  <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Campus Events</h1>
      <p class="text-gray-600 text-sm">Discover upcoming student activities and society gatherings.</p>
    </div>
  </div>

  <!-- Filter and Search Bar -->
  <div class="flex flex-col sm:flex-row gap-4 items-stretch sm:items-center">
    <div class="flex-grow">
      <input
        type="text"
        bind:value={searchQuery}
        placeholder="Search events by title, location, or host club..."
        class="w-full border border-gray-300 rounded-md px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
      />
    </div>

    <div class="w-full sm:w-48">
      <select
        bind:value={selectedCategory}
        class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
      >
        {#each categories as category}
          <option value={category}>{category === 'All' ? 'All Categories' : category}</option>
        {/each}
      </select>
    </div>
  </div>

  {#if errorMessage}
    <div class="p-4 bg-red-100 border border-red-400 text-red-700 rounded-md text-sm">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div class="text-center py-12 text-gray-500 text-sm">Loading upcoming events...</div>
  {:else if filteredEvents.length === 0}
    <div class="bg-white border border-gray-200 rounded-lg p-6 text-center text-gray-500 text-sm">
      No events found matching your criteria.
    </div>
  {:else}
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {#each filteredEvents as ev (ev.id)}
        <div class="bg-white border border-gray-200 rounded-lg p-5 shadow-sm hover:shadow-md transition flex flex-col justify-between space-y-4">
          <div class="space-y-2">
            <div class="flex justify-between items-start">
              <span class="text-xs font-semibold text-indigo-600 uppercase tracking-wide">
                {ev.clubName || 'Campus Event'}
              </span>
              <button
                onclick={() => handleVote(ev.id)}
                class="flex items-center space-x-1 px-2.5 py-1 rounded text-xs font-medium border transition {ev.userVoted ? 'bg-indigo-600 text-white border-indigo-600' : 'border-indigo-200 text-indigo-600 hover:bg-indigo-50'}"
              >
                <span>▲ Upvote</span>
                <span class="font-bold">{ev.upvotes || 0}</span>
              </button>
            </div>

            <h2 class="text-xl font-bold text-gray-900">{ev.title}</h2>
            <p class="text-gray-600 text-sm line-clamp-3">{ev.description}</p>
          </div>

          <div class="space-y-3 pt-3 border-t border-gray-100 text-xs text-gray-500">
            <div>📍 <strong>Location:</strong> {ev.location}</div>
            <div>📅 <strong>Time:</strong> {formatDate(ev.startTime || (ev as any).date)}</div>

            <div class="pt-2 flex justify-between items-center">
              <a
                href="/events/{ev.id}"
                class="text-indigo-600 font-semibold hover:underline"
              >
                Discuss & Details &rarr;
              </a>
            </div>
          </div>
        </div>
      {/each}
    </div>
  {/if}
</div>