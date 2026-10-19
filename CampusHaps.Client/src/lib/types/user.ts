export interface User {
  id: number;
  name: string;
  email: string;
  role: 'student' | 'club_admin';
  clubId?: number;
}