using Domain.Abstraction;
using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraStructure.Impelementation
{
    //    internal class WriteUnitOfWork : IWriteUnitOfWork
    //    {
    //        IDbContextTransaction _transaction;
    //        WriteSysDbContext _Wcontext;

    //        public WriteUnitOfWork(WriteSysDbContext wcontext)
    //        {
    //            _Wcontext = wcontext;
    //        }

    //        public async Task BeginTransiaction()
    //        {
    //            // Check if a transaction is already in progress
    //            if (_transaction != null)
    //            {
    //                throw new InvalidOperationException("A transaction is already in progress.");
    //            }
    //            // Start a new transaction
    //            // transaction is used to ensure that all operations within the transaction scope are atomic
    //            //this will ensure that either all operations succeed or none do, maintaining data integrity.
    //            _transaction = await _Wcontext.Database.BeginTransactionAsync();
    //        }

    //        public async Task CommitAsync()
    //        {
    //            if (_transaction == null)
    //            {
    //                throw new InvalidOperationException("No transaction in progress to commit.");
    //            }
    //            try
    //            {
    //                await _transaction.CommitAsync();
    //            }
    //            catch (Exception ex)
    //            {
    //                await RollbackAsync();
    //                throw new InvalidOperationException("Failed to commit transaction.", ex);
    //            }
    //            finally
    //            {
    //                await DisposeTransactionAsync();
    //            }
    //        }

    //        public void Dispose()
    //        {
    //            //if (_transaction != null)
    //            //{
    //            //    _transaction.Dispose();
    //            //    _transaction = null;
    //            //}
    //            //_context?.Dispose();
    //            Dispose(true);
    //            GC.SuppressFinalize(this);
    //        }
    //        protected virtual void Dispose(bool disposing)
    //        {
    //            if (disposing)
    //            {
    //                if (_transaction != null)
    //                {
    //                    _transaction.Dispose();
    //                    _transaction = null;
    //                }
    //                _Wcontext?.Dispose();
    //            }
    //        }     


    //        public async Task RollbackAsync()
    //        {
    //            if (_transaction == null)
    //            {
    //                throw new InvalidOperationException("No transaction in progress to rollback.");
    //            }
    //            try
    //            {
    //                await _transaction.RollbackAsync();
    //            }
    //            catch (Exception ex)
    //            {
    //                throw new InvalidOperationException("Failed to rollback transaction.", ex);
    //            }
    //            finally
    //            {
    //                await DisposeTransactionAsync();
    //            }
    //        }

    //        public async Task SaveChangesAsync()
    //        {
    //            try
    //            {
    //                await _Wcontext.SaveChangesAsync();
    //                if (_transaction != null)
    //                {
    //                    await CommitAsync();
    //                }
    //            }
    //            catch (DbUpdateException ex)
    //            {
    //                if (_transaction != null)
    //                {
    //                    await RollbackAsync();
    //                }
    //                // Extract entity details from the exception
    //                var errorDetails = ex.Entries.Select(e => $"Entity: {e.Entity.GetType().Name}, State: {e.State}");
    //                throw new InvalidOperationException($"Failed to save changes: {string.Join("; ", errorDetails)}", ex);
    //            }
    //        }
    //        private async Task DisposeTransactionAsync()
    //        {
    //            if (_transaction != null)
    //            {
    //                await _transaction.DisposeAsync();
    //                _transaction = null;
    //            }
    //        }

    //        ~WriteUnitOfWork()
    //        {
    //            Dispose(false);
    //        }
    //    }

    public class WriteUnitOfWork : IWriteUnitOfWork, IDisposable
    {
        private IDbContextTransaction _transaction;
        private readonly WriteSysDbContext _context;

        public WriteUnitOfWork(WriteSysDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var details = ex.Entries.Select(e => $"{e.Entity.GetType().Name}:{e.State}");
                throw new InvalidOperationException($"Failed to save changes: {string.Join(", ", details)}", ex);
            }
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to commit.");

            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction to rollback.");

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

       
    }

}
