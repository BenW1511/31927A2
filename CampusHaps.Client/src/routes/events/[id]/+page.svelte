<script lang="ts">
  import { onMount } from 'svelte';
  import { getEventById, toggleVote } from '../../../lib/api/events';
  import { getEventComments, createComment } from '../../../lib/api/comments';
  import { currentUser } from '../../../lib/stores/userStore';
  import type { Event as CampusEvent } from '../../../lib/types/event';
  import type { Comment } from '../../../lib/types/comment';

  interface Props {
    data?: {
      id?: string;
      [key: string]: any;
    };
  }

  let { data }: Props = $props();

  let event = $state<CampusEvent | null>(null);
  let comments = $state<Comment[]>([]);
  let isLoading = $state(true);
  let errorMsg = $state('');

  let commentText = $state('');
  let isSubmitting = $state(false);
  let commentError = $state('');

  function getRouteId(): number {
    if (data?.id) return Number(data.id);
    if (typeof window !== 'undefined') {
      const parts = window.location.pathname.split('/').filter(Boolean);
      const lastPart = parts[parts.length - 1];
      if (lastPart && !isNaN(Number(lastPart))) {
        return Number(lastPart);
      }
    }
    return 0;
  }

  async function loadEventData() {
    const eventId = getRouteId();

    if (!eventId) {
      errorMsg = 'Invalid Event ID.';
      isLoading = false;
      return;
    }

    isLoading = true;
    errorMsg = '';

    try {
      const [fetchedEvent, fetchedComments] = await Promise.all([
        getEventById(eventId),
        getEventComments(eventId)
      ]);
      event = fetchedEvent;
      comments = fetchedComments;
    } catch (err: any) {
      errorMsg = 'Failed to load event details. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  async function handleVote() {
    if (!event || !$currentUser) return;
    try {
      const result = await toggleVote(event.id, $currentUser.id);
      event.upvotes = result.upvotes;
      event.userVoted = result.userVoted;
    } catch (err) {
      console.error('Failed to vote:', err);
    }
  }

  async function handlePostComment(e: SubmitEvent) {
    e.preventDefault();
    commentError = '';

    if (!commentText.trim()) {
      commentError = 'Please enter a comment before posting.';
      return;
    }

    if (!$currentUser) {
      commentError = 'You must be logged in to post a comment.';
      return;
    }

    const eventId = getRouteId();
    isSubmitting = true;

    try {
      const newComment = await createComment(eventId, {
        content: commentText.trim(),
        userId: $currentUser.id
      });

      comments = [newComment, ...comments];
      commentText = '';
    } catch (err: any) {
      try {
        const parsed = JSON.parse(err.message);
        if (parsed?.errors) {
          commentError = Object.values(parsed.errors).flat().join(' ');
        } else {
          commentError = err.message;
        }
      } catch {
        commentError = err?.message || 'Failed to post comment.';
      }
    } finally {
      isSubmitting = false;
    }
  }

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'TBA';
    const d = new Date(dateStr);
    return isNaN(d.getTime()) ? 'TBA' : d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  // Resolves the link: uses custom ticketUrl if populated, or falls back to club route/search
  let targetUrl = $derived(() => {
    if (!event) return '#';
    const rawUrl = (event as any).ticketUrl || (event as any).ticket_url;
    if (rawUrl && rawUrl.trim() !== '') return rawUrl;
    if (event.clubId) return `/clubs/${event.clubId}`;
    return '#';
  });

  onMount(loadEventData);
</script>

{#if isLoading}
  <div class="text-center py-12 text-gray-500 text-sm">Loading event details...</div>
{:else if errorMsg || !event}
  <div class="p-4 bg-red-100 border border-red-400 text-red-700 rounded-md text-sm">
    {errorMsg || 'Event not found.'}
  </div>
{:else}
  <div class="space-y-8 max-w-3xl mx-auto">
    <div class="bg-white border border-gray-200 rounded-lg p-6 shadow-sm space-y-4">
      <div class="flex justify-between items-start">
        <div>
          <span class="text-xs font-semibold text-indigo-600 uppercase tracking-wide">
            {event.clubName || 'Campus Event'}
          </span>
          <h1 class="text-3xl font-bold text-gray-900 mt-1">{event.title}</h1>
        </div>
        <button
          onclick={handleVote}
          class="flex items-center space-x-1.5 px-3 py-1.5 border border-indigo-200 rounded-md text-sm font-medium transition {event.userVoted ? 'bg-indigo-600 text-white' : 'text-indigo-600 hover:bg-indigo-50'}"
        >
          <span>▲ Upvote</span>
          <span class="{event.userVoted ? 'bg-indigo-700 text-white' : 'bg-indigo-100'} px-2 py-0.5 rounded text-xs">{event.upvotes || 0}</span>
        </button>
      </div>

      <p class="text-gray-700 whitespace-pre-line">{event.description}</p>

      <div class="flex flex-wrap items-center justify-between gap-4 text-sm text-gray-500 pt-4 border-t border-gray-100">
        <div class="space-y-1">
          <div>📍 <strong>Location:</strong> {event.location}</div>
          <div>📅 <strong>Time:</strong> {formatDate(event.startTime || (event as any).date)}</div>
        </div>

        <a
          href={targetUrl()}
          target="_blank"
          rel="noopener noreferrer"
          class="inline-flex items-center gap-1.5 bg-indigo-600 hover:bg-indigo-700 text-white font-medium px-4 py-2 rounded-md text-sm transition shadow-sm"
        >
          <span>🔗 Open Event Link / Tickets</span>
        </a>
      </div>
    </div>

    <!-- Event Discussion Section -->
    <div class="bg-white border border-gray-200 rounded-lg p-6 shadow-sm space-y-4">
      <h2 class="text-xl font-bold text-gray-900">Event Discussion</h2>

      {#if commentError}
        <div class="p-3 bg-red-100 border border-red-400 text-red-700 rounded text-sm">
          {commentError}
        </div>
      {/if}

      {#if !$currentUser}
        <p class="text-sm text-gray-500">You must be logged in to participate in the event discussion.</p>
      {:else}
        <form onsubmit={handlePostComment} class="space-y-3">
          <textarea
            rows="3"
            bind:value={commentText}
            placeholder="Share your thoughts about this event..."
            class="w-full border border-gray-300 rounded-md p-3 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          ></textarea>
          <button
            type="submit"
            disabled={isSubmitting}
            class="px-4 py-2 bg-indigo-600 text-white font-medium text-sm rounded-md hover:bg-indigo-700 disabled:opacity-50"
          >
            {isSubmitting ? 'Posting...' : 'Post Comment'}
          </button>
        </form>
      {/if}

      <div class="space-y-3 pt-4 border-t border-gray-100">
        {#if comments.length === 0}
          <p class="text-sm text-gray-500">No comments yet. Be the first to start the conversation!</p>
        {:else}
          {#each comments as comment (comment.id)}
            <div class="bg-gray-50 p-4 rounded-md border border-gray-100 space-y-1">
              <div class="flex justify-between items-center text-xs text-gray-500">
                <span class="font-semibold text-gray-700">{comment.userName || 'Student'}</span>
                <span>{new Date(comment.createdAt).toLocaleDateString()}</span>
              </div>
              <p class="text-sm text-gray-800 whitespace-pre-line">{comment.content}</p>
            </div>
          {/each}
        {/if}
      </div>
    </div>
  </div>
{/if}