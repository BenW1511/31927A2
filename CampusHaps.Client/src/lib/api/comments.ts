import { apiFetch } from './client';
import type { Comment, CreateCommentInput, UpdateCommentInput } from '../types/comment';

export async function getEventComments(eventId: number): Promise<Comment[]> {
  return apiFetch<Comment[]>(`/api/events/${eventId}/comments`);
}

export async function createComment(eventId: number, data: CreateCommentInput): Promise<Comment> {
  const parsedId = parseInt(String(data.userId), 10);
  const safeUserId = !isNaN(parsedId) && parsedId > 0 && parsedId <= 2147483647 
    ? parsedId 
    : 1;

  const payload = {
    content: String(data.content).trim(),
    userId: safeUserId
  };

  return apiFetch<Comment>(`/api/events/${eventId}/comments`, {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}

export async function updateComment(eventId: number, commentId: number, data: UpdateCommentInput): Promise<void> {
  return apiFetch<void>(`/api/events/${eventId}/comments/${commentId}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  });
}

export async function deleteComment(eventId: number, commentId: number, userId: number): Promise<void> {
  return apiFetch<void>(`/api/events/${eventId}/comments/${commentId}?userId=${userId}`, {
    method: 'DELETE'
  });
}