<script lang="ts">
  import type { Club } from '../../types/club';

  let { club }: { club: Club } = $props();
</script>

<div class="bg-white border border-gray-200 rounded-lg p-5 shadow-sm hover:shadow-md transition">
  <div class="flex items-start justify-between">
    <div>
      <span class="inline-block bg-indigo-50 text-indigo-700 text-xs font-semibold px-2.5 py-1 rounded-full mb-2">
        {club.category}
      </span>
      <h3 class="text-lg font-bold text-gray-900">{club.name}</h3>
    </div>
  </div>

  <p class="text-gray-600 text-sm mt-2 line-clamp-2">{club.description}</p>

  <div class="mt-4 pt-3 border-t border-gray-100 flex justify-between items-center text-xs text-gray-500">
    <div class="flex gap-3">
      <span><strong>{club.eventCount}</strong> Events</span>
      <span><strong>{club.postCount}</strong> Posts</span>
    </div>
    <a href={`/clubs/${club.id}`} class="text-indigo-600 hover:underline font-semibold">View Club &rarr;</a>
  </div>
</div>