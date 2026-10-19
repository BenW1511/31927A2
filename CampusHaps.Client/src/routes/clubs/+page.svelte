<script lang="ts">
  import { onMount } from 'svelte';
  import { getClubs } from '../../lib/api/clubs';
  import { getEvents } from '../../lib/api/events';
  import type { Club } from '../../lib/types/club';
  import type { Event as CampusEvent } from '../../lib/types/event';
  import ClubCard from '../../lib/components/clubs/ClubCard.svelte';
  import CreateClubModal from '../../lib/components/clubs/CreateClubModal.svelte';

  let clubs = $state<Club[]>([]);
  let events = $state<CampusEvent[]>([]);
  let searchQuery = $state('');
  let selectedCategory = $state('All');
  let isLoading = $state(true);
  let errorMessage = $state('');
  let isModalOpen = $state(false);

  const categories = [
    'All',
    'Technology',
    'Academic',
    'Arts & Culture',
    'Sports & Fitness',
    'Social & Special Interest',
    'Social'
  ];

  async function loadData() {
    isLoading = true;
    errorMessage = '';
    try {
      const [fetchedClubs, fetchedEvents] = await Promise.all([
        getClubs(),
        getEvents()
      ]);
      clubs = fetchedClubs;
      events = fetchedEvents;
    } catch (err: any) {
      errorMessage = 'Failed to load clubs and events. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  onMount(loadData);

  let filteredClubs = $derived(
    clubs.filter((club) => {
      const q = searchQuery.toLowerCase();
      const matchesSearch =
        club.name.toLowerCase().includes(q) ||
        club.category.toLowerCase().includes(q) ||
        club.description.toLowerCase().includes(q);

      const matchesCategory =
        selectedCategory === 'All' ||
        club.category.toLowerCase() === selectedCategory.toLowerCase();

      return matchesSearch && matchesCategory;
    })
  );
</script>

<div style="max-width: 900px; margin: 0 auto; padding: 1.5rem; text-align: center;">
  
  <div style="margin-bottom: 1.5rem;">
    <h1 style="font-size: 2.25rem; font-weight: 800; color: #111827; margin-bottom: 0.25rem;">Campus Clubs</h1>
    <p style="color: #4b5563; font-size: 0.95rem;">Explore active university societies and student groups.</p>
    
    <div style="margin-top: 1rem;">
      <button
        onclick={() => (isModalOpen = true)}
        style="background-color: #ffffff; color: #000000; border: 1px solid #d1d5db; padding: 0.5rem 1rem; border-radius: 0.375rem; font-weight: 600; font-size: 0.875rem; cursor: pointer;"
      >
        + Create New Club
      </button>
    </div>
  </div>

  <!-- Search & Category Filter -->
  <div style="display: flex; flex-wrap: wrap; justify-content: center; gap: 0.75rem; margin-bottom: 2rem;">
    <input
      type="text"
      bind:value={searchQuery}
      placeholder="Search clubs by name or keywords..."
      style="width: 100%; max-width: 400px; border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; outline: none;"
    />

    <select
      bind:value={selectedCategory}
      style="border: 1px solid #d1d5db; border-radius: 0.375rem; padding: 0.5rem 0.75rem; font-size: 0.875rem; background: #ffffff; outline: none;"
    >
      {#each categories as category}
        <option value={category}>{category === 'All' ? 'All Categories' : category}</option>
      {/each}
    </select>
  </div>

  {#if errorMessage}
    <div style="padding: 0.75rem; background-color: #fee2e2; border: 1px solid #fca5a5; color: #b91c1c; border-radius: 0.375rem; font-size: 0.875rem; margin-bottom: 1rem;">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div style="padding: 2rem 0; color: #6b7280; font-size: 0.875rem;">Loading clubs...</div>
  {:else if filteredClubs.length === 0}
    <div style="background: #ffffff; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1.5rem; color: #6b7280; font-size: 0.875rem;">
      No clubs found matching your search and category selection.
    </div>
  {:else}
    <div style="display: flex; flex-direction: column; gap: 1.25rem;">
      {#each filteredClubs as club (club.id)}
        <ClubCard {club} {events} />
      {/each}
    </div>
  {/if}

  <CreateClubModal
    isOpen={isModalOpen}
    onclose={() => (isModalOpen = false)}
    onclubCreated={loadData}
  />
</div>