<script lang="ts">
  import { goto } from '$app/navigation';
  import { currentUser, logoutUser } from '../stores/userStore';

  function handleLogout() {
    logoutUser();
    goto('/login');
  }
</script>

<header class="bg-indigo-600 text-white shadow-md">
  <div class="container mx-auto px-4 max-w-6xl flex justify-between items-center h-16">
    <div class="flex items-center space-x-6">
      <a href="/" class="text-xl font-bold tracking-tight hover:text-indigo-100 flex items-center gap-2">
        <span>🎓</span> CampusHaps
      </a>
      <nav class="flex space-x-4 text-sm font-medium">
        <a href="/" class="hover:text-indigo-200 transition">Events</a>
        <a href="/clubs" class="hover:text-indigo-200 transition">Clubs</a>
        <a href="/calendar" class="hover:text-indigo-200 transition">Calendar</a>
      </nav>
    </div>

    <div class="flex items-center space-x-4 text-sm">
      {#if $currentUser}
        <span class="text-indigo-200">
          <strong class="text-white">{$currentUser.name}</strong> ({$currentUser.role})
        </span>
        <button
          onclick={handleLogout}
          class="bg-indigo-700 hover:bg-indigo-800 text-white px-3 py-1.5 rounded-md font-medium text-xs border border-indigo-500 transition"
        >
          Sign Out
        </button>
      {:else}
        <a
          href="/login"
          class="bg-white text-indigo-600 hover:bg-indigo-50 px-3 py-1.5 rounded-md font-medium text-xs transition"
        >
          Sign In
        </a>
      {/if}
    </div>
  </div>
</header>