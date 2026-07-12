using SlientMoon.Application.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;

public interface IUow : IDisposable
{
    IPomodoroRepository PomodoroRepository { get; }
    IUserRepository UserRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}