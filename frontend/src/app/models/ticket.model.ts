export interface Ticket {
  id: number;
  title: string;
  description: string;
  timeSpentHours: number;
  createdAt: string;
  adminNote: string | null;
  assignedUserId: number;
  kanbanColumnId: number;
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  timeSpentHours: number;
  kanbanColumnId: number;
}

export interface UpdateTicketRequest {
  title: string;
  description: string;
  timeSpentHours: number;
}

export interface MoveTicketRequest {
  targetColumnId: number;
}