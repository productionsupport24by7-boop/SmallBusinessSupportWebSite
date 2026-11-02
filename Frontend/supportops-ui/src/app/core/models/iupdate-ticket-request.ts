export interface IupdateTicketRequest {
  title?: string;
  description?: string;
  priority?: string;
  category?: string;
  status?: string;
  assignedToUserId?: number;
}
