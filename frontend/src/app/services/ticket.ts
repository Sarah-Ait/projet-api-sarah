import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateTicketRequest, Ticket } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private readonly apiUrl = 'http://localhost:5065/api/tickets';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl, {
      headers: this.getAuthHeaders()
    });
  }

  createTicket(request: CreateTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, request, {
      headers: this.getAuthHeaders()
    });
  }

  deleteTicket(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      headers: this.getAuthHeaders()
    });
  }
}