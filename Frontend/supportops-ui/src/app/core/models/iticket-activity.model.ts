export interface TicketActivity {
  id: number;
  ticketId: number;
  activityType: string;
  fieldName: string | null;
  oldValue: string | null;
  newValue: string | null;
  comment: string | null;
  changedByUserId: number;
  changedByName: string;
  createdOn: string;
}
