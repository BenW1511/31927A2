// CampusHaps.Client/src/lib/api/events.ts

import { apiFetch } from './client';
import type { Event } from '../types/event';

export async function getEvents(): Promise<Event[]> {
  return apiFetch<Event[]>('/api/events');
}

export async function getEventById(id: number): Promise<Event> {
  const raw = await apiFetch<any>(`/api/events/${id}`);
  return {
    ...raw,
    upvotes: raw.voteCount ?? raw.upvotes ?? 0
  };
}

// Calls Wud's backend calendar logic: GET /api/events/calendar?from=...&to=...
export async function getEventsByDateRange(from: Date, to: Date): Promise<Event[]> {
  const fromIso = from.toISOString();
  const toIso = to.toISOString();
  return apiFetch<Event[]>(`/api/events/calendar?from=${fromIso}&to=${toIso}`);
}

export async function toggleVote(eventId: number, userId: number): Promise<{ upvotes: number; userVoted: boolean }> {
  const parsedId = parseInt(String(userId), 10);
  const safeUserId = !isNaN(parsedId) && parsedId > 0 && parsedId <= 2147483647 
    ? parsedId 
    : 1;

  const res = await apiFetch<{ count: number; hasVoted: boolean }>(`/api/events/${eventId}/votes`, {
    method: 'POST',
    body: JSON.stringify({ userId: safeUserId })
  });

  return {
    upvotes: res.count,
    userVoted: res.hasVoted
  };
}