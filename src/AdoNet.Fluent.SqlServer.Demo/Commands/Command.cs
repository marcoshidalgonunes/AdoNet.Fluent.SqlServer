using AdoNet.Fluent.SqlServer.Demo.Models;
using Microsoft.Extensions.Logging;

namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal delegate int? Create(IEntity entity);

internal delegate Task<int?> CreateAsync(IEntity entity);

internal delegate void Delete(int id);

internal delegate Task DeleteAsync(int id);

internal delegate IEntity Read(int id);

internal delegate Task<IEntity> ReadAsync(int id);

internal delegate List<IEntity> ReadById(int id);

internal delegate Task<List<IEntity>> ReadByIdAsync(int id);

internal delegate List<IEntity> ReadByValue(string value);

internal delegate Task<List<IEntity>> ReadByValueAsync(string value);

internal delegate void Update(IEntity entity);

internal delegate Task UpdateAsync(IEntity entity);

internal abstract class Command(ILogger logger) : ICommand
{
    protected ILogger Logger { get; private set; } = logger;

    public abstract void Execute();

    public abstract Task ExecuteAsync();
}
