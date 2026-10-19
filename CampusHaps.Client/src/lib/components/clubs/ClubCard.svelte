<script lang="ts">
  import type { Club } from '../../types/club';
  import type { Event as CampusEvent } from '../../types/event';

  interface Props {
    club: Club;
    events?: CampusEvent[];
  }

  let { club, events = [] }: Props = $props();

  // Filter events hosted by this club
  let clubEvents = $derived(
    events.filter((ev) => ev.clubId === club.id)
  );

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'TBA';
    const d = new Date(dateStr);
    return isNaN(d.getTime())
      ? 'TBA'
      : d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
</script>

<div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 1.25rem; box-shadow: 0 1px 2px rgba(0,0,0,0.05); text-align: left; width: 100%; box-sizing: border-box;">
  
  <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem;">
    <div style="display: flex; align-items: center; gap: 0.5rem;">
      <span style="font-size: 0.8rem; font-weight: 700; color: #4f46e5; text-transform: uppercase;">
        {club.name}
      </span>
      <span style="background-color: #f3f4f6; color: #4b5563; font-size: 0.7rem; font-weight: 600; padding: 0.15rem 0.4rem; border-radius: 0.25rem;">
        {club.category}
      </span>
    </div>
  </div>

  <h2 style="font-size: 1.35rem; font-weight: 800; color: #111827; margin: 0 0 0.5rem 0;">{club.name}</h2>
  <p style="color: #374151; font-size: 0.925rem; margin: 0 0 1rem 0; line-height: 1.5;">{club.description}</p>

  <!-- Upcoming Events Preview Section -->
  <div style="background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 0.375rem; padding: 0.85rem; margin-bottom: 1rem;">
    <div style="font-size: 0.75rem; font-weight: 700; color: #4b5563; text-transform: uppercase; margin-bottom: 0.5rem;">
      📅 Upcoming Events ({clubEvents.length})
    </div>

    {#if clubEvents.length === 0}
      <p style="font-size: 0.825rem; color: #6b7280; margin: 0;">No scheduled upcoming events for this club yet.</p>
    {:else}
      <div style="display: flex; flex-direction: column; gap: 0.5rem;">
        {#each clubEvents as ev (ev.id)}
          <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.25rem; padding: 0.6rem 0.75rem; display: flex; justify-content: space-between; align-items: center;">
            <div>
              <strong style="font-size: 0.875rem; color: #111827; display: block;">{ev.title}</strong>
              <span style="font-size: 0.75rem; color: #6b7280;">
                📍 {ev.location} | 📅 {formatDate(ev.startTime || (ev as any).date)}
              </span>
            </div>
            <a
              href="/events/{ev.id}"
              style="font-size: 0.75rem; color: #000000; font-weight: 700; text-decoration: underline; white-space: nowrap; margin-left: 0.5rem;"
            >
              Details &rarr;
            </a>
          </div>
        {/each}
      </div>
    {/if}
  </div>

  <div style="padding-top: 0.75rem; border-top: 1px solid #f3f4f6; display: flex; justify-content: space-between; align-items: center; font-size: 0.85rem; color: #6b7280;">
    <div style="display: flex; gap: 1rem;">
      <span>📅 <strong>{club.eventCount ?? clubEvents.length}</strong> Events</span>
      <span>💬 <strong>{club.postCount ?? 0}</strong> Posts</span>
    </div>

    <a
      href="/clubs/{club.id}"
      style="color: #000000; font-weight: 700; text-decoration: underline;"
    >
      View Club &rarr;
    </a>
  </div>

</div>