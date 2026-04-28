import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { KanbanColumn } from '../../models/kanban-column.model';
import { Ticket } from '../../models/ticket.model';
import { Auth } from '../../services/auth';
import { KanbanColumnService } from '../../services/kanban-column';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-board',
  imports: [CommonModule, FormsModule],
  templateUrl: './board.html',
  styleUrl: './board.css',
})
export class Board implements OnInit {
  columns: KanbanColumn[] = [];
  tickets: Ticket[] = [];
  errorMessage = '';

  newTicketTitle = '';
  newTicketDescription = '';
  newTicketTimeSpentHours = 0;
  newTicketColumnId: number | null = null;

  constructor(
    private kanbanColumnService: KanbanColumnService,
    private ticketService: TicketService,
    private auth: Auth
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

  createTicket(): void {
    if (!this.newTicketTitle || this.newTicketColumnId === null) {
      this.errorMessage = 'Le titre et la colonne sont obligatoires.';
      return;
    }

    const currentUserId = this.auth.getCurrentUserId();

    if (currentUserId === null) {
      this.errorMessage = 'Utilisateur non connecté.';
      return;
    }

    this.ticketService.createTicket({
      title: this.newTicketTitle,
      description: this.newTicketDescription,
      timeSpentHours: this.newTicketTimeSpentHours,
      assignedUserId: currentUserId,
      kanbanColumnId: this.newTicketColumnId
    }).subscribe({
      next: () => {
        this.newTicketTitle = '';
        this.newTicketDescription = '';
        this.newTicketTimeSpentHours = 0;
        this.newTicketColumnId = null;
        this.errorMessage = '';
        this.loadBoard();
      },
      error: (error) => {
        console.error('Erreur création ticket', error);
        this.errorMessage = 'Impossible de créer le ticket.';
      }
    });
  }
}