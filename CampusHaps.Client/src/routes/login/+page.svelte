<script lang="ts">
  import { goto } from '$app/navigation';
  import { currentUser } from '../../lib/stores/userStore';

  let mode = $state<'login' | 'register'>('login');

  // Form Fields
  let name = $state('');
  let email = $state('');
  let password = $state('');

  // UI State
  let errorMessage = $state('');
  let isLoading = $state(false);

  function switchMode(newMode: 'login' | 'register') {
    mode = newMode;
    errorMessage = '';
  }

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    errorMessage = '';

    if (mode === 'register' && !name.trim()) {
      errorMessage = 'Please enter your name.';
      return;
    }

    if (!email.trim() || !password.trim()) {
      errorMessage = 'Please fill in all required fields.';
      return;
    }

    if (mode === 'register' && !email.trim().toLowerCase().endsWith('.edu.au')) {
      errorMessage = 'Please use your university email (.edu.au).';
      return;
    }

    if (mode === 'register' && password.length < 8) {
      errorMessage = 'Password must be at least 8 characters long.';
      return;
    }

    isLoading = true;

    try {
      const endpoint = mode === 'login' ? '/api/users/login' : '/api/users/register';
      const body = mode === 'login' 
        ? { email: email.trim(), password }
        : { name: name.trim(), email: email.trim(), password };

      const response = await fetch(endpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
      });

      if (response.ok) {
        const user = await response.json();
        currentUser.set({
          id: user.id,
          name: user.name,
          email: user.email,
          role: 'student'
        });
        await goto('/');
      } else {
        const responseText = await response.text();
        try {
          const jsonErr = JSON.parse(responseText);
          if (jsonErr.errors) {
            // ASP.NET Core ModelState validation dictionary
            const firstKey = Object.keys(jsonErr.errors)[0];
            errorMessage = jsonErr.errors[firstKey][0];
          } else if (jsonErr.title) {
            errorMessage = jsonErr.title;
          } else {
            errorMessage = responseText;
          }
        } catch {
          errorMessage = responseText || `Request failed with status ${response.status}.`;
        }
      }
    } catch (err: any) {
      errorMessage = 'Unable to connect to the backend server. Make sure your C# API is running on port 5188.';
    } finally {
      isLoading = false;
    }
  }
</script>

<div style="max-width: 420px; margin: 3rem auto; padding: 0 1rem; text-align: center;">
  <div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 2rem; box-shadow: 0 2px 4px rgba(0,0,0,0.05);">
    
    <!-- Mode Switcher Tabs -->
    <div style="display: flex; border-bottom: 1px solid #e5e7eb; margin-bottom: 1.5rem;">
      <button
        type="button"
        onclick={() => switchMode('login')}
        style="flex: 1; padding: 0.5rem; border: none; background: none; font-weight: 700; font-size: 0.9rem; cursor: pointer; border-bottom: 2px solid {mode === 'login' ? '#111827' : 'transparent'}; color: {mode === 'login' ? '#111827' : '#6b7280'};"
      >
        Sign In
      </button>
      <button
        type="button"
        onclick={() => switchMode('register')}
        style="flex: 1; padding: 0.5rem; border: none; background: none; font-weight: 700; font-size: 0.9rem; cursor: pointer; border-bottom: 2px solid {mode === 'register' ? '#111827' : 'transparent'}; color: {mode === 'register' ? '#111827' : '#6b7280'};"
      >
        Register
      </button>
    </div>

    <div style="margin-bottom: 1.5rem;">
      <h1 style="font-size: 1.75rem; font-weight: 800; color: #111827; margin-bottom: 0.25rem;">
        {mode === 'login' ? 'Sign In' : 'Create Account'}
      </h1>
      <p style="color: #6b7280; font-size: 0.875rem;">
        {mode === 'login' ? 'Access your CampusHaps student account' : 'Join CampusHaps with your university email'}
      </p>
    </div>

    {#if errorMessage}
      <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.85rem; margin-bottom: 1.25rem; text-align: left;">
        {errorMessage}
      </div>
    {/if}

    <form onsubmit={handleSubmit} style="display: flex; flex-direction: column; gap: 1rem; text-align: left;">
      {#if mode === 'register'}
        <div>
          <label for="name" style="display: block; font-size: 0.85rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
            Full Name
          </label>
          <input
            id="name"
            type="text"
            bind:value={name}
            placeholder="Jane Doe"
            disabled={isLoading}
            maxlength="100"
            style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
          />
        </div>
      {/if}

      <div>
        <label for="email" style="display: block; font-size: 0.85rem; font-weight: 600; color: #374151; margin-bottom: 0.25rem;">
          University Email (.edu.au)
        </label>
        <input
          id="email"
          type="email"
          bind:value={email}
          placeholder="student@university.edu.au"
          disabled={isLoading}
          maxlength="255"
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
          disabled={isLoading}
          style="width: 100%; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none; box-sizing: border-box;"
        />
      </div>

      <button
        type="submit"
        disabled={isLoading}
        style="width: 100%; background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.6rem; border-radius: 0.375rem; font-weight: 700; font-size: 0.875rem; cursor: pointer; margin-top: 0.5rem;"
      >
        {#if isLoading}
          {mode === 'login' ? 'Signing In...' : 'Registering...'}
        {:else}
          {mode === 'login' ? 'Sign In' : 'Register Account'}
        {/if}
      </button>
    </form>

    <div style="margin-top: 1.5rem; font-size: 0.85rem; color: #6b7280;">
      {#if mode === 'login'}
        Don't have an account? 
        <button type="button" onclick={() => switchMode('register')} style="background: none; border: none; color: #2563eb; font-weight: 600; cursor: pointer; padding: 0; text-decoration: underline;">
          Register
        </button>
      {:else}
        Already have an account? 
        <button type="button" onclick={() => switchMode('login')} style="background: none; border: none; color: #2563eb; font-weight: 600; cursor: pointer; padding: 0; text-decoration: underline;">
          Sign In
        </button>
      {/if}
    </div>

  </div>
</div>