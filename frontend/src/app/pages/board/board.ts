import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { KanbanColumn } from '../../models/kanban-column.model';
import { Ticket } from '../../models/ticket.model';
import { Auth } from '../../services/auth';
import { KanbanColumnService } from '../../services/kanban-column';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-board',
  imports: [CommonModule, FormsModule, RouterLink, DragDropModule],
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

  editingTicket: Ticket | null = null;
  editTitle = '';
  editDescription = '';
  editTimeSpentHours = 0;

  creatingColumn = false;
  newColumnName = '';

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
      next: columns => (this.columns = [...columns].sort((a, b) => a.order - b.order)),
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

  getColumnDropListId(columnId: number): string {
    return `column-${columnId}`;
  }

  getConnectedColumnIds(): string[] {
    return this.columns.map(column => this.getColumnDropListId(column.id));
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

  openEditModal(ticket: Ticket): void {
    this.editingTicket = ticket;
    this.editTitle = ticket.title;
    this.editDescription = ticket.description;
    this.editTimeSpentHours = ticket.timeSpentHours;
  }

  closeEditModal(): void {
    this.editingTicket = null;
  }

  saveEdit(): void {
    if (!this.editingTicket || !this.editTitle) {
      return;
    }

    const id = this.editingTicket.id;

    this.ticketService
      .updateTicket(id, {
        title: this.editTitle,
        description: this.editDescription,
        timeSpentHours: this.editTimeSpentHours
      })
      .subscribe({
        next: updated => {
          this.tickets = this.tickets.map(ticket => (ticket.id === id ? updated : ticket));
          this.errorMessage = '';
          this.closeEditModal();
        },
        error: () => (this.errorMessage = 'Impossible de modifier le ticket.')
      });
  }

  onTicketDrop(event: CdkDragDrop<Ticket[]>, targetColumnId: number): void {
    const ticket = event.item.data as Ticket;

    if (ticket.kanbanColumnId === targetColumnId) {
      return;
    }

    const previousColumnId = ticket.kanbanColumnId;
    ticket.kanbanColumnId = targetColumnId;

    this.ticketService.moveTicket(ticket.id, { targetColumnId }).subscribe({
      next: updated => {
        this.tickets = this.tickets.map(t => (t.id === updated.id ? updated : t));
      },
      error: () => {
        ticket.kanbanColumnId = previousColumnId;
        this.errorMessage = 'Impossible de déplacer le ticket.';
      }
    });
  }

  openColumnCreator(): void {
    this.creatingColumn = true;
    this.newColumnName = '';
  }

  cancelColumnCreator(): void {
    this.creatingColumn = false;
    this.newColumnName = '';
  }

  saveNewColumn(): void {
    const name = this.newColumnName.trim();

    if (!name) {
      return;
    }

    const userId = this.auth.getCurrentUserId();

    if (userId === null) {
      this.errorMessage = 'Utilisateur non connecté.';
      return;
    }

    this.kanbanColumnService.createColumn(name, userId).subscribe({
      next: column => {
        this.columns = [...this.columns, column].sort((a, b) => a.order - b.order);
        this.errorMessage = '';
        this.cancelColumnCreator();
      },
      error: () => (this.errorMessage = 'Impossible de créer la colonne.')
    });
  }

  deleteTicket(ticketId: number): void {
    if (!confirm('Voulez-vous vraiment supprimer ce ticket ?')) {
      return;
    }

    this.ticketService.deleteTicket(ticketId).subscribe({
      next: () => {
        this.tickets = this.tickets.filter(ticket => ticket.id !== ticketId);
        this.errorMessage = '';
      },
      error: () => (this.errorMessage = 'Impossible de supprimer le ticket.')
    });
  }
}
