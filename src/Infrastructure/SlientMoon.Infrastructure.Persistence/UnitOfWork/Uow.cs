using Microsoft.EntityFrameworkCore.Storage;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Infrastructure.Persistence.Contexts;
using SlientMoon.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class Uow : IUow
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IPomodoroRepository PomodoroRepository { get; }
    public ITopicRepository TopicRepository { get; }
    public IUserRepository UserRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IReminderRepository ReminderRepository { get; }
    public ICourseRepository CourseRepository { get; }
    public IFavoriteRepository FavoriteRepository { get; }
    public IDailyThoughtRepository DailyThoughtRepository { get; }

    public Uow(
        AppDbContext context,
        IPomodoroRepository pomodoroRepository,
        IUserRepository userRepository,
        ITopicRepository topicRepository,
        ICourseRepository courseRepository,
        IDailyThoughtRepository dailyThoughtRepository,
        ICategoryRepository categoryRepository,
        IReminderRepository reminderRepository,
        IFavoriteRepository favoriteRepository)
    {
        _context = context;
        TopicRepository = topicRepository;
        CourseRepository = courseRepository;
        DailyThoughtRepository = dailyThoughtRepository;
        PomodoroRepository = pomodoroRepository;
        UserRepository = userRepository;
        CategoryRepository = categoryRepository;
        ReminderRepository = reminderRepository;
        FavoriteRepository = favoriteRepository;
    }

    public IGenericRepository<T> GenericRepository<T>() where T : class
    {
        return (IGenericRepository<T>)_repositories.GetOrAdd(
            typeof(T),
            _ => new GenericRepository<T>(_context)
        );
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