namespace AdoNet.Fluent.SqlServer.Demo.Models;

internal class Hierarchy : Entity
{
    public required int RecursionLevel { get; set; }

    public required string OrganizationNode { get; set; }

    public required string ManagerFirstName { get; set; }

    public required string ManagerLastName { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}
