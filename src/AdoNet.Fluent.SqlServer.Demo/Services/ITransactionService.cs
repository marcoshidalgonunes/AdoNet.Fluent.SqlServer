using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface ITransactionService<TEntity>
    where TEntity : IEntity
{
    int Execute(TEntity entity);   

    Task ExecuteAsync(TEntity entity);
}
