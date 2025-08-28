using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface IReadService<TEntity> 
    where TEntity : IEntity
{
    List<TEntity> Read(int id);

    Task<List<TEntity>> ReadAsync(int id);
}
