<script lang="ts">
  import { createEventDispatcher } from 'svelte';

  let { title = '', isOpen = false, children } = $props();

  const dispatch = createEventDispatcher();

  function close() {
    dispatch('close');
  }
</script>

{#if isOpen}
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
    <div class="bg-white rounded-lg shadow-xl w-full max-w-lg overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200 flex justify-between items-center">
        <h3 class="text-lg font-bold text-gray-800">{title}</h3>
        <button onclick={close} class="text-gray-400 hover:text-gray-600 font-bold text-xl">&times;</button>
      </div>
      <div class="p-6">
        {#if children}
          {@render children()}
        {/if}
      </div>
    </div>
  </div>
{/if}