namespace AdoNet.Fluent.SqlServer.Demo.Models;

internal sealed class Department : Entity
{
    public string? Name { get; set; }

    public string? GroupName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public List<Shift>? Shifts { get; set; }
}
