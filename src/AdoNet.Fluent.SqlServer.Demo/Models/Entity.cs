namespace AdoNet.Fluent.SqlServer.Demo.Models;

internal interface IEntity
{
    int Id { get; set; }
}

internal abstract class Entity : IEntity
{
    public int Id { get; set; }
}
