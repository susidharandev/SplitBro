# SplitBro — Frontend Architecture Roadmap & Inventory

> **Purpose:** High-level frontend blueprint mapping the Angular 22 architecture, current file inventory, bootstrapping flow, and technical debt.
> For the deep-dive component specifications, see `FRONTEND_DETAILED_IMPLEMENTATION_PLAN.md`.

---

## 1. Frontend Tech Stack & Environment

| Property | Configuration | Notes |
| :--- | :--- | :--- |
| **Framework** | Angular 22.2 (Standalone) | No `NgModule` / `AppModule` boilerplate |
| **Reactivity** | Pure Zoneless (`signal()`) | No `zone.js`. Signals notify Angular to re-render |
| **Routing** | Angular Router | `app.routes.ts` with route-level lazy loading |
| **Styling** | SCSS | Global design tokens + scoped component styles |
| **Testing** | Vitest | Modern high-speed unit testing |
| **Development Proxy** | `proxy.conf.json` | Forwards all `/api/*` requests to .NET 8 on `:5149` |
| **Dev Server** | `npm start` (Vite) | Serves on `http://localhost:4200` |

---

## 2. Bootstrapping Flow (How the App Loads)

```
[index.html] 
     │  (Browser loads <app-root>)
     ▼
[main.ts] 
     │  (Calls bootstrapApplication(App, appConfig))
     ▼
[app.config.ts] 
     │  (Registers provideRouter(routes), provideHttpClient())
     ▼
[app.ts] & [app.html] 
     │  (Root shell rendering <router-outlet></router-outlet>)
     ▼
[app.routes.ts] 
     │  (Matches URL path: '' ➔ Loads GroupListComponent)
     ▼
[GroupListComponent] 
        (Initial active screen displaying groups and members)
```

---

## 3. Current Frontend File Inventory

```
frontend/src/
├── index.html
├── main.ts
├── styles.scss                 ◄── Global styles & design tokens
├── app/
│   ├── app.config.ts           ◄── App providers (HTTP, Router)
│   ├── app.routes.ts           ◄── Route definitions
│   ├── app.ts / app.html       ◄── Root shell with <router-outlet>
│   │
│   ├── models/                 ◄── TypeScript Data Contracts
│   │   ├── group.model.ts      (Group interface & collections)
│   │   ├── group-member.model.ts (Member interface: userId, name, email)
│   │   └── expense.model.ts    (Expense, Currency & Category enums)
│   │
│   ├── services/               ◄── Shared Singleton Services
│   │   ├── group.service.ts    (ACTIVE: All /api/group CRUD calls)
│   │   ├── expense.service.ts  (STUB: Ready for Milestone 2)
│   │   └── auth.service.ts     (STUB: Ready for Milestone 6)
│   │
│   └── components/
│       └── group-list/
│           └── group-list.component/
│               ├── group-list.component.ts   (Active: Signals, API subscriptions)
│               ├── group-list.component.html (Two-column layout, list & members)
│               └── group-list.component.scss (Component styles)
```

---

## 4. Current Frontend Status & Technical Debt

### What is Working (Milestone 1 ✅)
- Group listing via `GroupService.getGroups()` binding to `groups = signal<Group[]>([])`.
- Group creation with real-time UI updates via `this.groups.update(...)`.
- Selecting groups and dynamically switching RHS view between empty expense area and member management.
- Adding members by user ID and removing members with instant signal-based reactivity.
- Brand logo integrated into the header.

### Technical Debt to Address in Future Milestones
1. **The "God Component":** `GroupListComponent` currently manages groups, members, inputs, and layout in a single file. Milestone 2 will split this into clean, modular components.
2. **Inline Styles:** Template HTML currently uses heavy inline `style="..."` attributes. Moving these into `.scss` will dramatically improve readability.
3. **Double Nested Folder:** `src/app/components/group-list/group-list.component/` has an unnecessary repeated subfolder. Future components will follow standard flat directory structures.

---

## 5. Milestone-by-Milestone Frontend Roadmap

- [x] **Milestone 1: Groups & Members Management** (100% Complete)
- [ ] **Milestone 2: Expenses & Splitting (Single-Payer Engine)**
  - Implement `ExpenseService` (`getExpenses`, `createExpense`).
  - Build `AddExpenseModalComponent` with natural language sentence format.
  - Build Docked `ChooseSplitOptions` companion drawer (`=` and `1.23` modes).
  - Build `ExpenseFeedComponent` & `ExpenseCardComponent` with monthly grouping & "not involved" grey striping.
- [ ] **Milestone 3: Live Group Balances & Scorecard**
  - Implement `BalanceService` (`getBalances`).
  - Build `GroupBalancesPanelComponent` (Right panel live balances with green/orange colors & Simplify Debt toggle).
- [ ] **Milestone 4: Settle Up & Debt Clearance**
  - Implement `SettleUpModalComponent` (Cash/Direct payment dialog).
  - Feed integration for settlement payment items.
- [ ] **Milestone 5: Multi-Payer Bill Splitting**
  - Update `AddExpenseModalComponent` to support "Multiple people paid".
- [ ] **Milestone 6: Active User Switcher & Polish**
  - Implement `NavbarComponent` with user switcher dropdown (`CurrentUserService`).
  - Polish layout with Splitwise teal color palette (`#5bc5a7`).
