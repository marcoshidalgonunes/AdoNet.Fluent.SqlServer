namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface IPrepareService
{
    void Execute(int departmentId, int[] businessEntityID);

    Task ExecuteAsync(int departmentId, int[] businessEntityID);
}
