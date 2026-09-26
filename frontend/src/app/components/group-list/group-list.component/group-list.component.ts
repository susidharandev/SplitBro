import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Group } from '../../../models/group.model';
import { GroupService } from '../../../services/group.service';

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
  groups: Group[] = [];

  newGroupName: string = '';
  newGroupDescription: string = '';

  loadGroup(): void {
    this.groupService.getGroups().subscribe({
      next: (data) => {
        this.groups = data;
      },
      error: (err) => {
        console.error('error fetching group', err);
      },
    });
  }
  onCreateGroup(): void {
    if (!this.newGroupName.trim()) return;

    const payload = {
      name: this.newGroupName,
      description: this.newGroupDescription,
    };

    this.groupService.createGroup(payload).subscribe({
      next: (data) => {
        this.groups.push(data);
        this.newGroupName = '';
        this.newGroupDescription = '';
      },
      error: (err) => {
        console.error('error in group creation', err);
      },
    });
  }
}
