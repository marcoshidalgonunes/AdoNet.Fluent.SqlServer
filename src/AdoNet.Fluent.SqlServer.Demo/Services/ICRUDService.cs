using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface ICRUDService<TEntity> 
    where TEntity : IEntity
{
    int Create(TEntity entity);

    Task<int> CreateAsync(TEntity entity);

    void Delete(int id);

    Task DeleteAsync(int id);

    TEntity Read(int id);

    Task<TEntity> ReadAsync(int id);

    void Update(TEntity entity);

    Task UpdateAsync(TEntity entity);
}
