using Microsoft.EntityFrameworkCore.Storage;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Threading.Tasks;
using System.Threading;
using System;

public class Uow : IUow
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IPomodoroRepository PomodoroRepository { get; }
    public IUserRepository UserRepository { get; }

    public Uow(
        AppDbContext context,
        IPomodoroRepository pomodoroRepository,
        IUserRepository userRepository)
    {
        _context = context;
        PomodoroRepository = pomodoroRepository;
        UserRepository = userRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) return;
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }

}