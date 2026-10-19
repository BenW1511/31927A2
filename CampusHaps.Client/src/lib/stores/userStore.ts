import { writable } from 'svelte/store';
import type { User } from '../types/user';

// Initial state starts as null (or user details if authenticated)
export const currentUser = writable<User | null>(null);

/**
 * Clears the current user session state.
 */
export function logoutUser(): void {
  currentUser.set(null);
}