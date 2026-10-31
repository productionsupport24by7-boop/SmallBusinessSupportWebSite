export interface Ticket {
  id: number;
  ticketNumber: string;
  customerId: number;
  title: string;
  description: string;
  priority: string;
  status: string;
  category: string;
  assignedToUserId?: number | null;
  createdAt: string;
  updatedAt?: string | null;
  resolvedAt?: string | null;
}
