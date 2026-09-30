# SplitBro — Frontend Detailed Implementation Plan

> **Companion Document:** Detailed technical specification for all 10 Angular components, 4 shared services, routing configurations, state management flow, and global design tokens.

---

## 1. Master Component Inventory (The 10 Core Components)

```
┌────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ 1. NavbarComponent  (Top Bar: Logo, Quick Add Button, Active User Switcher)                            │
├──────────────────────────────┬─────────────────────────────────────────────────────────────────────────┤
│ 2. SidebarComponent          │ 4. GroupDashboardComponent (Router Outlet: /groups/:groupId)            │
│    • Groups List             │    ┌───────────────────────────┬──────────────────────────────────────┐ │
│    • 3. CreateGroupModal     │    │ 5. GroupHeaderComponent   │ 8. GroupBalancesPanelComponent       │ │
│                              │    ├───────────────────────────┤    • Member live debts               │ │
│                              │    │ 6. ExpenseFeedComponent   │    • Simplify debts toggle           │ │
│                              │    │    └── 7. ExpenseCard     │    • Member actions (Add/Remove)     │ │
│                              │    └───────────────────────────┴──────────────────────────────────────┘ │
│                              │    [ 9. AddExpenseModal ]    [ 10. SettleUpModal ]                      │
└──────────────────────────────┴─────────────────────────────────────────────────────────────────────────┘
```

---

### Layer A: Global App Shell (Persistent Across All Pages)

#### 1. `NavbarComponent`
* **File:** `src/app/layout/navbar/navbar.component.ts`
* **Job:** Fixed top brand banner with identity and quick actions.
* **Template Elements:**
  * App logo (`/SplitBro_logo.png`) & brand name (`SplitBro`).
  * Primary Action: **`[ + Add an expense ]`** button (orange `#ff652f`). Clicking emits an event or toggles `isAddExpenseOpen` signal.
  * **User Switcher Dropdown:** Displays current user name with avatar (e.g., *"Logged in as: Susi (1) ▼"*). Selecting another user calls `CurrentUserService.switchUser(id)`.
* **State Injected:** `CurrentUserService`.

#### 2. `SidebarComponent`
* **File:** `src/app/layout/sidebar/sidebar.component.ts`
* **Job:** Left navigation bar anchoring the user.
* **Template Elements:**
  * Top navigation links: `📊 Dashboard`, `🕒 Recent Activity`, `📑 All Expenses`.
  * Section Header: **"GROUPS"** with a **`[ + Add ]`** button (opens `CreateGroupModalComponent`).
  * Group list with active route styling (`routerLink="/groups/{{ group.id }}" routerLinkActive="active-group"`).
* **State Injected:** `GroupService`.

---

### Layer B: Group Management

#### 3. `CreateGroupModalComponent`
* **File:** `src/app/features/groups/create-group-modal/create-group-modal.component.ts`
* **Job:** Modal popup dialog to create a new group.
* **Inputs/Outputs:**
  * `@Input() isOpen = signal<boolean>(false);`
  * `@Output() closed = new EventEmitter<void>();`
* **Template Elements:**
  * Input for Group Name (`[(ngModel)]="name"`).
  * Input for Description (`[(ngModel)]="description"`).
  * Buttons: `[ Add Group ]` (calls `GroupService.createGroup()`) and `[ Cancel ]`.

#### 4. `GroupDashboardComponent` (The Page Container)
* **File:** `src/app/features/groups/group-dashboard/group-dashboard.component.ts`
* **Route:** `/groups/:groupId`
* **Job:** Reads the `:groupId` from the URL via Angular route inputs, loads group data, and coordinates the 2-column layout (Center Feed + Right Balances).
* **Modern Route Input:**
  ```typescript
  export class GroupDashboardComponent {
    groupId = input.required<string>(); // Automatically bound from route /groups/:groupId

    constructor(private groupService: GroupService, private expenseService: ExpenseService) {
      effect(() => {
        const id = Number(this.groupId());
        if (id) {
          this.groupService.selectGroupById(id);
          this.expenseService.loadExpenses(id);
        }
      });
    }
  }
  ```

---

### Layer C: Expenses Engine (Milestone 2 Focus)

#### 5. `GroupHeaderComponent`
* **File:** `src/app/features/groups/group-header/group-header.component.ts`
* **Job:** Top of the center workspace.
* **Template Elements:**
  * Group avatar icon + Group Name + member count subtext (*"7 people"*).
  * Two prominent buttons:
    * `[ + Add an expense ]` (Orange `#ff652f`)
    * `[ 💳 Settle up ]` (Teal `#5bc5a7`)

#### 6. `ExpenseFeedComponent`
* **File:** `src/app/features/expenses/expense-feed/expense-feed.component.ts`
* **Job:** Chronological transaction timeline inside the group.
* **Template Elements:**
  * Grouped by month (e.g. `── SEPTEMBER 2026 ──`).
  * Loops through `expenses()` signal and renders `<app-expense-card [expense]="item" />`.
  * Empty state placeholder when no expenses exist.

#### 7. `ExpenseCardComponent`
* **File:** `src/app/features/expenses/expense-card/expense-card.component.ts`
* **Inputs:** `@Input({ required: true }) expense!: ExpenseResponse;`
* **Template Elements:**
  * Left: Date badge (`SEP 28`) + Category Icon (🍕 Food, 🚗 Taxi, 🛒 General).
  * Center: Expense title (`"Beach Shack Dinner"`).
  * Right: Amount summary:
    * **If you lent:** 🟢 *"You paid ₹600, lent ₹400"*
    * **If you borrowed:** 🔴 *"Bob paid ₹300, you borrowed ₹100"*
    * **If not involved:** ⚪ **Diagonal grey striped background** with text *"not involved"*.

#### 8. `AddExpenseModalComponent` (The Signature Modal)
* **File:** `src/app/features/expenses/add-expense-modal/add-expense-modal.component.ts`
* **Job:** The core expense creation form with the docked side-by-side split drawer.
* **Visual Structure:**
  ```
  ┌───────────────────────────────────────────────┬───────────────────────────────────────────┐
  │ LEFT: Main Modal                              │ RIGHT: Docked "Choose Split Options"      │
  │ • Category Icon Picker                        │ • Toolbar: [ = ] [ 1.23 ] [ % ] [ ☰ ]     │
  │ • Description Input ("Dinner")                │ • Member Checklist:                       │
  │ • Currency & Amount ("₹ 600.00")              │   [✔] Ani             ₹200.00             │
  │ • Natural Sentence:                           │   [✔] Asker           ₹200.00             │
  │   "Paid by [you ▼] and split [equally ▼]"     │   [✔] Susi            ₹200.00             │
  │   (₹200.00/person)                            │   [ ] Dhana (excluded) ₹0.00              │
  │ • Date: [ September 30, 2026 ]                │ • Dynamic calculation updates live        │
  │ • [ Cancel ]   [ Save ]                       │   when checkboxes are toggled             │
  └───────────────────────────────────────────────┴───────────────────────────────────────────┘
  ```

---

### Layer D: Balances & Settlements (Milestones 3 & 4)

#### 9. `GroupBalancesPanelComponent`
* **File:** `src/app/features/groups/group-balances-panel/group-balances-panel.component.ts`
* **Job:** Right sidebar of the group page (~280px wide).
* **Template Elements:**
  * Header: **"GROUP BALANCES"**.
  * Switch: **"Simplify debts is ON [?]"**.
  * Live Member List:
    * 🟢 *Asker Parvez: gets back ₹2,181.68*
    * 🔴 *Susi (You): owes ₹515.01*
    * ⚪ *KARTHIKEYAN: settled up*
  * Quick button: `[ ⚙️ Manage Members ]` to add or remove people from the group.

#### 10. `SettleUpModalComponent`
* **File:** `src/app/features/settlements/settle-up-modal/settle-up-modal.component.ts`
* **Job:** Payment recording dialog.
* **Template Elements:**
  * Header: "Settle up" (Teal banner).
  * Selector: *"Who paid whom?"* (e.g. `[Susi] paid [Asker]`).
  * Amount input (pre-filled with the exact debt).
  * Action button: **`[ Record a cash payment ]`** (saves settlement and resets balance).

---

## 2. The 4 Singleton Services

### 1. `CurrentUserService` (`src/app/services/current-user.service.ts`)
Manages the active logged-in user context across the app.
```typescript
@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  // Currently active user signal
  activeUser = signal<{ id: number; name: string; email: string }>({
    id: 1,
    name: 'Susi',
    email: 'susi@splitbro.local'
  });

  switchUser(userId: number, name: string, email: string): void {
    this.activeUser.set({ id: userId, name, email });
  }
}
```

### 2. `GroupService` (`src/app/services/group.service.ts`)
Handles groups and member operations.
```typescript
@Injectable({ providedIn: 'root' })
export class GroupService {
  groups = signal<Group[]>([]);
  selectedGroup = signal<Group | null>(null);
  members = signal<GroupMember[]>([]);

  getGroups(): Observable<Group[]> { ... }
  createGroup(payload: CreateGroupRequest): Observable<Group> { ... }
  getMembers(groupId: number): Observable<GroupMember[]> { ... }
  addMember(groupId: number, userId: number): Observable<GroupMember> { ... }
  removeMember(groupId: number, userId: number): Observable<void> { ... }
}
```

### 3. `ExpenseService` (`src/app/services/expense.service.ts`)
Handles expense creation and fetching.
```typescript
@Injectable({ providedIn: 'root' })
export class ExpenseService {
  expenses = signal<ExpenseResponse[]>([]);

  getExpenses(groupId: number): Observable<ExpenseResponse[]> { ... }
  createExpense(groupId: number, payload: CreateExpenseRequest): Observable<ExpenseResponse> { ... }
  deleteExpense(expenseId: number): Observable<void> { ... }
}
```

### 4. `BalanceService` (`src/app/services/balance.service.ts`)
Handles group debt calculations and settlements.
```typescript
@Injectable({ providedIn: 'root' })
export class BalanceService {
  groupBalances = signal<GroupBalanceResponse | null>(null);

  getBalances(groupId: number): Observable<GroupBalanceResponse> { ... }
  recordSettlement(groupId: number, payload: SettlementRequest): Observable<SettlementResponse> { ... }
}
```

---

## 3. Routing Configuration (`app.routes.ts`)

Enable `withComponentInputBinding()` in `app.config.ts` so route parameters (`:groupId`) bind directly to component inputs:

```typescript
// app.config.ts
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient()
  ]
};

// app.routes.ts
export const routes: Routes = [
  { path: '', redirectTo: 'groups', pathMatch: 'full' },
  { path: 'groups', component: GroupDashboardComponent },
  { path: 'groups/:groupId', component: GroupDashboardComponent },
  { path: '**', redirectTo: 'groups' }
];
```

---

## 4. State Management Flow (Signal Architecture)

```
[User clicks Save on Expense Modal]
               │
               ▼
  Calls ExpenseService.createExpense()
               │
               ▼
  Backend returns 200 OK with new ExpenseResponse
               │
               ▼
  ExpenseService updates signal:
  this.expenses.update(prev => [newExpense, ...prev]);
               │
               ▼
  [ExpenseFeedComponent] automatically re-renders with new card!
               │
               ▼
  [BalanceService] automatically triggers balance refresh
  this.loadBalances(groupId);
               │
               ▼
  [GroupBalancesPanelComponent] updates member debts instantly!
```

---

## 5. Global Design Tokens (`src/styles.scss`)

```scss
:root {
  // Brand Palette
  --sb-teal: #5bc5a7;
  --sb-teal-dark: #38a385;
  --sb-orange: #ff652f;
  --sb-orange-hover: #e5531f;

  // Financial Semantics
  --sb-lent-green: #5bc5a7;
  --sb-owed-red: #ff5252;
  --sb-settled-grey: #888888;
  --sb-not-involved-bg: repeating-linear-gradient(
    45deg,
    #f9f9f9,
    #f9f9f9 10px,
    #f0f0f0 10px,
    #f0f0f0 20px
  );

  // Layout & Spacing
  --sb-border-color: #e0e0e0;
  --sb-card-bg: #ffffff;
  --sb-sidebar-width: 240px;
  --sb-right-panel-width: 280px;
}
```
