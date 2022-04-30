import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Room } from '../models/room';

@Injectable({ providedIn: 'root' })
export class RoomsService {

  constructor(private httpClient: HttpClient) {
  }

  get(id: string): Observable<Room> {
    return this.httpClient.get<Room>(`${environment.apiUrl}/api/rooms/${id}`);
  }

  getAll(): Observable<Room[]> {
    return this.httpClient.get<Room[]>(`${environment.apiUrl}/api/rooms`);
  }

  create(name: string): Observable<Room> {
    return this.httpClient.post<Room>(`${environment.apiUrl}/api/rooms`, { name });
  }
}
