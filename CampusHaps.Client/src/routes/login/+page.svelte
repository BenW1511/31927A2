<script lang="ts">
  import { goto } from '$app/navigation';
  import { currentUser } from '../../lib/stores/userStore';

  let email = $state('');
  let password = $state('');
  let errorMessage = $state('');
  let isLoading = $state(false);

  async function handleLogin(e: SubmitEvent) {
    e.preventDefault();
    errorMessage = '';

    if (!email.trim() || !password.trim()) {
      errorMessage = 'Please enter both email and password.';
      return;
    }

    isLoading = true;

    try {
      // Directly update the currentUser writable store
      currentUser.set({
        id: 1,
        name: email.split('@')[0] || 'Test Student',
        email: email,
        role: 'student'
      });

      goto('/');
    } catch (err: any) {
      errorMessage = err?.message || 'Login failed.';
    } finally {
      isLoading = false;
    }
  }
</script>

<div style="max-width: 420px; margin: 3rem auto; padding: 0 1rem; text-align: center;">
  <div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 2px 4px rgba(0,0,0,0.05);">
    
    <div style="margin-bottom: 1.5rem;">
      <h1 style="font-size: 1.75rem; font-weight: 800; color: #111827; margin-bottom: 0.25rem;">Sign In</h1>
      <p style="color: #6b7280; font-size: 0.875rem;">Access your CampusHaps student account</p>
    </div>

    {#if errorMessage}
      <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem; margin-bottom: 1.25rem; text-align: left;">
        {errorMessage}
      </div>
    {/if}

    <form onsubmit={handleLogin} style="display: flex; flex-direction: column; gap: 1rem; text-align: left;">
      <div>
        <label for="email" style="display: block; font-size: 0.85rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          Email Address
        </label>
        <input
          id="email"
          type="email"
          bind:value={email}
          placeholder="student1@example.com"
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
        />
      </div>

      <div>
        <label for="password" style="display: block; font-size: 0.85rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          Password
        </label>
        <input
          id="password"
          type="password"
          bind:value={password}
          placeholder="••••••••"
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
        />
      </div>

      <button
        type="submit"
        disabled={isLoading}
        style="width: 100%; background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.6rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer; margin-top: 0.5rem;"
      >
        {isLoading ? 'Signing In...' : 'Sign In'}
      </button>
    </form>

  </div>
</div>