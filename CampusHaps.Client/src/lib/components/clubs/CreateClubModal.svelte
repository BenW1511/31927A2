<script lang="ts">
  import { createClub } from '../../api/clubs';

  interface Props {
    isOpen: boolean;
    onclose: () => void;
    onclubCreated: () => void;
  }

  let { isOpen, onclose, onclubCreated }: Props = $props();

  let name = $state('');
  let category = $state('Social');
  let contactEmail = $state('');
  let description = $state('');
  let errorMessage = $state('');
  let isLoading = $state(false);

  const categories = [
    'Social',
    'Technology',
    'Academic',
    'Arts & Culture',
    'Sports & Fitness',
    'Social & Special Interest'
  ];

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    errorMessage = '';

    if (!name.trim() || !contactEmail.trim()) {
      errorMessage = 'Please provide a club name and contact email.';
      return;
    }

    isLoading = true;

    try {
      await createClub({
        name: name.trim(),
        category,
        contactEmail: contactEmail.trim(),
        description: description.trim()
      });

      // Reset form fields
      name = '';
      category = 'Social';
      contactEmail = '';
      description = '';

      onclubCreated();
      onclose();
    } catch (err: any) {
      errorMessage = err?.message || 'Failed to create club.';
    } finally {
      isLoading = false;
    }
  }
</script>

{#if isOpen}
  <div style="position: fixed; inset: 0; background-color: rgba(0, 0, 0, 0.4); display: flex; align-items: center; justify-content: center; z-index: 50; padding: 1rem;">
    <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; width: 100%; max-width: 480px; padding: 1.75rem; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1); position: relative; text-align: left;">
      
    
      <button
        type="button"
        onclick={onclose}
        style="position: absolute; top: 1rem; right: 1rem; background: transparent; border: none; font-size: 1.25rem; font-weight: 700; color: #6b7280; cursor: pointer;"
      >
        ✕
      </button>

      <h2 style="font-size: 1.5rem; font-weight: 800; color: #111827; margin: 0 0 1.25rem 0; text-align: center;">
        Create New Campus Club
      </h2>

      {#if errorMessage}
        <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem; margin-bottom: 1rem;">
          {errorMessage}
        </div>
      {/if}

      <form onsubmit={handleSubmit} style="display: flex; flex-direction: column; gap: 1rem;">
        <div>
          <label for="clubName" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Club Name
          </label>
          <input
            id="clubName"
            type="text"
            bind:value={name}
            placeholder="e.g. UTS Vibe Coding Society"
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
          />
        </div>

        <div>
          <label for="category" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Category
          </label>
          <select
            id="category"
            bind:value={category}
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; background: #ffffff; outline: none; box-sizing: border-box;"
          >
            {#each categories as cat}
              <option value={cat}>{cat}</option>
            {/each}
          </select>
        </div>

        <div>
          <label for="contactEmail" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Contact Email
          </label>
          <input
            id="contactEmail"
            type="email"
            bind:value={contactEmail}
            placeholder="e.g. contact@society.edu.au"
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
          />
        </div>

        <div>
          <label for="description" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Description
          </label>
          <textarea
            id="description"
            rows="3"
            bind:value={description}
            placeholder="Brief overview of the club..."
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box; resize: vertical;"
          ></textarea>
        </div>

       
        <div style="display: flex; justify-content: flex-end; gap: 0.75rem; margin-top: 0.5rem;">
          <button
            type="button"
            onclick={onclose}
            style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.5rem 1rem; border-radius: 0.375rem; font-size: 0.875rem; font-weight: 600; cursor: pointer;"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isLoading}
            style="background-color: #ffffff; color: #000000; border: 1px solid #000000; padding: 0.5rem 1rem; border-radius: 0.375rem; font-size: 0.875rem; font-weight: 700; cursor: pointer;"
          >
            {isLoading ? 'Creating...' : 'Create Club'}
          </button>
        </div>
      </form>

    </div>
  </div>
{/if}
