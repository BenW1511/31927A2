import { writable } from 'svelte/store';

export interface Notification {
  id: string;
  message: string;
  type: 'success' | 'error' | 'info';
}

export const notifications = writable<Notification[]>([]);

export function addNotif(message: string, type: 'success' | 'error' | 'info' = 'info') {
  const id = crypto.randomUUID();
  notifications.update((all) => [...all, { id, message, type }]);

  setTimeout(() => {
    removeNotif(id);
  }, 3000);
}

export function removeNotif(id: string) {
  notifications.update((all) => all.filter((n) => n.id !== id));
}