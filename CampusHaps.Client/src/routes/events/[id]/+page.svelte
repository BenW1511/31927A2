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
      commentError = err?.message || 'Failed to post comment.';
    } finally {
      isSubmitting = false;
    }
  }

  function formatDate(dateStr?: string) {
    if (!dateStr) return 'TBA';
    const d = new Date(dateStr);
    return isNaN(d.getTime()) ? 'TBA' : d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  let targetUrl = $derived(() => {
    if (!event) return '#';
    const rawUrl = (event as any).ticketUrl || (event as any).ticket_url;
    if (rawUrl && rawUrl.trim() !== '') return rawUrl;
    if (event.clubId) return `/clubs/${event.clubId}`;
    return '#';
  });

  onMount(loadEventData);
</script>

<!-- Centered and Expanded Layout Container -->
<div style="max-width: 1100px; margin: 2rem auto; padding: 0 1.5rem; width: 100%; box-sizing: border-box;">
  {#if isLoading}
    <div style="text-align: center; padding: 3rem 0; color: #6b7280; font-size: 0.875rem;">Loading event details...</div>
  {:else if errorMsg || !event}
    <div style="padding: 1rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem;">
      {errorMsg || 'Event not found.'}
    </div>
  {:else}
    <div style="display: flex; flex-direction: column; gap: 2rem; width: 100%;">
      
      <!-- Event Header Card -->
      <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); text-align: left; width: 100%; box-sizing: border-box;">
        <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
          <div>
            <span style="font-size: 0.85rem; font-weight: 700; color: #4f46e5; text-transform: uppercase; tracking-wide;">
              {event.clubName || 'Campus Event'}
            </span>
            <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin: 0.25rem 0 0 0;">{event.title}</h1>
          </div>
          <button
            onclick={handleVote}
            style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.4rem 0.85rem; border-radius: 0.375rem; font-size: 0.875rem; font-weight: 600; cursor: pointer;"
          >
            ▲ Upvote {event.upvotes || 0}
          </button>
        </div>

        <p style="color: #374151; font-size: 1rem; line-height: 1.6; margin: 0 0 1.5rem 0; white-space: pre-line;">
          {event.description}
        </p>

        <div style="padding-top: 1.25rem; border-top: 1px solid #e5e7eb; display: flex; flex-wrap: wrap; justify-content: space-between; align-items: center; gap: 1rem; font-size: 0.9rem; color: #4b5563;">
          <div style="display: flex; gap: 2rem;">
            <div>📍 <strong>Location:</strong> {event.location}</div>
            <div>📅 <strong>Time:</strong> {formatDate(event.startTime || (event as any).date)}</div>
          </div>

          <a
            href={targetUrl()}
            target="_blank"
            rel="noopener noreferrer"
            style="background-color: #ffffff; color: #000000; border: 1px solid #000000; padding: 0.5rem 1rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; text-decoration: none; display: inline-flex; align-items: center; gap: 0.5rem;"
          >
            <span>🔗 Open Event Link / Tickets</span>
          </a>
        </div>
      </div>

      <!-- Event Discussion Section -->
      <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); text-align: left; width: 100%; box-sizing: border-box;">
        <h2 style="font-size: 1.5rem; font-weight: 800; color: #111827; margin: 0 0 1.25rem 0;">Event Discussion</h2>

        {#if commentError}
          <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem; margin-bottom: 1.25rem;">
            {commentError}
          </div>
        {/if}

        {#if !$currentUser}
          <div style="padding: 1rem; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 0.375rem; font-size: 0.875rem; color: #6b7280; margin-bottom: 1.5rem;">
            You must be logged in to participate in the event discussion.
          </div>
        {:else}
          <form onsubmit={handlePostComment} style="display: flex; flex-direction: column; gap: 0.75rem; margin-bottom: 2rem;">
            <textarea
              rows="3"
              bind:value={commentText}
              placeholder="Share your thoughts about this event..."
              style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.85rem; font-size: 0.9rem; outline: none; box-sizing: border-box; resize: vertical;"
            ></textarea>
            <div>
              <button
                type="submit"
                disabled={isSubmitting}
                style="background-color: #ffffff; color: #000000; border: 1px solid #000000; padding: 0.5rem 1.25rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer;"
              >
                {isSubmitting ? 'Posting...' : 'Post Comment'}
              </button>
            </div>
          </form>
        {/if}

        <!-- Comments Feed: Each comment placed inside an individual styled card box -->
        <div style="display: flex; flex-direction: column; gap: 1rem;">
          {#if comments.length === 0}
            <p style="font-size: 0.875rem; color: #6b7280; margin: 0;">No comments yet. Be the first to start the conversation!</p>
          {:else}
            {#each comments as comment (comment.id)}
              <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 1rem 1.25rem; box-shadow: 0 1px 2px rgba(0,0,0,0.02);">
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 0.8rem; color: #6b7280; margin-bottom: 0.5rem;">
                  <strong style="color: #111827; font-size: 0.875rem;">{comment.userName || 'Student'}</strong>
                  <span>{new Date(comment.createdAt).toLocaleDateString()}</span>
                </div>
                <p style="font-size: 0.9rem; color: #374151; margin: 0; line-height: 1.5; white-space: pre-line;">
                  {comment.content}
                </p>
              </div>
            {/each}
          {/if}
        </div>

      </div>

    </div>
  {/if}
</div>