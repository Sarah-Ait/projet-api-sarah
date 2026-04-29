import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../core/api.config';
import { KanbanColumn } from '../models/kanban-column.model';

@Injectable({ providedIn: 'root' })
export class KanbanColumnService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${API_BASE_URL}/api/kanbancolumns`;

  getColumns(userId?: number): Observable<KanbanColumn[]> {
    const url = userId !== undefined ? `${this.apiUrl}?userId=${userId}` : this.apiUrl;
    return this.http.get<KanbanColumn[]>(url);
  }

  createColumn(name: string, userId: number): Observable<KanbanColumn> {
    return this.http.post<KanbanColumn>(this.apiUrl, { name, userId });
  }

  updateColumn(id: number, name: string): Observable<KanbanColumn> {
    return this.http.put<KanbanColumn>(`${this.apiUrl}/${id}`, { name });
  }

  deleteColumn(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  reorderColumns(orderedColumnIds: number[]): Observable<KanbanColumn[]> {
    return this.http.put<KanbanColumn[]>(`${this.apiUrl}/reorder`, { orderedColumnIds });
  }
}
