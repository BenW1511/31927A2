<script lang="ts">
  import type { ClubPost } from '../../types/club';

  let { post }: { post: ClubPost } = $props();
</script>

<article class="bg-white border border-gray-200 rounded-lg p-5 shadow-sm space-y-2">
  <div class="flex justify-between items-center text-xs text-gray-500">
    <span class="font-semibold text-indigo-600">{post.authorName}</span>
    <time>{new Date(post.createdAt).toLocaleDateString()} {new Date(post.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</time>
  </div>

  <h3 class="text-base font-bold text-gray-900">{post.title}</h3>
  <p class="text-gray-700 text-sm whitespace-pre-line">{post.content}</p>
</article>