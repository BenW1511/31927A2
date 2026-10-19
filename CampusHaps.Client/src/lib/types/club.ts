export interface Club {
  id: number;
  name: string;
  description: string;
  category: string;
  contactEmail: string;
  eventCount: number;
  postCount: number;
}

export interface ClubPost {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  clubId: number;
  authorId: number;
  authorName: string;
}

export interface CreateClubInput {
  name: string;
  description: string;
  category: string;
  contactEmail: string;
}

export interface CreateClubPostInput {
  title: string;
  content: string;
  authorId: number;
}