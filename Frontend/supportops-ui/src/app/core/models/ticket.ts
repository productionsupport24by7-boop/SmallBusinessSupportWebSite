export interface Ticket {
  id: number;
  ticketNumber: string;
  customerId: number;
  title: string;
  description: string;
  priority: string;
  status: string;
  category: string;
  assignedToUserId: number | null;
  createdOn: string;
  updatedOn: string | null;
  resolvedOn: string | null;
}
