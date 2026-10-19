<script lang="ts">
  import { onMount } from 'svelte';
  import { getClubById, getClubPosts, createClubPost } from '../../../lib/api/clubs';
  import { currentUser } from '../../../lib/stores/userStore';
  import type { Club, ClubPost } from '../../../lib/types/club';

  interface Props {
    data?: {
      id?: string;
      [key: string]: any;
    };
  }

  let { data }: Props = $props();

  let club = $state<Club | null>(null);
  let posts = $state<ClubPost[]>([]);
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

  let clubId = $derived(getRouteId());

  async function loadClubData() {
    const id = getRouteId();

    if (!id) {
      errorMsg = 'Invalid Club ID.';
      isLoading = false;
      return;
    }

    isLoading = true;
    errorMsg = '';

    try {
      const [fetchedClub, fetchedPosts] = await Promise.all([
        getClubById(id),
        getClubPosts(id)
      ]);
      club = fetchedClub;
      posts = fetchedPosts;
    } catch (err: any) {
      errorMsg = 'Failed to load club information. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  async function handleCreatePost(e: SubmitEvent) {
    e.preventDefault();
    postError = '';

    if (!postTitle.trim() || !postContent.trim()) {
      postError = 'Please provide both a title and content for your post.';
      return;
    }

    if (!$currentUser) {
      postError = 'You must be logged in to create a post.';
      return;
    }

    const id = getRouteId();
    isSubmitting = true;

    try {
      const newPost = await createClubPost(id, {
        title: postTitle.trim(),
        content: postContent.trim(),
        authorId: $currentUser.id
      });

      posts = [newPost, ...posts];
      postTitle = '';
      postContent = '';
    } catch (err: any) {
      try {
        const parsed = JSON.parse(err.message);
        if (parsed?.errors) {
          postError = Object.values(parsed.errors).flat().join(' ');
        } else {
          postError = err.message;
        }
      } catch {
        postError = err?.message || 'Failed to submit post.';
      }
    } finally {
      isSubmitting = false;
    }
  }

  onMount(loadClubData);
</script>

{#if isLoading}
  <div class="text-center py-12 text-gray-500 text-sm">Loading club details...</div>
{:else if errorMsg || !club}
  <div class="p-4 bg-red-100 border border-red-400 text-red-700 rounded-md text-sm">
    {errorMsg || 'Club not found.'}
  </div>
{:else}
  <div class="space-y-8">
    <div class="bg-white border border-gray-200 rounded-lg p-6 shadow-sm">
      <span class="inline-block px-3 py-1 bg-indigo-50 text-indigo-700 rounded-full text-xs font-semibold uppercase tracking-wide mb-2">
        {club.category}
      </span>
      <h1 class="text-3xl font-bold text-gray-900 mb-2">{club.name}</h1>
      <p class="text-gray-700 mb-4 whitespace-pre-line">{club.description}</p>
      <div class="text-sm text-gray-500">
        Contact: <a href="mailto:{club.contactEmail}" class="text-indigo-600 hover:underline">{club.contactEmail}</a>
      </div>
    </div>

    <!-- Create Post Section -->
    <div class="bg-white border border-gray-200 rounded-lg p-6 shadow-sm space-y-4">
      <h2 class="text-xl font-bold text-gray-900">Create Club Post / Announcement</h2>

      {#if !$currentUser}
        <p class="text-sm text-gray-500">You must be logged in to post announcements for this club.</p>
      {:else}
        {#if postError}
          <div class="p-3 bg-red-100 border border-red-400 text-red-700 rounded text-sm">
            {postError}
          </div>
        {/if}

        <form onsubmit={handleCreatePost} class="space-y-4">
          <div>
            <label for="post-title" class="block text-sm font-medium text-gray-700 mb-1">Post Title</label>
            <input
              id="post-title"
              type="text"
              bind:value={postTitle}
              placeholder="e.g. Semester Welcome Event Announcement"
              class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          <div>
            <label for="post-content" class="block text-sm font-medium text-gray-700 mb-1">Content</label>
            <textarea
              id="post-content"
              rows="4"
              bind:value={postContent}
              placeholder="Write announcement details..."
              class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            ></textarea>
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700 disabled:opacity-50"
          >
            {isSubmitting ? 'Posting...' : 'Publish Post'}
          </button>
        </form>
      {/if}
    </div>

    <!-- Club Posts List -->
    <div class="space-y-4">
      <h2 class="text-xl font-bold text-gray-900">Club Posts</h2>

      {#if posts.length === 0}
        <div class="bg-white border border-gray-200 rounded-lg p-6 text-center text-gray-500 text-sm">
          No posts published for this club yet.
        </div>
      {:else}
        <div class="space-y-4">
          {#each posts as post (post.id)}
            <div class="bg-white border border-gray-200 rounded-lg p-5 shadow-sm space-y-2">
              <h3 class="text-lg font-bold text-gray-900">{post.title}</h3>
              <p class="text-gray-700 text-sm whitespace-pre-line">{post.content}</p>
              <div class="text-xs text-gray-400 pt-2 border-t border-gray-100 flex justify-between">
                <span>By {post.authorName || club.name}</span>
                <span>{new Date(post.createdAt).toLocaleDateString()}</span>
              </div>
            </div>
          {/each}
        </div>
      {/if}
    </div>
  </div>
{/if}