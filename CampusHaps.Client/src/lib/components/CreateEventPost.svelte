<script lang="ts">
  import { EventType, type EventDto } from '../types/event';

  interface Props {
      clubId: number;
      onEventCreated?: (newEvent: EventDto) => void;
  }

  let { clubId, onEventCreated }: Props = $props();

  // Form field states
  let title = $state('');
  let description = $state('');
  let location = $state('');
  let startTime = $state('');
  let endTime = $state('');
  let ticketUrl = $state('');
  let type = $state<EventType>(EventType.Social);

  // UI states
  let isExpanded = $state(false);
  let isLoading = $state(false);
  let errorMessage = $state('');

  function resetForm() {
    title = '';
    description = '';
    location = '';
    startTime = '';
    endTime = '';
    ticketUrl = '';
    type = EventType.Social;
    errorMessage = '';
    isExpanded = false;
  }

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    errorMessage = '';

    if (!title.trim() || !location.trim() || !startTime || !endTime) {
      errorMessage = 'Please fill out all required fields (Title, Location, Start Time, and End Time).';
      return;
    }

    if (new Date(endTime) <= new Date(startTime)) {
      errorMessage = 'End time must be after the start time.';
      return;
    }

    isLoading = true;

    try {
      const payload = {
        title: title.trim(),
        description: description.trim(),
        location: location.trim(),
        startTime: new Date(startTime).toISOString(),
        endTime: new Date(endTime).toISOString(),
        ticketUrl: ticketUrl.trim() || null,
        type: Number(type),
        clubId: Number(clubId)
      };

      const response = await fetch('/api/events', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });

      if (response.ok) {
        const createdEvent: EventDto = await response.json();
        if (onEventCreated) onEventCreated(createdEvent);
        resetForm();
      } else {
        const responseText = await response.text();
        try {
          const jsonErr = JSON.parse(responseText);
          if (jsonErr.errors) {
            const firstKey = Object.keys(jsonErr.errors)[0];
            errorMessage = jsonErr.errors[firstKey][0];
          } else {
            errorMessage = jsonErr.title || responseText;
          }
        } catch {
          errorMessage = responseText || `Failed to create event (Status ${response.status}).`;
        }
      }
    } catch (err) {
      errorMessage = 'Unable to connect to the backend server. Ensure the API is running.';
    } finally {
      isLoading = false;
    }
  }
</script>

<div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1.25rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); margin-bottom: 1.5rem;">
  {#if !isExpanded}
    <button
      type="button"
      onclick={() => (isExpanded = true)}
      style="width: 100%; text-align: left; background: #f9fafb; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.75rem 1rem; color: #6b7280; font-size: 0.9rem; cursor: pointer;"
    >
      ✏️ Create a new event for this club...
    </button>
  {:else}
    <form onsubmit={handleSubmit} style="display: flex; flex-direction: column; gap: 1rem; text-align: left;">
      <div style="display: flex; justify-content: space-between; align-items: center;">
        <h2 style="font-size: 1.1rem; font-weight: 700; color: #111827; margin: 0;">Post New Event</h2>
        <button
          type="button"
          onclick={resetForm}
          disabled={isLoading}
          style="background: none; border: none; color: #6b7280; cursor: pointer; font-weight: 600; font-size: 0.85rem;"
        >
          Cancel
        </button>
      </div>

      {#if errorMessage}
        <div style="padding: 0.6rem 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem;">
          {errorMessage}
        </div>
      {/if}

      //Title input
      <div>
        <label for="title" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          Event Title *
        </label>
        <input
          id="title"
          type="text"
          bind:value={title}
          placeholder="e.g., Weekly Coding Workshop"
          maxlength="150"
          disabled={isLoading}
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.875rem; box-sizing: border-box;"
        />
      </div>

      //Description input
      <div>
        <label for="description" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          Description
        </label>
        <textarea
          id="description"
          bind:value={description}
          placeholder="What is this event about?"
          rows="3"
          maxlength="2000"
          disabled={isLoading}
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.875rem; box-sizing: border-box;"
        ></textarea>
      </div>

      //Location and type grid
      <div style="display: flex; gap: 0.75rem;">
        <div style="flex: 2;">
          <label for="location" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Location *
          </label>
          <input
            id="location"
            type="text"
            bind:value={location}
            placeholder="e.g., CB11.04.401"
            maxlength="200"
            disabled={isLoading}
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.875rem; box-sizing: border-box;"
          />
        </div>

        <div style="flex: 1;">
          <label for="type" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Category
          </label>
          <select
            id="type"
            bind:value={type}
            disabled={isLoading}
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.875rem; box-sizing: border-box;"
          >
            <option value={EventType.Sport}>Sport</option>
            <option value={EventType.Academic}>Academic</option>
            <option value={EventType.Social}>Social</option>
            <option value={EventType.Club}>Club</option>
            <option value={EventType.Other}>Other</option>
          </select>
        </div>
      </div>

      //start and end time grid
      <div style="display: flex; gap: 0.75rem;">
        <div style="flex: 1;">
          <label for="startTime" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Start Time *
          </label>
          <input
            id="startTime"
            type="datetime-local"
            bind:value={startTime}
            disabled={isLoading}
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.85rem; box-sizing: border-box;"
          />
        </div>

        <div style="flex: 1;">
          <label for="endTime" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            End Time *
          </label>
          <input
            id="endTime"
            type="datetime-local"
            bind:value={endTime}
            disabled={isLoading}
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.85rem; box-sizing: border-box;"
          />
        </div>
      </div>

      //ticket url
      <div>
        <label for="ticketUrl" style="display: block; font-size: 0.8rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          Ticket / Link URL
        </label>
        <input
          id="ticketUrl"
          type="url"
          bind:value={ticketUrl}
          placeholder="https://..."
          maxlength="500"
          disabled={isLoading}
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem; font-size: 0.875rem; box-sizing: border-box;"
        />
      </div>

      
      <button
        type="submit"
        disabled={isLoading}
        style="width: 100%; background-color: #2563eb; color: #ffffff; border: none; padding: 0.6rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer; margin-top: 0.25rem;"
      >
        {isLoading ? 'Posting Event...' : 'Post Event'}
      </button>
    </form>
  {/if}
</div>
