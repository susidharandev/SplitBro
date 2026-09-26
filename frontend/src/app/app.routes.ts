import { Routes } from '@angular/router';
import { GroupListComponent } from './components/group-list/group-list.component/group-list.component';

export const routes: Routes = [
    {path: '', component : GroupListComponent},
    {path:'**', redirectTo:''}
];
