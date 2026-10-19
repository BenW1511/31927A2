<script lang="ts">
  import { onMount } from 'svelte';
  import { getEventsByDateRange } from '../../lib/api/events';
  import type { Event as CampusEvent } from '../../lib/types/event';

  let currentDate = $state(new Date());
  let events = $state<CampusEvent[]>([]);
  let isLoading = $state(true);
  let errorMessage = $state('');

  let year = $derived(currentDate.getFullYear());
  let month = $derived(currentDate.getMonth());

  const monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  const daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  // Calculate calendar grid days for current month
  let calendarDays = $derived(() => {
    const firstDayOfMonth = new Date(year, month, 1);
    const lastDayOfMonth = new Date(year, month + 1, 0);

    const startingDayOfWeek = firstDayOfMonth.getDay();
    const totalDays = lastDayOfMonth.getDate();

    const days: Array<{ date: number | null; fullDate: Date | null }> = [];

    // Empty lead-in slots for previous month padding
    for (let i = 0; i < startingDayOfWeek; i++) {
      days.push({ date: null, fullDate: null });
    }

    // Days of active month
    for (let day = 1; day <= totalDays; day++) {
      days.push({
        date: day,
        fullDate: new Date(year, month, day)
      });
    }

    return days;
  });

  async function loadMonthEvents() {
    isLoading = true;
    errorMessage = '';

    // Calculate month boundary range for Wud's backend API
    const from = new Date(year, month, 1, 0, 0, 0);
    const to = new Date(year, month + 1, 0, 23, 59, 59);

    try {
      events = await getEventsByDateRange(from, to);
    } catch (err: any) {
      errorMessage = 'Failed to load calendar events from API.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  function changeMonth(delta: number) {
    currentDate = new Date(year, month + delta, 1);
    loadMonthEvents();
  }

  function getEventsForDay(dayNumber: number): CampusEvent[] {
    return events.filter((ev) => {
      const startTime = ev.startTime || (ev as any).date;
      if (!startTime) return false;
      const d = new Date(startTime);
      return (
        d.getFullYear() === year &&
        d.getMonth() === month &&
        d.getDate() === dayNumber
      );
    });
  }

  onMount(loadMonthEvents);
</script>

<div style="max-width: 1100px; margin: 0 auto; padding: 1rem;">
  <!-- Header Navigation -->
  <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
    <div>
      <h1 style="font-size: 1.875rem; font-weight: 700; color: #111827; margin: 0;">Campus Event Calendar</h1>
      <p style="color: #4b5563; font-size: 0.875rem; margin-top: 0.25rem;">Browse scheduled student activities by date.</p>
    </div>

    <div style="display: flex; align-items: center; gap: 0.75rem; background: #ffffff; border: 1px solid #e5e7eb; padding: 0.5rem 0.75rem; border-radius: 0.375rem; box-shadow: 0 1px 2px rgba(0,0,0,0.05);">
      <button
        onclick={() => changeMonth(-1)}
        style="padding: 0.375rem 0.75rem; background: #f3f4f6; border: 1px solid #d1d5db; border-radius: 0.25rem; font-weight: 600; font-size: 0.875rem; cursor: pointer;"
      >
        &larr; Prev
      </button>
      <span style="font-size: 1.125rem; font-weight: 700; color: #1f2937; min-width: 160px; text-align: center; display: inline-block;">
        {monthNames[month]} {year}
      </span>
      <button
        onclick={() => changeMonth(1)}
        style="padding: 0.375rem 0.75rem; background: #f3f4f6; border: 1px solid #d1d5db; border-radius: 0.25rem; font-weight: 600; font-size: 0.875rem; cursor: pointer;"
      >
        Next &rarr;
      </button>
    </div>
  </div>

  {#if errorMessage}
    <div style="padding: 1rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem; margin-bottom: 1rem;">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div style="text-center; padding: 3rem 0; color: #6b7280; font-size: 0.875rem;">Loading calendar events...</div>
  {:else}
    <!-- Calendar Main Box -->
    <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
      
      <!-- Weekday Headers (Strict 7 Columns) -->
      <div style="display: grid; grid-template-columns: repeat(7, 1fr); background: #f9fafb; border-bottom: 1px solid #e5e7eb; text-align: center; font-weight: 700; font-size: 0.75rem; color: #4b5563; text-transform: uppercase; padding: 0.75rem 0;">
        {#each daysOfWeek as day}
          <div>{day}</div>
        {/each}
      </div>

      <!-- Month Day Cells (Strict 7 Columns Grid) -->
      <div style="display: grid; grid-template-columns: repeat(7, 1fr); background: #e5e7eb; gap: 1px;">
        {#each calendarDays() as cell}
          {#if cell.date === null}
            <div style="background: #f9fafb; min-height: 110px; padding: 0.5rem;"></div>
          {:else}
            <div style="background: #ffffff; min-height: 110px; padding: 0.5rem; display: flex; flex-direction: column; justify-content: space-between;">
              <div style="text-align: right; font-size: 0.875rem; font-weight: 700; color: #374151; margin-bottom: 0.25rem;">
                {cell.date}
              </div>

              <div style="display: flex; flex-direction: column; gap: 0.25rem; overflow-y: auto; max-height: 80px;">
                {#each getEventsForDay(cell.date) as ev (ev.id)}
                  <a
                    href="/events/{ev.id}"
                    style="display: block; font-size: 0.75rem; padding: 0.25rem 0.375rem; background: #e0e7ff; color: #3730a3; border: 1px solid #c7d2fe; border-radius: 0.25rem; text-decoration: none; font-weight: 600; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;"
                    title={ev.title}
                  >
                    📌 {ev.title}
                  </a>
                {/each}
              </div>
            </div>
          {/if}
        {/each}
      </div>

    </div>
  {/if}
</div>