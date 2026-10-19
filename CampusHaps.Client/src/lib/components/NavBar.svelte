<script lang="ts">
  import { goto } from '$app/navigation';
  import { currentUser, logoutUser } from '../stores/userStore';

  function handleLogout() {
    logoutUser();
    goto('/login');
  }
</script>

<header style="background-color: #3b82f6; color: white; padding: 1rem 0; width: 100%;">
  <div style="max-width: 1200px; margin: 0 auto; padding: 0 1rem; text-align: center;">
    
    <!-- Top Row: CampusHaps Brand -->
    <div style="margin-bottom: 0.75rem;">
      <a href="/" style="font-size: 1.5rem; font-weight: 700; color: white; text-decoration: none; display: inline-flex; align-items: center; gap: 0.5rem;">
        <span>🎓</span> CampusHaps
      </a>
    </div>

    <!-- Bottom Row: Nav Links & Auth Button Spaced Evenly Across the Top -->
    <nav style="display: flex; justify-content: space-around; align-items: center; width: 100%; border-top: 1px solid rgba(255,255,255,0.2); padding-top: 0.75rem;">
      <a href="/" style="color: white; text-decoration: none; font-weight: 600; font-size: 0.95rem;">Events</a>
      <a href="/clubs" style="color: white; text-decoration: none; font-weight: 600; font-size: 0.95rem;">Clubs</a>
      <a href="/calendar" style="color: white; text-decoration: none; font-weight: 600; font-size: 0.95rem;">Calendar</a>

      <div>
        {#if $currentUser}
          <div style="display: flex; align-items: center; gap: 0.5rem;">
            <span style="font-size: 0.875rem;">
              <strong>{$currentUser.name}</strong> ({$currentUser.role})
            </span>
            <button
              onclick={handleLogout}
              style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.35rem 0.75rem; border-radius: 0.375rem; font-weight: 600; font-size: 0.85rem; cursor: pointer;"
            >
              Sign Out
            </button>
          </div>
        {:else}
          <a
            href="/login"
            style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.35rem 0.85rem; border-radius: 0.375rem; font-weight: 600; font-size: 0.85rem; text-decoration: none; display: inline-block;"
          >
            Sign In
          </a>
        {/if}
      </div>
    </nav>

  </div>
</header>