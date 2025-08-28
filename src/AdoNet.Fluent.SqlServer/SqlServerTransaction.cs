using System.Data;
using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer;

public sealed class SqlServerTransaction : SqlServerDataObject, ITransaction
{
    private bool _disposedValue;

    private SqlTransaction? _transaction;

    internal SqlServerTransaction(string connectionString) 
        : base(connectionString, ConnectionMode.Transational) { }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _transaction?.Dispose();

            base.Dispose(disposing);

            _disposedValue = true;
        }
    }

    protected override void OpenConnection()
    {
        Connection ??= new SqlConnection();

        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
            _transaction = Connection.BeginTransaction();
        }

        Command.Connection = _transaction?.Connection;
        Command.Transaction = _transaction;
    }

    protected override async Task OpenConnectionAsync(CancellationToken cancellationToken)
    {
        Connection ??= new SqlConnection();

        if (Connection.State != ConnectionState.Open)
        {
            await Connection.OpenAsync(cancellationToken);
            _transaction = await Task.Run(() => Connection.BeginTransaction(), cancellationToken);
        }

        Command.Connection = _transaction?.Connection;
        Command.Transaction = _transaction;
    }

    #region ITransaction members

    public void Commit()
    {
        _transaction?.Commit();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }

    #endregion
}
