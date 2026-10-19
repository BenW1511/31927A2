<script lang="ts">
  import { onMount } from 'svelte';
  import type { Event } from '../lib/types/event';
  import { getEvents, toggleVote } from '../lib/api/events';
  import { currentUser } from '../lib/stores/userStore';
  import { addNotif } from '../lib/stores/notifStore';
  import LoadingSpinner from '../lib/components/LoadingSpinner.svelte';

  let events = $state<Event[]>([]);
  let loading = $state(true);

  async function loadEvents() {
    try {
      loading = true;
      events = await getEvents();
    } catch (err: any) {
      addNotif('Failed to load events', 'error');
    } finally {
      loading = false;
    }
  }

  async function handleVote(eventId: number) {
    if (!$currentUser) {
      addNotif('Please sign in to vote', 'error');
      return;
    }

    try {
      const result = await toggleVote(eventId, $currentUser.id);
      events = events.map((e) =>
        e.id === eventId ? { ...e, upvotes: result.upvotes, userVoted: result.userVoted } : e
      );
    } catch (err: any) {
      addNotif('Failed to process vote', 'error');
    }
  }

  onMount(loadEvents);
</script>

<div class="space-y-6">
  <div class="bg-indigo-600 text-white rounded-xl p-8 shadow-md">
    <h1 class="text-3xl font-extrabold">Welcome to CampusHaps 👋</h1>
    <p class="mt-2 text-indigo-100 max-w-2xl">
      Discover on-campus events hosted by student clubs, vote on upcoming activities, and engage with your university community.
    </p>
  </div>

  <section>
    <h2 class="text-xl font-bold text-gray-900 mb-4">Upcoming Campus Events</h2>

    {#if loading}
      <LoadingSpinner />
    {:else if events.length === 0}
      <p class="text-gray-500 py-8 text-center bg-white rounded-lg border border-gray-200">No upcoming events right now.</p>
    {:else}
      <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {#each events as event (event.id)}
          <div class="bg-white border border-gray-200 rounded-lg p-5 shadow-sm hover:shadow-md transition flex flex-col justify-between">
            <div>
              <div class="flex justify-between items-start mb-2">
                <span class="text-xs font-semibold text-indigo-600 bg-indigo-50 px-2 py-1 rounded">{event.clubName}</span>
                <button
                  onclick={() => handleVote(event.id)}
                  class={`flex items-center gap-1 text-xs font-bold px-2.5 py-1 rounded-full border transition ${
                    event.userVoted ? 'bg-indigo-600 text-white border-indigo-600' : 'bg-gray-50 text-gray-700 border-gray-300 hover:bg-gray-100'
                  }`}
                >
                  ▲ {event.upvotes}
                </button>
              </div>
              <h3 class="text-lg font-bold text-gray-900">{event.title}</h3>
              <p class="text-sm text-gray-600 mt-2 line-clamp-3">{event.description}</p>
            </div>

            <div class="mt-4 pt-3 border-t border-gray-100 flex justify-between items-center text-xs text-gray-500">
              <span>📍 {event.location}</span>
              <a href={`/events/${event.id}`} class="text-indigo-600 font-semibold hover:underline">Discuss &rarr;</a>
            </div>
          </div>
        {/each}
      </div>
    {/if}
  </section>
</div>