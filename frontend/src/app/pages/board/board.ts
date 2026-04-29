import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { KanbanColumn } from '../../models/kanban-column.model';
import { Ticket } from '../../models/ticket.model';
import { Auth } from '../../services/auth';
import { KanbanColumnService } from '../../services/kanban-column';
import { TicketService } from '../../services/ticket';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-board',
  imports: [CommonModule, FormsModule, RouterLink, DragDropModule],
  templateUrl: './board.html',
  styleUrl: './board.css'
})
export class Board implements OnInit {
  private readonly kanbanColumnService = inject(KanbanColumnService);
  private readonly ticketService = inject(TicketService);
  private readonly userService = inject(UserService);
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly isAdmin = this.auth.isAdmin;

  columns: KanbanColumn[] = [];
  tickets: Ticket[] = [];
  errorMessage = '';

  viewingUserId: number | null = null;
  viewingUserName: string | null = null;

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

  editingColumnId: number | null = null;
  editingColumnName = '';

  searchTerm = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const explicitUserId = params['userId'] ? Number(params['userId']) : null;
      const currentId = this.auth.getCurrentUserId();

      this.viewingUserId =
        this.isAdmin() && explicitUserId !== null ? explicitUserId : currentId;

      const isViewingOther =
        this.viewingUserId !== null && this.viewingUserId !== currentId;

      if (isViewingOther) {
        this.userService.getUserById(this.viewingUserId!).subscribe({
          next: user => (this.viewingUserName = user.name),
          error: () => (this.viewingUserName = null)
        });
      } else {
        this.viewingUserName = null;
      }

      this.loadBoard();
    });
  }

  get isViewingOther(): boolean {
    return (
      this.viewingUserId !== null && this.viewingUserId !== this.auth.getCurrentUserId()
    );
  }

  exitViewing(): void {
    this.router.navigate(['/board']);
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
    const userId = this.viewingUserId ?? undefined;

    this.kanbanColumnService.getColumns(userId).subscribe({
      next: columns => (this.columns = [...columns].sort((a, b) => a.order - b.order)),
      error: () => (this.errorMessage = 'Impossible de charger les colonnes.')
    });

    this.ticketService.getTickets(userId).subscribe({
      next: tickets => (this.tickets = tickets),
      error: () => (this.errorMessage = 'Impossible de charger les tickets.')
    });
  }

  get filteredTickets(): Ticket[] {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) return this.tickets;
    return this.tickets.filter(ticket =>
      ticket.title.toLowerCase().includes(term) ||
      (ticket.description?.toLowerCase().includes(term) ?? false)
    );
  }

  getTicketsByColumn(columnId: number): Ticket[] {
    return this.filteredTickets.filter(ticket => ticket.kanbanColumnId === columnId);
  }

  getTotalHoursByColumn(columnId: number): number {
    return this.getTicketsByColumn(columnId).reduce(
      (total, ticket) => total + ticket.timeSpentHours,
      0
    );
  }

  getTotalHours(): number {
    return this.filteredTickets.reduce((total, ticket) => total + ticket.timeSpentHours, 0);
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

    this.tickets = this.tickets.map(t =>
      t.id === ticket.id ? { ...t, kanbanColumnId: targetColumnId } : t
    );

    this.ticketService.moveTicket(ticket.id, { targetColumnId }).subscribe({
      next: updated => {
        this.tickets = this.tickets.map(t => (t.id === updated.id ? updated : t));
      },
      error: () => {
        this.tickets = this.tickets.map(t =>
          t.id === ticket.id ? { ...t, kanbanColumnId: previousColumnId } : t
        );
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

  startEditColumn(column: KanbanColumn): void {
    this.editingColumnId = column.id;
    this.editingColumnName = column.name;
  }

  cancelEditColumn(): void {
    this.editingColumnId = null;
    this.editingColumnName = '';
  }

  saveEditColumn(): void {
    const id = this.editingColumnId;
    const name = this.editingColumnName.trim();

    if (id === null || !name) {
      return;
    }

    this.kanbanColumnService.updateColumn(id, name).subscribe({
      next: updated => {
        this.columns = this.columns.map(column => (column.id === id ? updated : column));
        this.errorMessage = '';
        this.cancelEditColumn();
      },
      error: () => (this.errorMessage = 'Impossible de renommer la colonne.')
    });
  }

  deleteColumn(column: KanbanColumn): void {
    if (!confirm(`Supprimer la colonne « ${column.name} » et tous ses tickets ?`)) {
      return;
    }

    this.kanbanColumnService.deleteColumn(column.id).subscribe({
      next: () => {
        this.columns = this.columns.filter(existing => existing.id !== column.id);
        this.tickets = this.tickets.filter(ticket => ticket.kanbanColumnId !== column.id);
        this.errorMessage = '';
      },
      error: () => (this.errorMessage = 'Impossible de supprimer la colonne.')
    });
  }

  onColumnDrop(event: CdkDragDrop<KanbanColumn[]>): void {
    if (event.previousIndex === event.currentIndex) {
      return;
    }

    const reordered = [...this.columns];
    moveItemInArray(reordered, event.previousIndex, event.currentIndex);
    this.columns = reordered;

    const orderedColumnIds = reordered.map(column => column.id);

    this.kanbanColumnService.reorderColumns(orderedColumnIds).subscribe({
      next: updated => {
        this.columns = [...updated].sort((a, b) => a.order - b.order);
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Impossible de réordonner les colonnes.';
        this.loadBoard();
      }
    });
  }

  saveNewColumn(): void {
    const name = this.newColumnName.trim();

    if (!name) {
      return;
    }

    const userId = this.viewingUserId ?? this.auth.getCurrentUserId();

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
