using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using SplitBro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SplitBro.Infra.Data
{
    public class SplitAppDbContext : DbContext
    {
        // constructor
        public SplitAppDbContext(DbContextOptions<SplitAppDbContext> option) : base(option)
        {

        }

        //tables
        public DbSet<User> Users { get; set; }
        public DbSet<Domain.Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMemebers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseParticipant> ExpenseParticipants { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // User ↔ GroupMember
            // User 1 ───── * GroupMember
            // =========================================================

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // Group ↔ GroupMember
            // Group 1 ───── * GroupMember
            // =========================================================

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // Group ↔ Expense
            // Group 1 ───── * Expense
            // =========================================================

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Group)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // Expense ↔ ExpenseParticipant
            // Expense 1 ───── * ExpenseParticipant
            // =========================================================

            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.Expense)
                .WithMany(e => e.ExpenseParticipants)
                .HasForeignKey(ep => ep.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // USER → EXPENSE PARTICIPANT (GIVER)
            // One User can give/pay in many ExpenseParticipants
            // =========================================================

            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.UserPaid)
                .WithMany(u => u.ExpenseParticipantsPaid)
                .HasForeignKey(ep => ep.UserPaidId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // USER → EXPENSE PARTICIPANT (RECEIVER)
            // One User can receive in many ExpenseParticipants
            // =========================================================

            modelBuilder.Entity<ExpenseParticipant>()
                .HasOne(ep => ep.UserReceived)
                .WithMany(u => u.ExpenseParticipantsReceived)
                .HasForeignKey(ep => ep.UserReceivedId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // USER → SETTLEMENT (PAID BY)
            // One User can make many settlements
            // =========================================================

            modelBuilder.Entity<Settlement>()
                .HasOne(s => s.UserPay)
                .WithMany(u => u.SettlementsPaid)
                .HasForeignKey(s => s.UserPaid)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // USER → SETTLEMENT (RECEIVED BY)
            // One User can receive many settlements
            // =========================================================

            modelBuilder.Entity<Settlement>()
                .HasOne(s => s.UserReceive)
                .WithMany(u => u.SettlementsReceived)
                .HasForeignKey(s => s.UserReceived)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // GROUP MEMBER PRIMARY KEY
            // Same User cannot be added twice to same Group
            // =========================================================

            modelBuilder.Entity<GroupMember>()
                .HasKey(gm => new
                {
                    gm.GroupId,
                    gm.UserId
                });

        }
    }
}

