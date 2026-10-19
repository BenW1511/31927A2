<script lang="ts">
  import { onMount } from 'svelte';
  import { getEvents } from '../../lib/api/events';
  import type { Event as CampusEvent } from '../../lib/types/event';

  let events = $state<CampusEvent[]>([]);
  let isLoading = $state(true);
  let errorMessage = $state('');

  let currentDate = $state(new Date());

  let year = $derived(currentDate.getFullYear());
  let month = $derived(currentDate.getMonth());

  const monthNames = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  const daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  async function loadCalendarEvents() {
    isLoading = true;
    errorMessage = '';
    try {
      events = await getEvents();
    } catch (err: any) {
      errorMessage = 'Failed to load calendar events. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  function prevMonth() {
    currentDate = new Date(year, month - 1, 1);
  }

  function nextMonth() {
    currentDate = new Date(year, month + 1, 1);
  }

  // Calculate calendar grid days
  let calendarDays = $derived(() => {
    const firstDayOfMonth = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    const days: Array<{ dayNumber: number | null; dateStr: string | null; events: CampusEvent[] }> = [];

    // Empty slots before the first day of the month
    for (let i = 0; i < firstDayOfMonth; i++) {
      days.push({ dayNumber: null, dateStr: null, events: [] });
    }

    // Days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      const dateObj = new Date(year, month, day);
      const dateIsoStr = dateObj.toISOString().split('T')[0];

      const dayEvents = events.filter((ev) => {
        const rawDate = ev.startTime || (ev as any).date;
        if (!rawDate) return false;
        const evDate = new Date(rawDate);
        return (
          !isNaN(evDate.getTime()) &&
          evDate.getFullYear() === year &&
          evDate.getMonth() === month &&
          evDate.getDate() === day
        );
      });

      days.push({
        dayNumber: day,
        dateStr: dateIsoStr,
        events: dayEvents
      });
    }

    return days;
  });

  onMount(loadCalendarEvents);
</script>

<div style="max-width: 1000px; margin: 0 auto; padding: 1.5rem; text-align: center;">
  
  <div style="margin-bottom: 1.5rem;">
    <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin-bottom: 0.25rem;">Campus Calendar</h1>
    <p style="color: #4b5563; font-size: 0.95rem;">View scheduled campus events across the month.</p>
  </div>

  <!-- Month Navigation Header -->
  <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem; background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 0.75rem 1.25rem;">
    <button
      onclick={prevMonth}
      style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.4rem 0.85rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer;"
    >
      &larr; Previous
    </button>

    <h2 style="font-size: 1.35rem; font-weight: 800; color: #111827; margin: 0;">
      {monthNames[month]} {year}
    </h2>

    <button
      onclick={nextMonth}
      style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.4rem 0.85rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer;"
    >
      Next &rarr;
    </button>
  </div>

  {#if errorMessage}
    <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem; margin-bottom: 1rem;">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div style="padding: 2rem 0; color: #6b7280; font-size: 0.875rem;">Loading calendar grid...</div>
  {:else}
    <!-- Calendar Grid Table -->
    <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
      
      <!-- Days of Week Row -->
      <div style="display: grid; grid-template-columns: repeat(7, 1fr); background-color: #f9fafb; border-bottom: 1px solid #d1d5db;">
        {#each daysOfWeek as dayName}
          <div style="padding: 0.75rem 0.25rem; font-size: 0.8rem; font-weight: 700; color: #374151; text-transform: uppercase;">
            {dayName}
          </div>
        {/each}
      </div>

      <!-- Calendar Cells -->
      <div style="display: grid; grid-template-columns: repeat(7, 1fr); auto-rows: minmax(100px, auto);">
        {#each calendarDays() as cell, index}
          <div style="border-right: 1px solid #e5e7eb; border-bottom: 1px solid #e5e7eb; padding: 0.5rem; background-color: {cell.dayNumber ? '#ffffff' : '#f9fafb'}; text-align: left; min-height: 90px; box-sizing: border-box;">
            {#if cell.dayNumber}
              <div style="font-size: 0.85rem; font-weight: 700; color: #111827; margin-bottom: 0.35rem;">
                {cell.dayNumber}
              </div>

              <!-- Events for this day -->
              <div style="display: flex; flex-direction: column; gap: 0.25rem;">
                {#each cell.events as ev (ev.id)}
                  <a
                    href="/events/{ev.id}"
                    style="background-color: #e0e7ff; color: #3730a3; border: 1px solid #c7d2fe; padding: 0.2rem 0.35rem; border-radius: 0.25rem; font-size: 0.725rem; font-weight: 600; text-decoration: none; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; display: block;"
                    title="{ev.title} ({ev.location})"
                  >
                    • {ev.title}
                  </a>
                {/each}
              </div>
            {/if}
          </div>
        {/each}
      </div>

    </div>
  {/if}
</div>