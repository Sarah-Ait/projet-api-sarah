import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { KanbanColumn } from '../../models/kanban-column.model';
import { Ticket } from '../../models/ticket.model';
import { Auth } from '../../services/auth';
import { KanbanColumnService } from '../../services/kanban-column';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-board',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './board.html',
  styleUrl: './board.css'
})
export class Board implements OnInit {
  private readonly kanbanColumnService = inject(KanbanColumnService);
  private readonly ticketService = inject(TicketService);
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  readonly isAdmin = this.auth.isAdmin;

  columns: KanbanColumn[] = [];
  tickets: Ticket[] = [];
  errorMessage = '';

  newTicketTitle = '';
  newTicketDescription = '';
  newTicketTimeSpentHours = 0;
  newTicketColumnId: number | null = null;

  ngOnInit(): void {
    this.loadBoard();
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.auth.clearState();
        this.router.navigate(['/']);
      }
    });
  }

  loadBoard(): void {
    this.kanbanColumnService.getColumns().subscribe({
      next: columns => (this.columns = columns),
      error: () => (this.errorMessage = 'Impossible de charger les colonnes.')
    });

    this.ticketService.getTickets().subscribe({
      next: tickets => (this.tickets = tickets),
      error: () => (this.errorMessage = 'Impossible de charger les tickets.')
    });
  }

  getTicketsByColumn(columnId: number): Ticket[] {
    return this.tickets.filter(ticket => ticket.kanbanColumnId === columnId);
  }

  getTotalHoursByColumn(columnId: number): number {
    return this.getTicketsByColumn(columnId).reduce(
      (total, ticket) => total + ticket.timeSpentHours,
      0
    );
  }

  getTotalHours(): number {
    return this.tickets.reduce((total, ticket) => total + ticket.timeSpentHours, 0);
  }

  createTicket(): void {
    if (!this.newTicketTitle || this.newTicketColumnId === null) {
      this.errorMessage = 'Le titre et la colonne sont obligatoires.';
      return;
    }

    this.ticketService
      .createTicket({
        title: this.newTicketTitle,
        description: this.newTicketDescription,
        timeSpentHours: this.newTicketTimeSpentHours,
        kanbanColumnId: this.newTicketColumnId
      })
      .subscribe({
        next: () => {
          this.newTicketTitle = '';
          this.newTicketDescription = '';
          this.newTicketTimeSpentHours = 0;
          this.newTicketColumnId = null;
          this.errorMessage = '';
          this.loadBoard();
        },
        error: () => (this.errorMessage = 'Impossible de créer le ticket.')
      });
  }

  deleteTicket(ticketId: number): void {
    if (!confirm('Voulez-vous vraiment supprimer ce ticket ?')) {
      return;
    }

    this.ticketService.deleteTicket(ticketId).subscribe({
      next: () => {
        this.errorMessage = '';
        this.loadBoard();
      },
      error: () => (this.errorMessage = 'Impossible de supprimer le ticket.')
    });
  }
}
