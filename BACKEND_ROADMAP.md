# SplitBro — Backend Architecture Roadmap & Inventory

> **Purpose:** Detailed backend inventory mapping every file, DTO, repository, service, and controller to its respective milestone.
> Use this as your step-by-step checklist when working across systems.

---

## 1. Master File Status Matrix

| File Path | Layer | Milestone | Current Status | Description |
| :--- | :--- | :---: | :---: | :--- |
| `Domain/Group.cs` | Domain | M1 | ✅ Done | Group entity |
| `Domain/GroupMember.cs` | Domain | M1 | ✅ Done | Join table entity (`GroupId`, `UserId`) |
| `Domain/User.cs` | Domain | M1 | ✅ Done | User entity |
| `Domain/Enums/Currency.cs` | Domain | M1 | ✅ Done | Currency enum (`Rupee`, `USD`, `EUR`) |
| `Domain/Enums/ExpenseCatagory.cs`| Domain | M1 | ✅ Done | Category enum (`Food`, `Travel`, etc.) |
| `Domain/Enums/ExpenseSplitType.cs`| Domain | M1 | ✅ Done | Split modes (`Equally`, `UnEqually`, etc.) |
| `Domain/Expense.cs` | Domain | M2 | 🟡 Ready | Expense entity |
| `Domain/ExpenseParticipant.cs` | Domain | M2 | 🟡 Ready | Expense participant join entity |
| `Domain/Settlement.cs` | Domain | M4 | 🟡 Ready | Settlement entity (needs `GroupId` in M4) |
| `Application/Dto/GroupDto.cs` | Application | M1 | ✅ Done | Group & Member DTOs |
| `Application/Dto/UserDto.cs` | Application | M1 | ✅ Done | User DTOs |
| `Application/Dto/ExpenseDto.cs` | Application | **M2** | 🔲 **To Create** | Expense request & response DTOs |
| `Application/Dto/BalanceDto.cs` | Application | **M3** | 🔲 **To Create** | Group balance & debt matrix DTOs |
| `Application/Dto/SettlementDto.cs`| Application | **M4** | 🔲 **To Create** | Settlement payment DTOs |
| `Application/RepoInterfaces/IGroupRepository.cs` | Application | M1 | ✅ Done | Group repo contract |
| `Application/RepoInterfaces/IUserRepository.cs` | Application | M1 | ✅ Done | User repo contract |
| `Application/RepoInterfaces/IExpenseRepository.cs` | Application | **M2** | 🔲 **Empty Stub** | Expense repo contract |
| `Application/RepoInterfaces/ISettlementRepository.cs`| Application | **M4** | 🔲 **Empty Stub** | Settlement repo contract |
| `Application/Service/GroupManagerService.cs` | Application | M1 | ✅ Done | Group & Member business service |
| `Application/Service/UserManagerService.cs` | Application | M1 | ✅ Done | User business service |
| `Application/Service/ExpenseManagerService.cs` | Application | **M2** | 🔲 **Empty Stub** | Expense & split calculations service |
| `Application/Service/BalanceCalculationService.cs`| Application | **M3** | 🔲 **To Create** | Net debt & balance matrix service |
| `Application/Service/SettlementManagerService.cs` | Application | **M4** | 🔲 **Empty Stub** | Settlement recording service |
| `Infra/Data/SplitAppDbContext.cs` | Infra | M1 | ✅ Done | EF Core DbContext with mappings |
| `Infra/Repo/GroupRepository.cs` | Infra | M1 | ✅ Done | Group EF Core data queries |
| `Infra/Repo/UserRepository.cs` | Infra | M1 | ✅ Done | User EF Core data queries |
| `Infra/Repo/ExpenseRepository.cs` | Infra | **M2** | 🔲 **Empty Stub** | Expense EF Core queries |
| `Infra/Repo/SettlementRepository.cs` | Infra | **M4** | 🔲 **Empty Stub** | Settlement EF Core queries |
| `Api/Controllers/GroupController.cs` | API | M1 | ✅ Done | Group & Member endpoints |
| `Api/Controllers/UserController.cs` | API | M1 | ✅ Done | User CRUD endpoints |
| `Api/Controllers/ExpenseController.cs` | API | **M2** | 🔲 **Empty Stub** | Expense endpoints |
| `Api/Controllers/SettlementController.cs` | API | **M4** | 🔲 **Empty Stub** | Settlement endpoints |
| `Api/Program.cs` | API | M1 | ✅ Done | DI, CORS, Middleware, DB pipeline |

---

## 2. Milestone-by-Milestone Implementation Blueprint

---

### MILESTONE 2: Expenses & Splitting (Single-Payer Engine)
> **Goal:** Create an expense in a group, pick who paid, split it equally or by exact amounts among members, and fetch group expense history.

#### 1. DTOs to Create: `SplitBro.Application/Dto/ExpenseDto.cs`
```csharp
namespace SplitBro.Application.Dto
{
    public class ExpenseDto
    {
        // 1. Create Expense Request Payload
        public class CreateExpenseRequest
        {
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
            public int PaidByUserId { get; set; }
            public List<int> ParticipantUserIds { get; set; } = new();
            public ExpenseSplitType SplitType { get; set; } = ExpenseSplitType.Equally;
            public Dictionary<int, int>? ExactAmounts { get; set; } // Optional: UserId -> Amount for Exact splits
            public ExpenseCatagory? Category { get; set; }
            public Currency Currency { get; set; } = Currency.Rupee;
        }

        // 2. Expense Response for Feeds
        public class ExpenseResponse
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Amount { get; set; }
            public int GroupId { get; set; }
            public int PaidByUserId { get; set; }
            public string PaidByName { get; set; } = string.Empty;
            public ExpenseCatagory? Category { get; set; }
            public Currency Currency { get; set; }
            public DateTime AddedTime { get; set; }
            public List<ExpenseParticipantResponse> Participants { get; set; } = new();
        }

        // 3. Participant Share Response
        public class ExpenseParticipantResponse
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public int AmountOwed { get; set; }
            public ExpenseSplitType SplitType { get; set; }
        }
    }
}
```

#### 2. Interface to Update: `SplitBro.Application/RepoInterfaces/IExpenseRepository.cs`
* `Task<Expense> CreateExpenseAsync(Expense expense, List<ExpenseParticipant> participants);`
* `Task<List<Expense>> GetExpensesByGroupIdAsync(int groupId);`
* `Task<Expense?> GetExpenseByIdAsync(int expenseId);`
* `Task DeleteExpenseAsync(int expenseId);`

#### 3. Repository Implementation: `SplitBro.Infra/Repo/ExpenseRepository.cs`
* Eager load participants and users:
  ```csharp
  _context.Expenses
      .Include(e => e.ExpenseParticipants)
          .ThenInclude(ep => ep.UserReceived)
      .Where(e => e.GroupId == groupId)
      .OrderByDescending(e => e.AddedTime)
      .ToListAsync();
  ```
* Save `Expense` and `ExpenseParticipants` within an execution strategy or transaction.

#### 4. Service Implementation: `SplitBro.Application/Service/ExpenseManagerService.cs`
* **Validation Rules:**
  1. Verify group exists via `_groupRepository.GetGroupByIdAsync(groupId)`.
  2. Verify `PaidByUserId` is an active member of the group.
  3. Verify all `ParticipantUserIds` are members of the group.
  4. Amount must be greater than 0.
* **Split Calculations:**
  * **Equally:** `share = TotalAmount / participantCount`. Assign any rounding remainder to the first participant.
  * Generate `ExpenseParticipant` records linking `UserPaidId` and `UserReceivedId`.

#### 5. Controller Implementation: `SplitBro.Api/Controllers/ExpenseController.cs`
* `[HttpPost("api/group/{groupId:int}/expenses")]` ➔ `CreateExpense(int groupId, [FromBody] CreateExpenseRequest request)`
* `[HttpGet("api/group/{groupId:int}/expenses")]` ➔ `GetExpensesByGroup(int groupId)`
* `[HttpDelete("api/expenses/{expenseId:int}")]` ➔ `DeleteExpense(int expenseId)`

---

### MILESTONE 3: Live Group Balances & Debt Matrix
> **Goal:** Compute "Who owes whom how much" in a group and supply the scorecard for the Right Panel.

#### 1. DTOs to Create: `SplitBro.Application/Dto/BalanceDto.cs`
```csharp
namespace SplitBro.Application.Dto
{
    public class BalanceDto
    {
        // Net balance per member in group
        public class MemberBalanceResponse
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
            public int TotalPaid { get; set; }
            public int TotalOwed { get; set; }
            public int NetBalance { get; set; } // TotalPaid - TotalOwed (Positive = gets back, Negative = owes)
        }

        // Pairwise debt (e.g. "User A owes User B ₹300")
        public class PairwiseDebtResponse
        {
            public int DebtorId { get; set; }
            public string DebtorName { get; set; } = string.Empty;
            public int CreditorId { get; set; }
            public string CreditorName { get; set; } = string.Empty;
            public int Amount { get; set; }
        }

        // Full group balance package
        public class GroupBalanceSummaryResponse
        {
            public int GroupId { get; set; }
            public List<MemberBalanceResponse> MemberBalances { get; set; } = new();
            public List<PairwiseDebtResponse> Debts { get; set; } = new();
        }
    }
}
```

#### 2. Service Implementation: Balance Calculation Engine
* **Location:** Add method `GetGroupBalancesAsync(int groupId)` in `GroupManagerService.cs` or create `BalanceCalculationService.cs`.
* **Math Logic:**
  1. Fetch all expenses for the group with their participants.
  2. Fetch all settlements for the group.
  3. Calculate net sum:
     * For each expense: Payer gets `+ (Total - OwnShare)`, Participants get `- (TheirShare)`.
     * For each settlement: Payer gets `+ SettledAmount`, Receiver gets `- SettledAmount`.
  4. Compute pairwise resolution algorithm (e.g., standard greedy algorithm to minimize transactions).

#### 3. Controller Endpoint:
* `[HttpGet("api/group/{groupId:int}/balances")]` inside `GroupController.cs`.

---

### MILESTONE 4: Settlements & Debt Clearance
> **Goal:** Record direct transfers (Cash / UPI) between two users to pay off debts.

#### 1. Entity Update: `SplitBro.Domain/Settlement.cs`
* Add `public int GroupId { get; set; }` and navigation property `public Group Group { get; set; }`.
* Run migration: `dotnet ef migrations add AddGroupIdToSettlement`.

#### 2. DTOs to Create: `SplitBro.Application/Dto/SettlementDto.cs`
```csharp
namespace SplitBro.Application.Dto
{
    public class SettlementDto
    {
        public class RecordSettlementRequest
        {
            public int PayerUserId { get; set; }
            public int ReceiverUserId { get; set; }
            public int Amount { get; set; }
            public Currency Currency { get; set; } = Currency.Rupee;
        }

        public class SettlementResponse
        {
            public int Id { get; set; }
            public int GroupId { get; set; }
            public int PayerUserId { get; set; }
            public string PayerUserName { get; set; } = string.Empty;
            public int ReceiverUserId { get; set; }
            public string ReceiverUserName { get; set; } = string.Empty;
            public int Amount { get; set; }
            public DateTime SettledAt { get; set; }
        }
    }
}
```

#### 3. Interface & Repo: `ISettlementRepository.cs` & `SettlementRepository.cs`
* `Task<Settlement> CreateSettlementAsync(Settlement settlement);`
* `Task<List<Settlement>> GetSettlementsByGroupIdAsync(int groupId);`

#### 4. Service & Controller:
* `SettlementManagerService.cs`: Validates payer and receiver belong to group, amount > 0.
* `SettlementController.cs`:
  * `POST /api/group/{groupId}/settlements`
  * `GET /api/group/{groupId}/settlements`

---

### MILESTONE 5: Multi-Payer Bill Splitting
> **Goal:** Allow multiple users to contribute payments toward a single large bill.

#### 1. DTO Updates in `ExpenseDto.cs`:
* Add `List<PayerContributionRequest> Payers` to `CreateExpenseRequest`:
  ```csharp
  public class PayerContributionRequest
  {
      public int UserId { get; set; }
      public int AmountPaid { get; set; }
  }
  ```

#### 2. Service Logic:
* Validate: `sum(Payers.AmountPaid) == TotalExpenseAmount`.
* Record multi-payer ledger in `ExpenseParticipants`.

---

### MILESTONE 6: User Directory API (For Top-Bar Switcher)
> **Goal:** Support switching logged-in users in the frontend for multi-user simulation.

#### 1. Repository & Service Updates:
* In `IUserRepository.cs` & `UserRepository.cs`: Add `Task<List<User>> GetAllUsersAsync();`.
* In `UserManagerService.cs`: Add `Task<List<UserResponse>> GetAllUsersAsync();`.

#### 2. Controller Update in `UserController.cs`:
* Add `[HttpGet] GetAllUsers()` ➔ `GET /api/user`.
