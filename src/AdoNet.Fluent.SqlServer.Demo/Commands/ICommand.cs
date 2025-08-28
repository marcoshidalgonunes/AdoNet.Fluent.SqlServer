namespace AdoNet.Fluent.SqlServer.Demo.Commands;

internal interface ICommand
{
    void Execute();

    Task ExecuteAsync();
}
