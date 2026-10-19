<script lang="ts">
  import { onMount } from 'svelte';
  import { getEvents, toggleVote } from '../lib/api/events';
  import { getClubs } from '../lib/api/clubs';
  import { currentUser } from '../lib/stores/userStore';
  import type { Event as CampusEvent } from '../lib/types/event';
  import type { Club } from '../lib/types/club';
  import ClubCard from '../lib/components/clubs/ClubCard.svelte';

  let events = $state<CampusEvent[]>([]);
  let clubs = $state<Club[]>([]);
  let isLoading = $state(true);
  let errorMessage = $state('');

  async function loadData() {
    isLoading = true;
    errorMessage = '';
    try {
      const [fetchedEvents, fetchedClubs] = await Promise.all([
        getEvents(),
        getClubs()
      ]);
      events = fetchedEvents;
      clubs = fetchedClubs;
    } catch (err: any) {
      errorMessage = 'Failed to load home page content. Please check API.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  async function handleVote(eventId: number, currentStatus?: boolean) {
    if (!$currentUser) return;
    try {
      const res = await toggleVote(eventId, $currentUser.id, !!currentStatus);
      events = events.map((ev) =>
        ev.id === eventId
          ? { ...ev, upvotes: res.upvotes, userVoted: res.userVoted }
          : ev
      );
    } catch (err) {
      console.error('Failed to toggle vote:', err);
    }
  }

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'TBA';
    const d = new Date(dateStr);
    return isNaN(d.getTime())
      ? 'TBA'
      : d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  onMount(loadData);
</script>

<div style="max-width: 900px; margin: 0 auto; padding: 1.5rem; text-align: center;">
  
  <div style="margin-bottom: 2.5rem;">
    <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin-bottom: 0.5rem;">
      Welcome to CampusHaps 👋
    </h1>
    <p style="color: #4b5563; font-size: 1rem; margin: 0;">
      Discover on-campus events hosted by student clubs, vote on upcoming activities, and engage with your university community.
    </p>
  </div>

  {#if errorMessage}
    <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem; margin-bottom: 1.5rem;">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div style="padding: 2rem 0; color: #6b7280; font-size: 0.875rem;">Loading campus content...</div>
  {:else}
    <!-- Upcoming Events Section -->
    <div style="margin-bottom: 3rem; text-align: left;">
      <h2 style="font-size: 1.5rem; font-weight: 800; color: #111827; margin: 0 0 1rem 0;">
        Upcoming Campus Events
      </h2>

      <div style="display: flex; flex-direction: column; gap: 1.25rem;">
        {#each events as ev (ev.id)}
          <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 1.25rem; box-shadow: 0 1px 2px rgba(0,0,0,0.05);">
            <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem;">
              <span style="font-size: 0.8rem; font-weight: 700; color: #4f46e5; text-transform: uppercase;">
                {ev.clubName || 'Campus Event'}
              </span>

              <button
                onclick={() => handleVote(ev.id, ev.userVoted)}
                style="background-color: {ev.userVoted ? '#4f46e5' : '#ffffff'}; color: {ev.userVoted ? '#ffffff' : '#000000'}; border: 1px solid #d1d5db; padding: 0.25rem 0.6rem; border-radius: 0.375rem; font-size: 0.8rem; font-weight: 600; cursor: pointer;"
              >
                ▲ Upvote {ev.upvotes || 0}
              </button>
            </div>

            <h3 style="font-size: 1.35rem; font-weight: 800; color: #111827; margin: 0 0 0.5rem 0;">{ev.title}</h3>
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
    </div>

    <!-- Active Societies & Clubs Section (Identical to Clubs page card styling) -->
    <div style="text-align: left;">
      <h2 style="font-size: 1.5rem; font-weight: 800; color: #111827; margin: 0 0 1rem 0;">
        Active Student Clubs & Societies
      </h2>

      <div style="display: flex; flex-direction: column; gap: 1.25rem;">
        {#each clubs as club (club.id)}
          <ClubCard {club} {events} />
        {/each}
      </div>
    </div>
  {/if}
</div>