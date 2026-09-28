import { CommonModule, TrailingSlashPathLocationStrategy } from '@angular/common';
import { Component, OnInit, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Group } from '../../../models/group.model';
import { GroupService } from '../../../services/group.service';
import { GroupMember } from '../../../models/group-member.model';
import { Currency } from '../../../models/expense.model';

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-group-list',
  styleUrl: './group-list.component.scss',
  templateUrl: './group-list.component.html',
  standalone: true,
})
export class GroupListComponent implements OnInit {
  constructor(private groupService: GroupService) {}
  ngOnInit(): void {
    this.loadGroup();
  }
  groups = signal<Group[]>([]);
  members = signal<GroupMember[]>([]);
  selectedGroup = signal<Group | null>(null);

    // Form Inputs
  newGroupName: string = '';
  newGroupDescription: string = '';
  newMemberUserId: number | null = null;
  memberErrorMessage: string = '';

  // group related
  loadGroup(): void {
    this.groupService.getGroups().subscribe({
      next: (data) => {
        this.groups.set(data);
      },
      error: (err) => {
        console.error('error fetching group', err);
      },
    });
  }
  onCreateGroup(): void {
    if (!this.newGroupName.trim()) return;

    const payload = {
      userId: 1, // temp for now until user is implemented
      name: this.newGroupName,
      description: this.newGroupDescription,
    };

    this.groupService.createGroup(payload).subscribe({
      next: (data) => {
        this.groups.update((current) => [...current, data]);
        this.newGroupName = '';
        this.newGroupDescription = '';
      },
      error: (err) => {
        console.error('error in group creation', err);
      },
    });
  }

  // group members related

  onSelectGroup(group: Group): void {
    this.selectedGroup.set(group);
    this.memberErrorMessage = '';
    this.newMemberUserId = null;

    this.groupService.getMembers(group.id).subscribe({
      next: (data) => {
        this.members.set(data);
      },
      error: (err) => console.log(`get memebers failed with error`, err),
    });
  }
  onAddMember(): void {
    const group = this.selectedGroup();
    if (!group || !this.newMemberUserId) return;

    this.groupService.addMembers(group.id, this.newMemberUserId).subscribe({
      next: (data) => {
        this.members.update((current) => [...current, data]);
        this.newMemberUserId = null;
      },
      error: (err) => {
        console.log(`add member failed with error`, err);
      },
    });
  }
  onRemoveMember(userId: number): void {
    const group = this.selectedGroup();
    if (!group || !userId) return;

    this.groupService.removeMembers(group.id, userId).subscribe({
      next: (data) => {
        this.members.update((current) => current.filter((gm) => gm.userId != userId));
      },
      error: (err) => {
        console.log(`error in remove member`, err);
      },
    });
  }
}
