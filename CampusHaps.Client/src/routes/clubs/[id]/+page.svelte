<script lang="ts">
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import CreateEventPost from '../../../lib/components/CreateEventPost.svelte';
  import type { EventDto } from '../../../lib/types/event';

  let clubId = $derived(Number(page.params.id));
  let events = $state<EventDto[]>([]);
  let isLoadingEvents = $state(true);

  async function fetchClubEvents() {
    isLoadingEvents = true;
    try {
      const res = await fetch(`/api/events?ClubId=${clubId}`);
      if (res.ok) {
        events = await res.json();
      }
    } catch (err) {
      console.error('Failed to fetch events', err);
    } finally {
      isLoadingEvents = false;
    }
  }

  function handleEventCreated(newEvent: EventDto) {
    // Add the newly created event to the top of the event feed
    events = [newEvent, ...events];
  }

  onMount(() => {
    fetchClubEvents();
  });
</script>

<div style="max-width: 680px; margin: 2rem auto; padding: 0 1rem;">
  <h1 style="font-size: 1.75rem; font-weight: 800; margin-bottom: 1.5rem; color: #111827;">Club Feed</h1>

  <!-- Create Event Box -->
  <CreateEventPost {clubId} onEventCreated={handleEventCreated} />

  <!-- Events List -->
  {#if isLoadingEvents}
    <p style="color: #6b7280; text-align: center;">Loading club posts...</p>
  {:else if events.length === 0}
    <div style="text-align: center; padding: 2rem; background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; color: #6b7280;">
      No events posted for this club yet. Be the first to post one!
    </div>
  {:else}
    <div style="display: flex; flex-direction: column; gap: 1rem;">
      {#each events as event (event.id)}
        <div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1.25rem; text-align: left;">
          <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem;">
            <h3 style="font-size: 1.15rem; font-weight: 700; color: #111827; margin: 0;">{event.title}</h3>
            <span style="font-size: 0.75rem; background-color: #e0e7ff; color: #3730a3; padding: 0.2rem 0.5rem; border-radius: 0.25rem; font-weight: 600;">
              Type: {event.type}
            </span>
          </div>

          {#if event.description}
            <p style="font-size: 0.9rem; color: #4b5563; margin-bottom: 0.75rem; line-height: 1.4;">
              {event.description}
            </p>
          {/if}

          <div style="font-size: 0.825rem; color: #6b7280; display: flex; flex-wrap: wrap; gap: 1rem; margin-bottom: 0.75rem;">
            <span>📍 {event.location}</span>
            <span>📅 {new Date(event.startTime).toLocaleString()}</span>
          </div>

          {#if event.ticketUrl}
            <a
              href={event.ticketUrl}
              target="_blank"
              rel="noopener noreferrer"
              style="display: inline-block; font-size: 0.85rem; color: #2563eb; font-weight: 600; text-decoration: underline;"
            >
              Get Tickets / More Info →
            </a>
          {/if}
        </div>
      {/each}
    </div>
  {/if}
</div>