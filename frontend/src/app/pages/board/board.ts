import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { KanbanColumn } from '../../models/kanban-column.model';
import { Ticket } from '../../models/ticket.model';
import { KanbanColumnService } from '../../services/kanban-column';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-board',
  imports: [CommonModule],
  templateUrl: './board.html',
  styleUrl: './board.css',
})
export class Board implements OnInit {
  columns: KanbanColumn[] = [];
  tickets: Ticket[] = [];
  errorMessage = '';

  constructor(
    private kanbanColumnService: KanbanColumnService,
    private ticketService: TicketService
  ) {}

  ngOnInit(): void {
    this.loadBoard();
  }

  loadBoard(): void {
    this.kanbanColumnService.getColumns().subscribe({
      next: (columns) => {
        this.columns = columns;
      },
      error: (error) => {
        console.error('Erreur chargement colonnes', error);
        this.errorMessage = 'Impossible de charger les colonnes.';
      }
    });

    this.ticketService.getTickets().subscribe({
      next: (tickets) => {
        this.tickets = tickets;
      },
      error: (error) => {
        console.error('Erreur chargement tickets', error);
        this.errorMessage = 'Impossible de charger les tickets.';
      }
    });
  }

  getTicketsByColumn(columnId: number): Ticket[] {
    return this.tickets.filter(ticket => ticket.kanbanColumnId === columnId);
  }

  getTotalHoursByColumn(columnId: number): number {
  return this.getTicketsByColumn(columnId)
    .reduce((total, ticket) => total + ticket.timeSpentHours, 0);
  }

  getTotalHours(): number {
    return this.tickets
      .reduce((total, ticket) => total + ticket.timeSpentHours, 0);
  }
}