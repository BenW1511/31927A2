<script lang="ts">
  import { createEventDispatcher } from 'svelte';
  import Modal from '../Modal.svelte';
  import { createClubPost } from '../../api/clubs';
  import { currentUser } from '../../stores/userStore';
  import { addNotif } from '../../stores/notifStore';

  let { isOpen = $bindable(false), clubId }: { isOpen: boolean; clubId: number } = $props();

  const dispatch = createEventDispatcher();

  let title = $state('');
  let content = $state('');
  let loading = $state(false);

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    if (!$currentUser) {
      addNotif('You must be logged in to post', 'error');
      return;
    }

    if (!title || !content) {
      addNotif('Please enter both title and content', 'error');
      return;
    }

    try {
      loading = true;
      await createClubPost(clubId, {
        title,
        content,
        authorId: $currentUser.id
      });
      addNotif('Post published!', 'success');
      dispatch('created');
      isOpen = false;
      title = '';
      content = '';
    } catch (err: any) {
      addNotif(err.message || 'Failed to create post', 'error');
    } finally {
      loading = false;
    }
  }
</script>

<Modal title="Create Club Post" {isOpen} on:close={() => (isOpen = false)}>
  <form onsubmit={handleSubmit} class="space-y-4">
    <div>
      <label class="block text-xs font-bold text-gray-700 mb-1" for="post-title">Post Title</label>
      <input id="post-title" type="text" bind:value={title} class="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 outline-none" placeholder="e.g., Upcoming TypeScript Workshop" />
    </div>

    <div>
      <label class="block text-xs font-bold text-gray-700 mb-1" for="post-content">Content</label>
      <textarea id="post-content" bind:value={content} rows="4" class="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500 outline-none" placeholder="Share announcement details with students..."></textarea>
    </div>

    <div class="flex justify-end gap-2 pt-2">
      <button type="button" onclick={() => (isOpen = false)} class="px-4 py-2 text-sm text-gray-600 hover:bg-gray-100 rounded">Cancel</button>
      <button type="submit" disabled={loading} class="px-4 py-2 text-sm bg-indigo-600 text-white rounded font-semibold hover:bg-indigo-700 disabled:opacity-50">
        {loading ? 'Publishing...' : 'Publish Post'}
      </button>
    </div>
  </form>
</Modal>