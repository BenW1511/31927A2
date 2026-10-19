<script lang="ts">
  import '../app.css';
  import NavBar from '../lib/components/NavBar.svelte';
  let { children } = $props();
</script>

<div class="min-h-screen flex flex-col bg-gray-50 text-gray-900">
  <NavBar />
  <main class="flex-grow container mx-auto px-4 py-6 max-w-6xl">
    {@render children()}
  </main>
  <footer class="bg-white border-t border-gray-200 py-6 mt-12">
    <div class="container mx-auto px-4 text-center text-sm text-gray-500">
      &copy; {new Date().getFullYear()} CampusHaps &mdash; On-Campus Events & Student Clubs Platform
    </div>
  </footer>
</div>