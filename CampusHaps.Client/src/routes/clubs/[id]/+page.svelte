<script lang="ts">
  import { onMount } from 'svelte';
  import { getClubById, getClubPosts, createClubPost } from '../../../lib/api/clubs';
  import { currentUser } from '../../../lib/stores/userStore';
  import type { Club } from '../../../lib/types/club';

  interface Props {
    data?: {
      id?: string;
      [key: string]: any;
    };
  }

  let { data }: Props = $props();

  let club = $state<Club | null>(null);
  let posts = $state<any[]>([]);
  let isLoading = $state(true);
  let errorMsg = $state('');

  let postTitle = $state('');
  let postContent = $state('');
  let isSubmitting = $state(false);
  let postError = $state('');

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

  async function loadClubData() {
    const clubId = getRouteId();

    if (!clubId) {
      errorMsg = 'Invalid Club ID.';
      isLoading = false;
      return;
    }

    isLoading = true;
    errorMsg = '';

    try {
      const [fetchedClub, fetchedPosts] = await Promise.all([
        getClubById(clubId),
        getClubPosts(clubId)
      ]);
      club = fetchedClub;
      posts = fetchedPosts;
    } catch (err: any) {
      errorMsg = 'Failed to load club details. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  async function handlePublishPost(e: SubmitEvent) {
    e.preventDefault();
    postError = '';

    if (!postTitle.trim() || !postContent.trim()) {
      postError = 'Please fill in both title and content for the post.';
      return;
    }

    if (!$currentUser) {
      postError = 'You must be logged in to create a post.';
      return;
    }

    const clubId = getRouteId();
    isSubmitting = true;

    try {
      const newPost = await createClubPost(clubId, {
        title: postTitle.trim(),
        content: postContent.trim(),
        authorId: $currentUser.id
      });

      posts = [newPost, ...posts];
      postTitle = '';
      postContent = '';
    } catch (err: any) {
      postError = err?.message || 'Failed to publish post.';
    } finally {
      isSubmitting = false;
    }
  }

  onMount(loadClubData);
</script>

<div style="max-width: 1100px; margin: 2rem auto; padding: 0 1.5rem; width: 100%; box-sizing: border-box;">
  {#if isLoading}
    <div style="text-align: center; padding: 3rem 0; color: #6b7280; font-size: 0.875rem;">Loading club details...</div>
  {:else if errorMsg || !club}
    <div style="padding: 1rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem;">
      {errorMsg || 'Club not found.'}
    </div>
  {:else}
    <div style="display: flex; flex-direction: column; gap: 2rem; width: 100%;">
      
      <!-- Main Club Information Card -->
      <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); text-align: left; width: 100%; box-sizing: border-box;">
        <span style="font-size: 0.85rem; font-weight: 700; color: #4f46e5; text-transform: uppercase;">
          {club.category}
        </span>
        <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin: 0.25rem 0 0.5rem 0;">{club.name}</h1>
        
        <p style="color: #374151; font-size: 1rem; line-height: 1.6; margin: 0 0 1.25rem 0; white-space: pre-line;">
          {club.description}
        </p>

        <div style="padding-top: 1rem; border-top: 1px solid #e5e7eb; font-size: 0.9rem; color: #4b5563;">
          <strong>Contact Email:</strong> <a href="mailto:{club.contactEmail}" style="color: #4f46e5; text-decoration: underline;">{club.contactEmail}</a>
        </div>
      </div>

      <!-- Announcements & Discussion Section -->
      <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); text-align: left; width: 100%; box-sizing: border-box;">
        <h2 style="font-size: 1.5rem; font-weight: 800; color: #111827; margin: 0 0 1.25rem 0;">Create Club Post / Announcement</h2>

        {#if postError}
          <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem; margin-bottom: 1.25rem;">
            {postError}
          </div>
        {/if}

        {#if !$currentUser}
          <div style="padding: 1rem; background: #f9fafb; border: 1px solid #e5e7eb; border-radius: 0.375rem; font-size: 0.875rem; color: #6b7280; margin-bottom: 1.5rem;">
            You must be logged in to post an announcement for this club.
          </div>
        {:else}
          <form onsubmit={handlePublishPost} style="display: flex; flex-direction: column; gap: 1rem; margin-bottom: 2rem;">
            <div>
              <label for="postTitle" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
                Post Title
              </label>
              <input
                id="postTitle"
                type="text"
                bind:value={postTitle}
                placeholder="e.g. Semester Welcome Event"
                style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.75rem; font-size: 0.9rem; outline: none; box-sizing: border-box;"
              />
            </div>

            <div>
              <label for="postContent" style="display: block; font-size: 0.875rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
                Content
              </label>
              <textarea
                id="postContent"
                rows="4"
                bind:value={postContent}
                placeholder="Write announcement details..."
                style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.75rem; font-size: 0.9rem; outline: none; box-sizing: border-box; resize: vertical;"
              ></textarea>
            </div>

            <div>
              <button
                type="submit"
                disabled={isSubmitting}
                style="background-color: #ffffff; color: #000000; border: 1px solid #000000; padding: 0.5rem 1.25rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer;"
              >
                {isSubmitting ? 'Publishing...' : 'Publish Post'}
              </button>
            </div>
          </form>
        {/if}

        <h3 style="font-size: 1.25rem; font-weight: 800; color: #111827; margin: 0 0 1rem 0; padding-top: 1rem; border-top: 1px solid #f3f4f6;">
          Club Posts
        </h3>

        <!-- Individual Post Cards -->
        <div style="display: flex; flex-direction: column; gap: 1rem;">
          {#if posts.length === 0}
            <p style="font-size: 0.875rem; color: #6b7280; margin: 0;">No posts published for this club yet.</p>
          {:else}
            {#each posts as post (post.id)}
              <div style="background: #ffffff; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 1.25rem; box-shadow: 0 1px 2px rgba(0,0,0,0.02);">
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 0.8rem; color: #6b7280; margin-bottom: 0.5rem;">
                  <strong style="color: #111827; font-size: 0.875rem;">{post.authorName || 'Club Exec'}</strong>
                  <span>{new Date(post.createdAt || Date.now()).toLocaleDateString()}</span>
                </div>
                <h4 style="font-size: 1.1rem; font-weight: 700; color: #111827; margin: 0 0 0.5rem 0;">{post.title}</h4>
                <p style="font-size: 0.9rem; color: #374151; margin: 0; line-height: 1.5; white-space: pre-line;">
                  {post.content}
                </p>
              </div>
            {/each}
          {/if}
        </div>

      </div>

    </div>
  {/if}
</div>