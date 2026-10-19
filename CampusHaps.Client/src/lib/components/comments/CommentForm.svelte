<script lang="ts">
  import { createEventDispatcher } from 'svelte';
  import { createComment } from '../../api/comments';
  import { currentUser } from '../../stores/userStore';
  import { addNotif } from '../../stores/notifStore';

  let { eventId }: { eventId: number } = $props();

  const dispatch = createEventDispatcher();

  let content = $state('');
  let loading = $state(false);

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    if (!$currentUser) {
      addNotif('Please log in to post a comment', 'error');
      return;
    }

    if (!content.trim()) return;

    try {
      loading = true;
      await createComment(eventId, {
        userId: $currentUser.id,
        content
      });
      content = '';
      addNotif('Comment posted', 'success');
      dispatch('added');
    } catch (err: any) {
      addNotif(err.message || 'Failed to post comment', 'error');
    } finally {
      loading = false;
    }
  }
</script>

<form onsubmit={handleSubmit} class="space-y-2 mb-4">
  <textarea bind:value={content} rows="2" placeholder={$currentUser ? "Write a comment..." : "Log in to join the discussion"} disabled={!$currentUser || loading} class="w-full border border-gray-300 rounded-lg p-3 text-sm focus:ring-2 focus:ring-indigo-500 outline-none disabled:bg-gray-100"></textarea>

  <div class="flex justify-end">
    <button type="submit" disabled={!$currentUser || !content.trim() || loading} class="px-4 py-2 bg-indigo-600 text-white font-semibold rounded-md text-xs hover:bg-indigo-700 disabled:opacity-50">
      {loading ? 'Posting...' : 'Post Comment'}
    </button>
  </div>
</form>