# SplitBro — Master Project Plan & Architecture Roadmap

> **SplitBro** is a full-featured, clean-architecture Splitwise clone built with **.NET 8 Web API** and **Angular (Zoneless + Signals)**.
> This document serves as the persistent project blueprint across different machines. Update the checklist as tasks are completed.

---

## 1. Tech Stack & Environment Setup

| Layer | Technology | Key Details |
| :--- | :--- | :--- |
| **Backend** | .NET 8 Web API | Clean Architecture (`Api`, `Application`, `Infra`, `Domain`) |
| **Database** | SQL Server Express | EF Core 8 Code-First, Database: `SplitBroDb` |
| **Frontend** | Angular 18+ (Zoneless) | Standalone Components, Signals (`signal()`), SCSS |
| **API Proxy** | Angular Dev Server Proxy | `frontend/proxy.conf.json` forwards `/api` ➔ `http://localhost:5149` |
| **Backend Port** | `http://localhost:5149` | `launchSettings.json` HTTP profile |
| **Frontend Port** | `http://localhost:4200` | `npm start` |

---

## 2. Overall Progress Tracker

- [x] **Milestone 1: Groups & Members Management** (100% Completed)
- [ ] **Milestone 2: Expenses & Splitting (Single-Payer)** (Next Focus)
- [ ] **Milestone 3: Live Group Balances & Scorecard**
- [ ] **Milestone 4: Settle Up & Debt Clearance**
- [ ] **Milestone 5: Multi-Payer Bill Splitting**
- [ ] **Milestone 6: Active User Switcher & Polishing**

---

## 3. UI/UX Layout Architecture (The 3-Zone Blueprint)

The entire desktop application is anchored by a persistent **3-Zone Layout** that never leaves the screen:

```
┌────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ TOP NAVBAR: [SplitBro Logo]                                             Logged in as: [Susi (1) ▼]     │
├──────────────────────────┬─────────────────────────────────────────────────┬───────────────────────────┤
│ ZONE 1: LEFT SIDEBAR     │ ZONE 2: CENTER WORKSPACE (THE FEED)             │ ZONE 3: RIGHT PANEL       │
│                          │                                                 │                           │
│ [ GROUPS ]               │ [ Group Header ]                                │ [ GROUP BALANCES ]        │
│ • 🏖️ Goa Trip (Selected) │   🏖️ Goa Trip (7 members)                       │ • Simplify debts is ON    │
│ • 🏠 At SHA Home         │   [ + Add an Expense ]   [ 💳 Settle Up ]       │ • 🟢 Asker: gets ₹2,181   │
│ • 🚗 Brogrammers         │                                                 │ • 🔴 Susi (You): owes ₹515│
│                          │ ── AUGUST 2026 ──────────────────────────────── │ • ⚪ Vishnu: settled up   │
│ [ + Create New Group ]   │ [31] 🍕 Pizza Party       ₹600 (you lent ₹400)  │                           │
│                          │ [15] 🚗 Taxi Fare         ₹300 (you owe ₹100)   │ [ ⚙️ Manage Members ]     │
│                          │ [02] 🛒 Snacks            ₹200 (not involved)   │                           │
└──────────────────────────┴─────────────────────────────────────────────────┴───────────────────────────┘
```

### Visual & Behavioral Rules
1. **Three-Color Meaning:**
   * 🟢 **Green:** Positive / Money coming to you (*"you lent"*, *"gets back"*).
   * 🔴 **Orange/Red:** Negative / Money you need to pay (*"you owe"*, *"you borrowed"*).
   * ⚪ **Grey:** Neutral / Settled (*"settled up"*, *"not involved"*).
2. **"Not Involved" Visual Cue:**
   * When an expense occurred in the group but you were not part of the split, the card has a **diagonal grey striped pattern** and displays **"not involved"**.
3. **Action Dialogs as Focused Modals:**
   * Only two actions trigger popup dialogs: **`Add an expense`** and **`Settle up`**. Everything else updates in-place.

---

## 4. Detailed Milestone Plans

---

### Milestone 1: Groups & Members Management
> **Status:** ✅ Completed & Verified

- [x] **Backend Domain Models:** `Group`, `GroupMember` composite key (`GroupId`, `UserId`).
- [x] **Database Setup:** EF Core migration `InitialCreate` executed on SQL Server `SplitBroDb`.
- [x] **Repository Layer:** `GroupRepository` implements `IGroupRepository` (`CreateGroupWithMemberAsync`, `GetGroupAllAsync`, `GetGroupByIdAsync`, `AddMemberAsync`, `GetMembersAsync`, `RemoveMemberAsync`).
- [x] **Application Services:** `GroupManagerService` with business validation (minimum 1 member rule, user existence checks).
- [x] **API Endpoints:**
  - `GET /api/group` (List all groups)
  - `POST /api/group` (Create group with initial creator as member)
  - `GET /api/group/{id}/members` (List members with name & email)
  - `POST /api/group/{id}/members/{userId}` (Add member)
  - `DELETE /api/group/{id}/members/{userId}` (Remove member)
- [x] **Frontend Services & Models:** `group.service.ts`, `group.model.ts`, `group-member.model.ts`.
- [x] **Frontend UI:**
  - Two-column layout with SplitBro logo.
  - Group creation form with Signals.
  - Group selection switching RHS view.
  - Member management (view, add by ID, remove).

---

### Milestone 2: Expenses & Splitting (Single-Payer)
> **Status:** 🔲 Next In Queue

#### Goals:
Allow a user to record an expense in a group, pick who paid, split it equally or by exact amounts among group members, and render the chronological expense feed.

#### Backend Plan:
1. **DTOs (`SplitBro.Application/Dto/ExpenseDto.cs`)**:
   - `CreateExpenseRequest`:
     - `int GroupId`
     - `string Name`
     - `int Amount`
     - `int PaidByUserId`
     - `List<int> ParticipantUserIds`
     - `ExpenseSplitType SplitType` (`Equally` or `UnEqually`/`Exact`)
     - `Dictionary<int, int>? ExactAmounts` (optional map of `UserId` ➔ `Amount` for exact splits)
     - `ExpenseCatagory? Category`
   - `ExpenseResponse`:
     - `int Id`, `string Name`, `int Amount`, `int PaidByUserId`, `string PaidByName`, `DateTime AddedTime`
     - `List<ParticipantShareResponse> Participants` (`UserId`, `UserName`, `AmountOwed`)
2. **Repository (`SplitBro.Infra/Repo/ExpenseRepository.cs`)**:
   - `CreateExpenseAsync(Expense expense, List<ExpenseParticipant> participants)` in an atomic transaction.
   - `GetExpensesByGroupIdAsync(int groupId)` with eager loading of `ExpenseParticipants` and `User`.
3. **Application Service (`SplitBro.Application/Service/ExpenseManagerService.cs`)**:
   - Validate group exists.
   - Validate `PaidByUserId` is a group member.
   - Validate all `ParticipantUserIds` are group members.
   - Calculate shares:
     - **Equally:** `share = Amount / participantCount`. Assign remainder if not divisible.
     - **Exact:** Verify `sum(exactAmounts) == Total Amount`.
4. **API Controller (`SplitBro.Api/Controllers/ExpenseController.cs`)**:
   - `POST /api/group/{groupId}/expenses` ➔ Create expense.
   - `GET /api/group/{groupId}/expenses` ➔ List expenses for group.

#### Frontend Plan:
1. **Frontend Service (`expense.service.ts`)**:
   - Implement `getExpenses(groupId: number): Observable<ExpenseResponse[]>`.
   - Implement `createExpense(groupId: number, request: CreateExpenseRequest): Observable<ExpenseResponse>`.
2. **"Add an Expense" Modal (`add-expense-modal`)**:
   - **Main Modal:**
     - Category icon selector (food, taxi, shopping, general).
     - Description input (*"Dinner"*).
     - Large amount input with currency symbol (*"₹ 0.00"*).
     - Natural sentence line: `Paid by [You ▼] and split [Equally ▼] (₹200/person)`.
     - Date picker (defaults to today).
   - **Docked "Choose Split Options" Side Panel:**
     - Split mode toolbar: `=` (Equal) and `1.23` (Exact amounts).
     - Member checklist: Avatar + Name + Checkbox.
     - Live calculation text updating in real time when checkboxes change.
3. **Center Expense Feed**:
   - Group expenses by Month (e.g. `SEPTEMBER 2026`, `AUGUST 2026`).
   - Cards showing:
     - Category icon + Date badge.
     - Expense title.
     - Summary text:
       - *"You paid ₹600, lent ₹400"* (Green)
       - *"Bob paid ₹300, you borrowed ₹100"* (Orange)
       - *"Bob paid ₹200, not involved"* (Grey striped)

---

### Milestone 3: Live Group Balances & Scorecard
> **Status:** 🔲 Upcoming

#### Goals:
Calculate the net debt matrix for every member in the group and show the live scorecard in the right panel.

#### Backend Plan:
1. **Balance Engine Logic (`SplitBro.Application`)**:
   - Query all expenses and settlements for a group.
   - For each member: `NetBalance = TotalAmountPaid - TotalAmountConsumed`.
   - Positive balance = Member gets back money.
   - Negative balance = Member owes money.
   - Zero balance = Settled up.
2. **Pairwise Debt Resolution**:
   - Compute pairwise debt: `User A owes User B ₹X`.
3. **API Endpoint**:
   - `GET /api/group/{groupId}/balances` ➔ Returns net balances and pairwise who-owes-whom list.

#### Frontend Plan:
1. **Right Panel `GROUP BALANCES` Card**:
   - "Simplify debts is ON" toggle switch.
   - Member balance list with avatars:
     - 🟢 *Asker Parvez: gets back ₹2,181.68*
     - 🔴 *Susi (You): owes ₹515.01*
     - ⚪ *Vishnu: settled up*

---

### Milestone 4: Settle Up & Debt Clearance
> **Status:** 🔲 Upcoming

#### Goals:
Allow a user to record cash or direct transfers between two members, reducing outstanding debts to ₹0.

#### Backend Plan:
1. **Domain & Entity Verification:**
   - Ensure `Settlement` table has: `Id`, `GroupId`, `UserPaidId`, `UserReceivedId`, `Amount`, `SettledAt`.
2. **Repository & Service (`SettlementRepository`, `SettlementManagerService`)**:
   - `RecordSettlementAsync(Settlement settlement)`
   - Reduce outstanding balance.
3. **API Controller (`SettlementController`)**:
   - `POST /api/group/{groupId}/settlements`
   - `GET /api/group/{groupId}/settlements`

#### Frontend Plan:
1. **"Settle Up" Modal**:
   - Payer dropdown: *"Who paid?"*
   - Receiver dropdown: *"Who received?"*
   - Amount input (pre-filled with exact debt owed).
   - "Record Cash / Direct Payment" button.
2. **Feed Integration**:
   - Settle up payments appear in the center feed with the scales ⚖️ / cash 💵 icon.

---

### Milestone 5: Multi-Payer Bill Splitting
> **Status:** 🔲 Upcoming (Real-world enhancement)

#### Goals:
Support scenarios where multiple people contribute payments toward a single large bill (e.g. Alice paid ₹6,000 and Bob paid ₹4,000 on a ₹10,000 rental deposit).

#### Implementation Details:
1. Backend: Allow `CreateExpenseRequest` to accept `List<PayerContribution> Payers` instead of a single `PaidByUserId`.
2. Split logic: Validate `sum(contributions) == TotalAmount`.
3. UI: Add a "Multiple people paid" option inside the payer selector.

---

### Milestone 6: Active User Switcher & Final Polish
> **Status:** 🔲 Upcoming

#### Goals:
Provide seamless multi-user testing without opening incognito windows, plus visual polishing.

1. **Top Bar User Switcher**:
   - Dropdown in navbar: `Active User: [Susidharan (1) | Asker (2) | Vishnu (3)]`.
   - Changing active user re-evaluates the feed perspective instantly (*"you paid"* vs *"you borrowed"*).
2. **SCSS Styling & Animations**:
   - Match Splitwise’s exact teal color palette (`#5bc5a7`), font weights, and spacing.

---

## 5. API Endpoints Reference Table

| Method | Endpoint | Description | Status |
| :--- | :--- | :--- | :---: |
| `GET` | `/api/group` | List all groups | ✅ Done |
| `POST` | `/api/group` | Create a new group | ✅ Done |
| `GET` | `/api/group/{id}` | Get single group details | ✅ Done |
| `GET` | `/api/group/{id}/members` | Get members of group | ✅ Done |
| `POST` | `/api/group/{id}/members/{userId}` | Add member to group | ✅ Done |
| `DELETE` | `/api/group/{id}/members/{userId}` | Remove member from group | ✅ Done |
| `GET` | `/api/group/{id}/expenses` | Get expenses feed for group | 🔲 M2 |
| `POST` | `/api/group/{id}/expenses` | Record a new expense | 🔲 M2 |
| `DELETE` | `/api/expenses/{expenseId}` | Delete an expense | 🔲 M2 |
| `GET` | `/api/group/{id}/balances` | Get live member balance scorecard | 🔲 M3 |
| `POST` | `/api/group/{id}/settlements` | Record a settlement payment | 🔲 M4 |
| `GET` | `/api/user` | Get all users (for user switcher) | 🔲 M6 |

---

## 6. How to Run & Verify

1. **Start Backend API:**
   ```powershell
   cd s:\Susi\SplitBro\backend\src\SplitBro.Api
   dotnet run
   # Listening on http://localhost:5149
   ```

2. **Start Frontend Application:**
   ```powershell
   cd s:\Susi\SplitBro\frontend
   npm start
   # Listening on http://localhost:4200 (proxies /api to :5149)
   ```

3. **Database Verification:**
   - Connect SSMS to `localhost\SQLEXPRESS`.
   - Database: `SplitBroDb`.
