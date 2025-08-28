using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface IMARSService<TEntity> 
    where TEntity : IEntity
{
    List<TEntity> Read(string value);

    Task<List<TEntity>> ReadAsync(string value);
}
