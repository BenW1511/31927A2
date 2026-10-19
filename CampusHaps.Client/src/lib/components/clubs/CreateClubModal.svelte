<script lang="ts">
  import { createClub } from '../../api/clubs';

  let { isOpen = false, onclose, onclubCreated } = $props();

  let name = $state('');
  let category = $state('Technology');
  let contactEmail = $state('');
  let description = $state('');

  let errorMessage = $state('');
  let isSubmitting = $state(false);

  function closeModal() {
    errorMessage = '';
    if (onclose) onclose();
  }

  async function handleSubmit(e: Event) {
    e.preventDefault();
    errorMessage = '';

    if (!name.trim() || !contactEmail.trim() || !description.trim()) {
      errorMessage = 'Please complete all required fields before submitting.';
      return;
    }

    isSubmitting = true;

    try {
      const newClub = await createClub({
        name: name.trim(),
        category,
        contactEmail: contactEmail.trim(),
        description: description.trim()
      });

      name = '';
      category = 'Technology';
      contactEmail = '';
      description = '';

      if (onclubCreated) onclubCreated(newClub);
      closeModal();
      alert('Campus club created successfully!');
    } catch (err: any) {
      errorMessage = err?.message || 'Failed to create club. Please try again.';
    } finally {
      isSubmitting = false;
    }
  }
</script>

{#if isOpen}
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-lg shadow-xl max-w-md w-full p-6 border border-gray-200">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-bold text-gray-900">Create New Campus Club</h2>
        <button
          onclick={closeModal}
          class="text-gray-400 hover:text-gray-600 font-bold text-lg"
          aria-label="Close modal"
        >
          &times;
        </button>
      </div>

      {#if errorMessage}
        <div class="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded text-sm">
          {errorMessage}
        </div>
      {/if}

      <form onsubmit={handleSubmit} class="space-y-4">
        <div>
          <label for="club-name" class="block text-sm font-medium text-gray-700 mb-1">Club Name</label>
          <input
            id="club-name"
            type="text"
            bind:value={name}
            placeholder="e.g. UTS Vibe Coding Society"
            class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>

        <div>
          <label for="club-category" class="block text-sm font-medium text-gray-700 mb-1">Category</label>
          <select
            id="club-category"
            bind:value={category}
            class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            <option value="Technology">Technology</option>
            <option value="Academic">Academic</option>
            <option value="Arts & Culture">Arts & Culture</option>
            <option value="Sports & Fitness">Sports & Fitness</option>
            <option value="Social & Special Interest">Social & Special Interest</option>
          </select>
        </div>

        <div>
          <label for="club-email" class="block text-sm font-medium text-gray-700 mb-1">Contact Email</label>
          <input
            id="club-email"
            type="email"
            bind:value={contactEmail}
            placeholder="e.g. contact@society.edu.au"
            class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>

        <div>
          <label for="club-description" class="block text-sm font-medium text-gray-700 mb-1">Description</label>
          <textarea
            id="club-description"
            rows="3"
            bind:value={description}
            placeholder="Brief overview of the club..."
            class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          ></textarea>
        </div>

        <div class="flex justify-end space-x-2 pt-2">
          <button
            type="button"
            onclick={closeModal}
            class="px-4 py-2 border border-gray-300 rounded-md text-sm text-gray-700 hover:bg-gray-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700 disabled:opacity-50"
          >
            {isSubmitting ? 'Creating...' : 'Create Club'}
          </button>
        </div>
      </form>
    </div>
  </div>
{/if}