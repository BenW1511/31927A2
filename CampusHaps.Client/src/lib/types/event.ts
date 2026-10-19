export enum EventType {
  Sport = 0,
  Academic = 1,
  Social = 2,
  Club = 3,
  Other = 4
}
export interface Event {
  id: number;
  title: string;
  description: string;
  date?: string;
  startTime: string;
  endTime: string;
  location: string;
  clubId?: number | null;
  clubName?: string | null;
  upvotes: number; // Maps to voteCount
  userVoted?: boolean;
  type?: EventType | string;
  ticketUrl?: string | null;
  status?: number;
  commentCount?: number;
}

export interface EventDto {
  id: number;
  title: string;
  description: string;
  location: string;
  startTime: string;
  endTime: string;
  ticketUrl?: string | null;
  type: EventType;
  clubId?: number | null;
  status: number;
  clubName?: string | null;
  voteCount: number;
  commentCount: number;
}
export interface CreateEventDto {
  title: string;
  description: string;
  location: string;
  startTime: string;
  endTime: string;
  ticketUrl?: string | null;
  type: EventType;
  clubId?: number | null;
}