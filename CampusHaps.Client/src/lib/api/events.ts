import { apiFetch } from './client';
import type { Event } from '../types/event';

export async function getEvents(): Promise<Event[]> {
  const events = await apiFetch<any[]>('/api/events');
  return events.map((ev) => ({
    ...ev,
    upvotes: ev.voteCount ?? ev.upvotes ?? 0,
    userVoted: ev.userVoted ?? false
  }));
}

export async function getEventById(id: number, userId?: number): Promise<Event> {
  const raw = await apiFetch<any>(`/api/events/${id}`);
  
  let userVoted = false;
  if (userId) {
    try {
      const voteData = await apiFetch<{ count: number; hasVoted: boolean }>(
        `/api/events/${id}/votes?userId=${userId}`
      );
      userVoted = voteData.hasVoted;
    } catch {
      // Fallback if vote query fails
    }
  }

  return {
    ...raw,
    upvotes: raw.voteCount ?? raw.upvotes ?? 0,
    userVoted
  };
}

export async function toggleVote(
  eventId: number, 
  userId: number, 
  currentlyVoted?: boolean
): Promise<{ upvotes: number; userVoted: boolean }> {
  const safeUserId = parseInt(String(userId), 10) || 1;

  if (currentlyVoted) {
    // Already upvoted then remove vote via DELETE
    try {
      const res = await apiFetch<{ count: number }>(`/api/events/${eventId}/votes?userId=${safeUserId}`, {
        method: 'DELETE'
      });
      return { upvotes: res.count ?? 0, userVoted: false };
    } catch (err) {
      console.error('Failed to remove vote:', err);
      throw err;
    }
  } else {
    // Not upvoted yet then add vote via POST
    try {
      const res = await apiFetch<{ count: number }>(`/api/events/${eventId}/votes`, {
        method: 'POST',
        body: JSON.stringify({ userId: safeUserId })
      });
      return { upvotes: res.count ?? 0, userVoted: true };
    } catch (err) {
      console.error('Failed to add vote:', err);
      throw err;
    }
  }
}
