export enum Currency {
  Rupee = 0,
  USD = 1,
  EUR = 2
}

export enum ExpenseCategory {
  General = 0,
  Food = 1,
  Travel = 2,
  Entertainment = 3
}

export interface Expense {
  id: number;
  name: string;
  catagory?: ExpenseCategory;   
  currency: Currency;
  amount: number;
  addedBy: string;
  addedTime: Date;
  groupId: number;
}
