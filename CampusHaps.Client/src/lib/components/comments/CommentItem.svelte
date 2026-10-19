<script lang="ts">
  import { createEventDispatcher } from 'svelte';
  import type { Comment } from '../../types/comment';
  import { updateComment, deleteComment } from '../../api/comments';
  import { currentUser } from '../../stores/userStore';
  import { addNotif } from '../../stores/notifStore';

  let { comment, eventId }: { comment: Comment; eventId: number } = $props();

  const dispatch = createEventDispatcher();

  let isEditing = $state(false);
  let editContent = $state('');
  let loading = $state(false);

  // Sync editContent when comment prop changes/loads
  $effect(() => {
    editContent = comment.content;
  });

  let isOwner = $derived($currentUser?.id === comment.userId);

  async function handleUpdate() {
    if (!editContent.trim()) return;

    try {
      loading = true;
      await updateComment(eventId, comment.id, {
        userId: $currentUser!.id,
        content: editContent
      });
      addNotif('Comment updated', 'success');
      isEditing = false;
      dispatch('updated');
    } catch (err: any) {
      addNotif(err.message || 'Failed to update comment', 'error');
    } finally {
      loading = false;
    }
  }

  async function handleDelete() {
    if (!confirm('Are you sure you want to delete this comment?')) return;

    try {
      loading = true;
      await deleteComment(eventId, comment.id, $currentUser!.id);
      addNotif('Comment deleted', 'success');
      dispatch('deleted');
    } catch (err: any) {
      addNotif(err.message || 'Failed to delete comment', 'error');
    } finally {
      loading = false;
    }
  }
</script>

<div class="bg-gray-50 border border-gray-200 rounded-lg p-3 text-sm">
  <div class="flex justify-between items-center mb-1 text-xs text-gray-500">
    <span class="font-semibold text-gray-700">{comment.userName}</span>
    <div class="flex items-center gap-2">
      <time>{new Date(comment.createdAt).toLocaleDateString()}</time>
      {#if isOwner}
        <button onclick={() => (isEditing = !isEditing)} class="text-indigo-600 hover:underline">
          {isEditing ? 'Cancel' : 'Edit'}
        </button>
        <button onclick={handleDelete} class="text-red-600 hover:underline" disabled={loading}>
          Delete
        </button>
      {/if}
    </div>
  </div>

  {#if isEditing}
    <div class="mt-2 space-y-2">
      <textarea bind:value={editContent} rows="2" class="w-full border border-gray-300 rounded p-2 text-sm outline-none focus:ring-1 focus:ring-indigo-500"></textarea>
      <button onclick={handleUpdate} disabled={loading} class="px-3 py-1 bg-indigo-600 text-white rounded text-xs font-semibold hover:bg-indigo-700">
        Save
      </button>
    </div>
  {:else}
    <p class="text-gray-800">{comment.content}</p>
  {/if}
</div>