import { apiFetch } from './client';
import type { Club, ClubPost, CreateClubInput, CreateClubPostInput } from '../types/club';

export async function getClubs(): Promise<Club[]> {
  return apiFetch<Club[]>('/api/clubs');
}

export async function getClubById(id: number): Promise<Club> {
  return apiFetch<Club>(`/api/clubs/${id}`);
}

export async function createClub(data: CreateClubInput): Promise<Club> {
  return apiFetch<Club>('/api/clubs', {
    method: 'POST',
    body: JSON.stringify(data)
  });
}

export async function updateClub(id: number, data: CreateClubInput): Promise<void> {
  return apiFetch<void>(`/api/clubs/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  });
}

export async function deleteClub(id: number): Promise<void> {
  return apiFetch<void>(`/api/clubs/${id}`, {
    method: 'DELETE'
  });
}

export async function getClubPosts(clubId: number): Promise<ClubPost[]> {
  return apiFetch<ClubPost[]>(`/api/clubs/${clubId}/posts`);
}

export async function createClubPost(clubId: number, data: CreateClubPostInput): Promise<ClubPost> {
  // parse authorId explicitly into a 32-bit base-10 integer
  const parsedId = parseInt(String(data.authorId), 10);
  
  
  const safeAuthorId = !isNaN(parsedId) && parsedId > 0 && parsedId <= 2147483647 
    ? parsedId 
    : 1;

  const payload = {
    title: String(data.title).trim(),
    content: String(data.content).trim(),
    authorId: safeAuthorId
  };

  return apiFetch<ClubPost>(`/api/clubs/${clubId}/posts`, {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}
