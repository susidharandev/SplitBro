import {GroupMember} from './group-member.model';
import {Expense} from './expense.model'
export interface Group
{
    id:number;
    name:string;
    description?:string;
    simplifyDebt:boolean;
    createdAt:Date;
     lastUpdatedAt: Date;    

     groupMembers?:GroupMember[];
     expense? : Expense[];

}