<script lang="ts">
  import { onMount } from 'svelte';
  import type { Comment } from '../../types/comment';
  import { getEventComments } from '../../api/comments';
  import CommentItem from './CommentItem.svelte';
  import CommentForm from './CommentForm.svelte';
  import LoadingSpinner from '../LoadingSpinner.svelte';

  let { eventId }: { eventId: number } = $props();

  let comments = $state<Comment[]>([]);
  let loading = $state(true);
  let error = $state('');

  async function loadComments() {
    try {
      loading = true;
      comments = await getEventComments(eventId);
    } catch (err: any) {
      error = 'Failed to load comments';
    } finally {
      loading = false;
    }
  }

  onMount(loadComments);
</script>

<section class="mt-8 pt-6 border-t border-gray-200">
  <h3 class="text-lg font-bold text-gray-900 mb-4">Event Discussion</h3>

  <CommentForm {eventId} on:added={loadComments} />

  {#if loading}
    <LoadingSpinner />
  {:else if error}
    <p class="text-red-500 text-sm">{error}</p>
  {:else if comments.length === 0}
    <p class="text-gray-500 text-sm py-4 text-center">No comments yet. Be the first to start the conversation!</p>
  {:else}
    <div class="space-y-3 mt-4">
      {#each comments as comment (comment.id)}
        <CommentItem {comment} {eventId} on:updated={loadComments} on:deleted={loadComments} />
      {/each}
    </div>
  {/if}
</section>