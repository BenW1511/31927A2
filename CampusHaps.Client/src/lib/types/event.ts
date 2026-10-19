export interface Event {
  id: number;
  title: string;
  description: string;
  date?: string;
  startTime?: string;
  endTime?: string;
  location: string;
  clubId?: number;
  clubName?: string;
  upvotes: number;
  userVoted?: boolean;
  type?: string;
}