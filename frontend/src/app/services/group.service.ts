import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Group } from '../models/group.model';

@Injectable({ providedIn: 'root' })
export class GroupService {
  private apiUrl = '/api/group';

  constructor(private http: HttpClient) {}
  // Matches [HttpPost] -> CreateGroup
  createGroup(request: { name: string; description?: string }): Observable<Group> {
    return this.http.post<Group>(this.apiUrl, request);
  }
  // Matches [HttpGet("{groupId:int}")] -> GetGroup
  getGroupById(groupId: number): Observable<Group> {
    return this.http.get<Group>(`${this.apiUrl}/${groupId}`);
  }
  getGroups(): Observable<Group[]> {
    return this.http.get<Group[]>(this.apiUrl);
  }
}
