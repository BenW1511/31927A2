<script lang="ts">
  import { onMount } from 'svelte';
  import { getEvents, toggleVote } from '../lib/api/events';
  import { currentUser } from '../lib/stores/userStore';
  import type { Event as CampusEvent } from '../lib/types/event';

  let events = $state<CampusEvent[]>([]);
  let searchQuery = $state('');
  let selectedCategory = $state('All');
  let isLoading = $state(true);
  let errorMessage = $state('');

  const categories = ['All', 'Sport', 'Academic', 'Social', 'Club', 'Other'];

  // Maps numeric C# EventType enum values back to readable strings
  const eventTypeMap: Record<number, string> = {
    0: 'Sport',
    1: 'Academic',
    2: 'Social',
    3: 'Club',
    4: 'Other'
  };

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

  function getCategoryName(rawType: any): string {
    if (typeof rawType === 'number') return eventTypeMap[rawType] || 'Other';
    if (typeof rawType === 'string') {
      const parsed = parseInt(rawType, 10);
      if (!isNaN(parsed)) return eventTypeMap[parsed] || 'Other';
      return rawType;
    }
    return 'Other';
  }

  let filteredEvents = $derived(
    events.filter((ev) => {
      const q = searchQuery.toLowerCase();
      const matchesSearch =
        ev.title.toLowerCase().includes(q) ||
        ev.description.toLowerCase().includes(q) ||
        (ev.location && ev.location.toLowerCase().includes(q)) ||
        (ev.clubName && ev.clubName.toLowerCase().includes(q));

      const eventCategoryName = getCategoryName((ev as any).type);
      const matchesCategory =
        selectedCategory === 'All' ||
        eventCategoryName.toLowerCase() === selectedCategory.toLowerCase();

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

<div style="max-width: 900px; margin: 0 auto; padding: 1.5rem; text-align: center;">
  
  <div style="margin-bottom: 2rem;">
    <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin-bottom: 0.5rem;">
      Welcome to CampusHaps 👋
    </h1>
    <p style="color: #4b5563; font-size: 1rem; margin: 0;">
      Discover on-campus events hosted by student clubs, vote on upcoming activities, and engage with your university community.
    </p>
  </div>

  <!-- Search & Category Filter -->
  <div style="display: flex; flex-wrap: wrap; justify-content: center; gap: 0.75rem; margin-bottom: 2rem;">
    <input
      type="text"
      bind:value={searchQuery}
      placeholder="Search events by title, location, or host club..."
      style="width: 100%; max-width: 400px; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none;"
    />

    <select
      bind:value={selectedCategory}
      style="border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; background: #ffffff; outline: none;"
    >
      {#each categories as category}
        <option value={category}>{category === 'All' ? 'All Categories' : category}</option>
      {/each}
    </select>
  </div>

  {#if errorMessage}
    <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem; margin-bottom: 1.5rem;">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div style="padding: 2rem 0; color: #6b7280; font-size: 0.875rem;">Loading upcoming events...</div>
  {:else if filteredEvents.length === 0}
    <div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1.5rem; color: #6b7280; font-size: 0.875rem;">
      No events found matching your search and category selection.
    </div>
  {:else}
    <div style="display: flex; flex-direction: column; gap: 1.25rem; text-align: left;">
      {#each filteredEvents as ev (ev.id)}
        <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 1.25rem; box-shadow: 0 1px 2px rgba(0,0,0,0.05);">
          
          <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem;">
            <div style="display: flex; align-items: center; gap: 0.5rem;">
              <span style="font-size: 0.8rem; font-weight: 700; color: #4f46e5; text-transform: uppercase;">
                {ev.clubName || 'Campus Event'}
              </span>
              <span style="background-color: #f3f4f6; color: #4b5563; font-size: 0.7rem; font-weight: 600; padding: 0.15rem 0.4rem; border-radius: 0.25rem;">
                {getCategoryName((ev as any).type)}
              </span>
            </div>

            <button
              onclick={() => handleVote(ev.id)}
              style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.25rem 0.6rem; border-radius: 0.375rem; font-size: 0.8rem; font-weight: 600; cursor: pointer;"
            >
              ▲ Upvote {ev.upvotes || 0}
            </button>
          </div>

          <h2 style="font-size: 1.35rem; font-weight: 800; color: #111827; margin: 0 0 0.5rem 0;">{ev.title}</h2>
          <p style="color: #374151; font-size: 0.925rem; margin: 0 0 1rem 0; line-height: 1.5;">{ev.description}</p>

          <div style="padding-top: 0.75rem; border-top: 1px solid #f3f4f6; display: flex; justify-content: space-between; align-items: center; font-size: 0.85rem; color: #6b7280;">
            <div style="display: flex; gap: 1.5rem;">
              <span>📍 <strong>Location:</strong> {ev.location}</span>
              <span>📅 <strong>Time:</strong> {formatDate(ev.startTime || (ev as any).date)}</span>
            </div>

            <a
              href="/events/{ev.id}"
              style="color: #000000; font-weight: 700; text-decoration: underline;"
            >
              Discuss & Details &rarr;
            </a>
          </div>

        </div>
      {/each}
    </div>
  {/if}
</div>