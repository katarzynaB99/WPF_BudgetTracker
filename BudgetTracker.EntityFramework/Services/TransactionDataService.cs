﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Models;
using BudgetTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.EntityFramework.Services
{
    public class TransactionDataService : GenericDataService<Transaction>, ITransactionService
    {
        private readonly BudgetTrackerDbContextFactory _contextFactory;

        public TransactionDataService(BudgetTrackerDbContextFactory contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Transaction>> GetRecentTransactions(int userId, int numberOfTransactions)
        {
            await using var context = _contextFactory.CreateDbContext();
            IEnumerable<Transaction> entities = await context.Transactions
                .Where(e => e.Account.User.Id == userId)
                .OrderBy(e => e.TransactionDate)
                .Take(numberOfTransactions)
                .ToListAsync();
            
            return entities;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCategory(int userId, int categoryId)
        {
            await using var context = _contextFactory.CreateDbContext();
            IEnumerable<Transaction> entities = await context.Transactions
                .Where(e => e.Account.User.Id == userId && e.Category.Id == categoryId)
                .OrderBy(e => e.TransactionDate)
                .ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccount(int accountId)
        {
            await using var context = _contextFactory.CreateDbContext();
            IEnumerable<Transaction> entities = await context.Transactions
                .Where(e => e.Account.Id == accountId)
                .OrderBy(e => e.TransactionDate)
                .ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUser(int userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            IEnumerable<Transaction> entities = await context.Transactions
                .Where(e => e.Account.User.Id == userId)
                .OrderBy(e => e.TransactionDate)
                .ToListAsync();
            return entities;
        }

        public override async Task<Transaction> Create(Transaction entity)
        {
            await using var context = _contextFactory.CreateDbContext();
            IDataService<Account> accountService = new GenericDataService<Account>(_contextFactory);
            IDataService<Category> categoryService = new GenericDataService<Category>(_contextFactory);

            // Update account balance
            var accountToUpdate = await accountService.Get(entity.AccountId);
            accountToUpdate.Balance += entity.Amount;
            await accountService.Update(entity.AccountId, accountToUpdate);

            // Proceed with creating as usual
            var createdEntity = await base.Create(entity);

            createdEntity.Account = accountToUpdate;
            createdEntity.Category = await categoryService.Get(createdEntity.CategoryId);
            return createdEntity;
        }

        public override async Task<Transaction> Update(int id, Transaction entity)
        {
            await using var context = _contextFactory.CreateDbContext();
            IDataService<Account> accountService = new GenericDataService<Account>(_contextFactory);

            // Update account balances based on transaction change
            var oldTransaction = await Get(id);

            // If account changed
            if (oldTransaction.Account.Id != entity.Account.Id)
            {
                oldTransaction.Account.Balance -= oldTransaction.Amount;
                entity.Account.Balance += entity.Amount;

                await accountService.Update(entity.Account.Id, entity.Account);
                await accountService.Update(oldTransaction.Account.Id, oldTransaction.Account);
            }
            // If transaction amount changed
            else if (oldTransaction.Amount != entity.Amount)
            {
                entity.Account.Balance -= oldTransaction.Amount;
                entity.Account.Balance += entity.Amount;
                await accountService.Update(entity.Account.Id, entity.Account);
            }

            // Proceed with updating as usual
            return await base.Update(id, entity);
        }

        public override async Task<bool> Delete(int id)
        {
            await using var context = _contextFactory.CreateDbContext();
            IDataService<Account> accountService = new GenericDataService<Account>(_contextFactory);

            // Update account balance before deleting
            var transaction = await Get(id);
            if (transaction != null)
            {
                var accountToUpdate = await accountService.Get(transaction.AccountId);

                if (accountToUpdate != null)
                {
                    accountToUpdate.Balance -= transaction.Amount;
                    await accountService.Update(transaction.AccountId, accountToUpdate);
                }
                // Proceed with deleting as usual
                return await base.Delete(id);
            }

            return true;
        }
    }
}
