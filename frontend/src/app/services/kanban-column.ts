import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KanbanColumn } from '../models/kanban-column.model';

@Injectable({
  providedIn: 'root'
})
export class KanbanColumnService {
  private readonly apiUrl = 'http://localhost:5065/api/kanbancolumns';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getColumns(): Observable<KanbanColumn[]> {
    return this.http.get<KanbanColumn[]>(this.apiUrl, {
      headers: this.getAuthHeaders()
    });
  }
}