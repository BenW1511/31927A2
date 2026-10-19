export interface Comment {
  id: number;
  content: string;
  createdAt: string;
  userId: number;
  userName: string;
  eventId: number;
}

export interface CreateCommentInput {
  userId: number;
  content: string;
}

export interface UpdateCommentInput {
  userId: number;
  content: string;
}