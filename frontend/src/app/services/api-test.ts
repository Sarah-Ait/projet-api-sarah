import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiTest {
  private readonly apiUrl = 'http://localhost:5065';

  constructor(private http: HttpClient) {}

  testBackend(): Observable<string> {
    return this.http.get(`${this.apiUrl}/api/test`, {
      responseType: 'text'
    });
  }
}