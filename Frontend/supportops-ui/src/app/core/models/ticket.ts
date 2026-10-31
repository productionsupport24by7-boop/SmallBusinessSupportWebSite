export interface Ticket {
  id: number;
  ticketNumber: string;
  title: string;
  description: string;
  priority: string;
  status: string;
  category: string;
  assignedToUserId?: string;
  createdAt: string;
  updatedAt?: string;
  resolvedAt?: string;
}
