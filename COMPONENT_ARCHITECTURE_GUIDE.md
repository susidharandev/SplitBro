# SplitBro — Frontend Component Architecture & Organization Guide

> **Purpose:** Comprehensive architectural guide explaining Angular Component Hierarchy, Parent-Child Relationships, Smart vs. Dumb Component Design, and Feature-Based Folder Organization.

---

## 1. The Core Philosophy: "Smart" vs. "Dumb" Components

In scalable Angular applications, components are strictly separated into two functional roles:

| Component Type | Role Analogy | SplitBro Example | Responsibilities |
| :--- | :--- | :--- | :--- |
| **Smart / Container Component** | *The Restaurant Manager* | `GroupDashboardComponent`<br>`ExpenseFeedComponent` | • Injects Services & handles API calls<br>• Reads Route Parameters (`:groupId`)<br>• Manages business logic and coordinate state<br>• Passes data down to children |
| **Dumb / Presentational Component**| *The Waiter* | `ExpenseCardComponent`<br>`UserAvatarComponent` | • Does **not** talk to backend APIs<br>• Receives data via `input()`<br>• Renders pure HTML & styling<br>• Emits user actions via `output()` |

---

## 2. The SplitBro Component Family Tree

Here is the complete hierarchy of how components nest inside each other in the DOM:

```
[ App (Root Shell) ]
  │
  ├── [ NavbarComponent ]                      (Layout Parent: Logo, User Switcher)
  │
  └── [ RouterOutlet ]
        │
        └── [ GroupDashboardComponent ]        ◄── [SMART CONTAINER / PAGE PARENT]
              │
              ├── [ GroupHeaderComponent ]     (Child: Title, Member Count, Top Action Buttons)
              │
              ├── [ ExpenseFeedComponent ]     (Smart Child: Loops over expenses)
              │     │
              │     └── [ ExpenseCardComponent ] ◄── [DUMB GRANDCHILD: Single Transaction Row]
              │
              ├── [ GroupBalancesPanelComponent ] (Child: Right Sidebar Debt Scorecard)
              │     │
              │     └── [ UserAvatarComponent ]  ◄── [DUMB GRANDCHILD: Photo / Initials Circle]
              │
              ├── [ AddExpenseModalComponent ] (Dialog Child: Form + Docked Split Drawer)
              │
              └── [ SettleUpModalComponent ]   (Dialog Child: Cash Payment Flow)
```

---

## 3. The 3 Component Communication Patterns

There are **only 3 ways** components communicate in modern Angular:

```
               ┌────────────────────────────────────────────────────────┐
               │                    PARENT COMPONENT                    │
               │               (ExpenseFeedComponent)                   │
               └────────────────────────────────────────────────────────┘
                          │                                  ▲
        1. DATA DOWN      │                                  │  2. EVENT UP
      [expense]="item"    │                                  │  (delete)="onDelete($event)"
                          ▼                                  │
               ┌────────────────────────────────────────────────────────┐
               │                    CHILD COMPONENT                     │
               │                (ExpenseCardComponent)                  │
               └────────────────────────────────────────────────────────┘
```

---

### Pattern 1: Parent ➔ Child (Passing Data Down via `input()`)
The parent owns the data array, and passes individual objects down to the child using **property binding `[ ]`**.

#### In Child (`expense-card.component.ts`):
```typescript
import { Component, input } from '@angular/core';
import { ExpenseResponse } from '../../../models/expense.model';

@Component({
  selector: 'app-expense-card',
  standalone: true,
  template: `
    <div class="expense-card" [class.not-involved]="expense().isNotInvolved">
      <span class="icon">{{ expense().categoryIcon }}</span>
      <div class="info">
        <h4>{{ expense().name }}</h4>
        <small>{{ expense().addedTime | date:'mediumDate' }}</small>
      </div>
      <div class="amount">₹{{ expense().amount }}</div>
    </div>
  `
})
export class ExpenseCardComponent {
  // Angular 18/22 Signal-based input:
  expense = input.required<ExpenseResponse>();
}
```

#### In Parent (`expense-feed.component.html`):
```html
<!-- Parent loops over signal array and passes each item into child via [ ] -->
@for (item of expenses(); track item.id) {
  <app-expense-card [expense]="item" />
}
```

---

### Pattern 2: Child ➔ Parent (Emitting Events Up via `output()`)
When a user clicks an action button inside a child (e.g. Delete, View Receipt), the child does **not** make HTTP calls. Instead, it emits an event up to the parent using **event binding `( )`**.

#### In Child (`expense-card.component.ts`):
```typescript
export class ExpenseCardComponent {
  expense = input.required<ExpenseResponse>();

  // Declares an event emitter
  deleteClicked = output<number>();

  onDelete(): void {
    // Sends the expense ID up to the parent
    this.deleteClicked.emit(this.expense().id);
  }
}
```

#### In Parent (`expense-feed.component.html`):
```html
<app-expense-card 
  [expense]="item" 
  (deleteClicked)="handleDeleteExpense($event)">
</app-expense-card>
```

#### In Parent (`expense-feed.component.ts`):
```typescript
handleDeleteExpense(expenseId: number): void {
  // Parent handles the actual API call and state update
  this.expenseService.deleteExpense(expenseId).subscribe();
}
```

---

### Pattern 3: Unrelated Components (Shared Service via Signals)
When components have **no direct Parent-Child connection** (e.g. `NavbarComponent` at the top of the screen wants to open `AddExpenseModalComponent` inside the page), they communicate via a **Shared Signal in a Service**:

```
[ NavbarComponent ] ────► ExpenseService.isModalOpen.set(true) ────► [ AddExpenseModalComponent ]
(User clicks button)                                                  (Popup opens reactively!)
```

#### In Service (`expense.service.ts`):
```typescript
@Injectable({ providedIn: 'root' })
export class ExpenseService {
  isAddExpenseModalOpen = signal<boolean>(false);

  openModal(): void { this.isAddExpenseModalOpen.set(true); }
  closeModal(): void { this.isAddExpenseModalOpen.set(false); }
}
```

---

## 4. Feature-Based Directory Structure (`src/app/`)

Instead of placing all files in one folder, components are organized by **Feature Modules**:

```
frontend/src/app/
├── layout/                           ◄── Global persistent frame
│   ├── navbar/                       (NavbarComponent: logo, active user switcher)
│   └── sidebar/                      (SidebarComponent: navigation, group list)
│
├── features/                         ◄── Business capability domains
│   ├── groups/
│   │   ├── group-dashboard/          (Smart Parent page: /groups/:groupId)
│   │   ├── group-header/             (Child: Title & action buttons)
│   │   └── group-balances-panel/     (Child: Live member debts & simplify debts)
│   │
│   ├── expenses/
│   │   ├── expense-feed/             (Smart Child: Monthly grouped feed)
│   │   ├── expense-card/             (Dumb Grandchild: Single card with visual cues)
│   │   └── add-expense-modal/        (Dialog Child: Natural sentence form + split drawer)
│   │
│   └── settlements/
│       └── settle-up-modal/          (Dialog Child: Cash payment settlement)
│
├── shared/                           ◄── Reusable dumb widgets used everywhere
│   └── user-avatar/                  (UserAvatarComponent: initials / photo circle)
│
├── models/                           ◄── TypeScript contracts & enums
└── services/                         ◄── Singleton data services & API callers
```

---

## 5. Architectural Benefits of This Design

1. **Isolation:** Editing the card styling in `expense-card/` cannot break the modal logic in `add-expense-modal/`.
2. **Reusability:** `<app-user-avatar>` is written once, but rendered inside the navbar, balance scorecard, expense cards, and member checklists.
3. **High Performance:** Dumb components don't hold heavy service injections. Angular can check and render them at lightning speed.
4. **Clean Git History:** Different features live in distinct folders, minimizing merge conflicts when collaborating.
