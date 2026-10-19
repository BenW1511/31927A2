<script lang="ts">
  import { onMount } from 'svelte';
  import { getClubs } from '../../lib/api/clubs';
  import type { Club } from '../../lib/types/club';
  import ClubCard from '../../lib/components/clubs/ClubCard.svelte';
  import CreateClubModal from '../../lib/components/clubs/CreateClubModal.svelte';

  let clubs = $state<Club[]>([]);
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

  async function loadClubs() {
    isLoading = true;
    errorMessage = '';
    try {
      clubs = await getClubs();
    } catch (err: any) {
      errorMessage = 'Failed to load clubs. Please ensure the backend API is running.';
      console.error(err);
    } finally {
      isLoading = false;
    }
  }

  onMount(loadClubs);

  // Filter clubs by search query and selected category
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

<div class="space-y-6">
  <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Campus Clubs</h1>
      <p class="text-gray-600 text-sm">Explore active university societies and student groups.</p>
    </div>
    <button
      onclick={() => (isModalOpen = true)}
      class="bg-indigo-600 hover:bg-indigo-700 text-white font-medium px-4 py-2 rounded-md text-sm shadow-sm transition"
    >
      + Create New Club
    </button>
  </div>

  <!-- Filter & Search Controls -->
  <div class="flex flex-col sm:flex-row gap-4 items-stretch sm:items-center">
    <div class="flex-grow">
      <input
        type="text"
        bind:value={searchQuery}
        placeholder="Search clubs by name or keywords..."
        class="w-full border border-gray-300 rounded-md px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
      />
    </div>

    <div class="w-full sm:w-56">
      <select
        bind:value={selectedCategory}
        class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
      >
        {#each categories as category}
          <option value={category}>{category === 'All' ? 'All Categories' : category}</option>
        {/each}
      </select>
    </div>
  </div>

  {#if errorMessage}
    <div class="p-4 bg-red-100 border border-red-400 text-red-700 rounded-md text-sm">
      {errorMessage}
    </div>
  {/if}

  {#if isLoading}
    <div class="text-center py-12 text-gray-500 text-sm">Loading clubs...</div>
  {:else if filteredClubs.length === 0}
    <div class="bg-white border border-gray-200 rounded-lg p-6 text-center text-gray-500 text-sm">
      No clubs found matching your search and category selection.
    </div>
  {:else}
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {#each filteredClubs as club (club.id)}
        <ClubCard {club} />
      {/each}
    </div>
  {/if}

  <CreateClubModal
    isOpen={isModalOpen}
    onclose={() => (isModalOpen = false)}
    onclubCreated={loadClubs}
  />
</div>