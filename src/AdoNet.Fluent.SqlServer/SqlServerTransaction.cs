using System.Data;
using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Class for transactions execution in SQL Server.
/// </summary>
public sealed class SqlServerTransaction : SqlServerDataObject, ITransaction
{
    private bool _disposedValue;

    private SqlTransaction? _transaction;

    internal SqlServerTransaction(string connectionString) 
        : base(connectionString, ConnectionMode.Transational) { }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    /// <param name="disposing">Flag for dispose in execution.</param>
    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _transaction?.Dispose();

            base.Dispose(disposing);

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Opens connection with database.
    /// </summary>
    /// <exception cref="SqlException" />
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

    /// <summary>
    /// Asynchronously opens connection with database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="SqlException" />
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

    /// <summary>
    /// Performs transaction commit.
    /// </summary>
    /// <exception cref="InvalidOperationException" />"
    public void Commit()
    {
        _transaction?.Commit();
    }

    /// <summary>
    /// Performs transaction rollback.
    /// </summary>
    /// <exception cref="InvalidOperationException" />"
    public void Rollback()
    {
        _transaction?.Rollback();
    }

    #endregion
}
