namespace MusicStore.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);
}
