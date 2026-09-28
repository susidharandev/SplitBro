import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Group } from '../models/group.model';
import { GroupMember } from '../models/group-member.model';

@Injectable({ providedIn: 'root' })
export class GroupService {
  private apiUrl = '/api/group';

  constructor(private http: HttpClient) {}

  // group methods
  createGroup(request: { userId:number, name: string; description?: string }): Observable<Group> {
    return this.http.post<Group>(this.apiUrl, request);
  }
  getGroups(): Observable<Group[]> {
    return this.http.get<Group[]>(this.apiUrl);
  }
  getGroupById(groupId: number): Observable<Group> {
    return this.http.get<Group>(`${this.apiUrl}/${groupId}`);
  }

  // members methods
  getMembers(groupId:number) : Observable<GroupMember[]>{
    return this.http.get<GroupMember[]>(`${this.apiUrl}/${groupId}/members`);
  }
  addMembers(groupId:number, userId:number) : Observable<GroupMember>{
    return this.http.post<GroupMember>(`${this.apiUrl}/${groupId}/members/${userId}`, {});
  }
  removeMembers(groupId:number, userId:number) : Observable<void>{
    return this.http.delete<void>(`${this.apiUrl}/${groupId}/members/${userId}`,{})
  }
}
