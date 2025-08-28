using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Xml;
using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer;

/// <summary>
/// Base class for operations on SQL Server databases.
/// </summary>
public abstract class SqlServerDataObject : DataObject<SqlConnection, SqlCommand, SqlParameter, SqlException>
{
    ///<summary>
    /// Class constructor.
    ///</summary>
    /// <param name="connectionString">Database connection string.</param>
    /// <param name="mode"><see cref="ConnectionMode"/> (normal, with transactios or using MARS).</param>
    protected SqlServerDataObject(string connectionString, ConnectionMode mode)
        : base(connectionString, mode)
    {
        ForeignKeyError = 547;
        PrimaryKeyError = 2627;
        DuplicateKeyError = 2601;

        errorCodes = [ForeignKeyError, DuplicateKeyError, PrimaryKeyError];
    }

    #region Protected/Private Members

    private readonly int storedProcedureError = 50000;
    private readonly HashSet<int> errorCodes;

    /// <summary>
    /// Creates input/output parameter of boolean type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, bool value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Bit, direction);
        par.Value = new SqlBoolean(value);

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of decimal type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="scale">Parameter scale.</param>
    /// <param name="precision">Parameter precision.</param>
    /// <param name="direction">Parameter <seealso cref="Data.ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    protected override void AddInOutParameter(string parameterName, byte precision, byte scale, ParameterDirection direction)
    {
        CheckParameter(parameterName, precision, scale);
        Command.Parameters.Add(CreateParameter(parameterName, direction, precision, scale));
    }

    /// <summary>
    /// Creates input/output parameter of byte type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, byte? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.TinyInt, direction);
        par.Value = value.HasValue ? new SqlByte(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of <see cref="DateTime"/> type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, DateTime? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.DateTime, direction);
        par.Value = value.HasValue ? new SqlDateTime(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of decimal type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="precision">Parameter precision.</param>
    /// <param name="scale">Parameter scale.</param>
    /// <param name="direction">Parameter <seealso cref="Data.ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, decimal? value, byte precision, byte scale, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, direction, precision, scale);
        par.Value = value.HasValue ? new SqlDecimal(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of double type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="Data.ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, double? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Float, direction);
        par.Value = value.HasValue ? new SqlDouble(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of short type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="Data.ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, short? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.SmallInt, direction);
        par.Value = value.HasValue ? new SqlInt16(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of int type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, int? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Int, direction);
        par.Value = value.HasValue ? new SqlInt32(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of long type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, long? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.BigInt, direction);
        par.Value = value.HasValue ? new SqlInt64(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates input/output parameter of single type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, float? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Real, direction);
        par.Value = value.HasValue ? new SqlSingle(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }


    /// <summary>
    /// Creates input/output parameter of string type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="size">Maximum size of string.</param>
    /// <param name="variable">Flag for variable size.</param>
    /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    protected override void AddInOutParameter(string parameterName, string? value, int size, bool variable, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, direction, size, variable);
        if (!string.IsNullOrEmpty(value))
        {
            par.Value = new SqlString(value);
        }
        else
        {
            par.Value = DBNull.Value;
        }

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Creates numeric parameter.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="type"><see cref="NumericType"/> parameter.</param>
    /// /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <returns>Numeric parameter instance</returns>
    protected override SqlParameter CreateParameter(string parameterName, NumericType type, ParameterDirection direction)
    {
        SqlDbType sqlType = SqlDbType.Bit;
        switch (type)
        {
            case NumericType.Boolean:
                sqlType = SqlDbType.Bit;
                break;
            case NumericType.Byte:
                sqlType = SqlDbType.TinyInt;
                break;
            case NumericType.DateTime:
                sqlType = SqlDbType.DateTime;
                break;
            case NumericType.Double:
                sqlType = SqlDbType.Float;
                break;
            case NumericType.Int16:
                sqlType = SqlDbType.SmallInt;
                break;
            case NumericType.Int32:
                sqlType = SqlDbType.Int;
                break;
            case NumericType.Int64:
                sqlType = SqlDbType.BigInt;
                break;
            case NumericType.Single:
                sqlType = SqlDbType.Real;
                break;
            default:
                break;
        }

        return CreateParameter(parameterName, sqlType, direction);
    }

    private static SqlParameter CreateParameter(string parameterName, ParameterDirection direction)
    {
        return CreateParameter(parameterName, SqlDbType.Xml, direction);
    }

    /// <summary>
    /// Creates parameter of decimal type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// /// <param name="direction">Parameter <seealso cref="ParameterDirection">direction</seealso>.</param>
    /// <param name="scale">Parameter scale.</param>
    /// <param name="precision">Parameter precision.</param>
    /// <returns>Parameter instance of decimal type.</returns>
    protected override SqlParameter CreateParameter(string parameterName, ParameterDirection direction, byte precision, byte scale)
    {
        SqlParameter par = CreateParameter(parameterName, SqlDbType.Decimal, direction);
        par.Precision = precision;
        par.Scale = scale;

        return par;
    }

    /// <summary>
    /// Creates parameter of string type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="direction">Parameter direction ("input", "output" ou "inputoutput").</param>
    /// <param name="size">Maximum size of string.</param>
    /// <param name="variable">Flag for variable size.</param>
    /// <returns>Parameter instance of string type.</returns>
    protected override SqlParameter CreateParameter(string parameterName, ParameterDirection direction, int size, bool variable)
    {
        SqlParameter par = CreateParameter(parameterName, (variable ? SqlDbType.NVarChar : SqlDbType.NChar));
        par.Size = size;
        par.Direction = direction;

        return par;
    }

    private static SqlParameter CreateParameter(string parameterName, SqlDbType type)
    {
        return new SqlParameter(string.Concat("@", parameterName), type);
    }

    private static SqlParameter CreateParameter(string parameterName, SqlDbType type, ParameterDirection direction)
    {
        SqlParameter par = CreateParameter(parameterName, type);
        par.Direction = direction;
        return par;
    }

    private SqlCommand GetCommand()
    {
        CheckCommand();

        SqlCommand command = new()
        {
            CommandType = Command.CommandType,
            CommandText = Command.CommandText,
            Connection = Command.Connection,
        };

        command.CopyParameters(Command);

        return command;
    }

    /// <summary>
    /// Returns parameter from <see cref="SqlCommand.Parameters"/> collection.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Parameter instance.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    protected override SqlParameter GetParameter(string parameterName)
    {
        CheckParameter(parameterName);
        return Command.Parameters[string.Concat("@", parameterName)];
    }

    /// <summary>
    /// Handles exception thrown in SQL Server command execution.
    /// </summary>
    /// <param name="ex">Exception thrown in SQL Server command execution.</param>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Data.ConstraintException" />
    /// <exception cref="Microsoft.Data.SqlClient.SqlException" />
    protected override void HandleException(SqlException ex)
    {
        int errorNumber = ex.Number;
        if (errorNumber == storedProcedureError)
        {
            Match m = Regex.Match(ex.Message, $"({ForeignKeyError}|{DuplicateKeyError}|{PrimaryKeyError})");
            if (m.Success)
            {
                HandleException(Convert.ToInt32(m.Value), ex);
            }
            else
            {
                throw new InvalidOperationException(ResourcesFacade.GetString("CustomError"), ex);
            }
        } 
        else if (errorCodes.Contains(errorNumber)) 
        {
            HandleException(errorNumber, ex);
        }

        if (ex.Class == 16)
        {
            throw new InvalidOperationException(ResourcesFacade.GetString("ExecuteError"), ex);
        }

        throw ex;
    }

    #endregion

    #region IDataObject Members

    /// <summary>
    /// Creates input/output parameter of string type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="size">Maximum size of string.</param>
    /// <param name="variable">Flag for variable size.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ArgumentOutOfRangeException" />
    public override IDataObject AddInOutParameter(string parameterName, string? value, int size, bool variable)
    {
        AddInOutParameter(parameterName, value, size, variable, ParameterDirection.InputOutput);
        return this;
    }

    /// <summary>
    /// Creates input parameter of binary data for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName, byte[] value)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.VarBinary, ParameterDirection.Input);
        if (value.Length > 0)
        {
            par.Value = value;
        }

        Command.Parameters.Add(par);

        return this;
    }

    /// <summary>
    /// Creates input parameter for table-valued type (TVP) for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="parameterTypeName">Database user-defined type (UDT).</param>
    /// <param name="dt"><see cref="DataTable"/> for table-valued type.</param>
    public override IDataObject AddInParameter(string parameterName, string parameterTypeName, DataTable dt)
    {
        SqlParameter par = CreateParameter(parameterName, SqlDbType.Structured, ParameterDirection.Input);
        par.TypeName = parameterTypeName;
        if (dt != null)
        {
            par.Value = dt;
        }

        Command.Parameters.Add(par);

        return this;
    }

    /// <summary>
    /// Creates input parameter of string type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <param name="size">Maximum size of string.</param>
    /// <param name="variable">Flag for variable size.</param>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName, string? value, int size, bool variable)
    {
        AddInOutParameter(parameterName, value, size, variable, ParameterDirection.Input);
        return this;
    }

    /// <summary>
    /// Creates input parameter of XML type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value"><see cref="XmlReader"/> with XML content to be set as parameter value.</param>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName, XmlReader value)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, ParameterDirection.Input);
        if (value != null)
        {
            par.Value = new SqlXml(value);
        }

        Command.Parameters.Add(par);

        return this;
    }

    /// <summary>
    /// Creates input parameter of XML type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName)
    {
        CheckParameter(parameterName);
        Command.Parameters.Add(CreateParameter(parameterName, ParameterDirection.Input));

        return this;
    }

    /// <summary>
    /// Creates output parameter of XML type for database statement.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject AddOutParameter(string parameterName)
    {
        CheckParameter(parameterName);
        Command.Parameters.Add(CreateParameter(parameterName, ParameterDirection.Output));

        return this;
    }

    /// <summary>
    /// Returns output parameter value in binary data.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    public override byte[]? GetBinary(string parameterName)
    {
        SqlParameter par = GetParameter(parameterName);
        if (!Convert.IsDBNull(par.Value))
        {
            return par.Value as byte[];
        }

        return null;
    }

    /// <summary>
    /// Returns output parameter value of boolean type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override bool GetBoolean(string parameterName)
    {
        return Parameter<SqlParameter, bool>.GetValue(GetParameter(parameterName), false);
    }

    /// <summary>
    /// Returns output parameter value of boolean type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override bool? GetBooleanOrNull(string parameterName)
    {
        return Parameter<SqlParameter, bool>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of byte type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override byte GetByte(string parameterName)
    {
        return Parameter<SqlParameter, byte>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of byte type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override byte? GetByteOrNull(string parameterName)
    {
        return Parameter<SqlParameter, byte>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of type <see cref="DateTime"/>.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override DateTime GetDateTime(string parameterName)
    {
        return Parameter<SqlParameter, DateTime>.GetValue(GetParameter(parameterName), DateTime.MinValue);
    }

    /// <summary>
    /// Returns output parameter value of type <see cref="DateTime"/>.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override DateTime? GetDateTimeOrNull(string parameterName)
    {
        return Parameter<SqlParameter, DateTime>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of decimal type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override decimal GetDecimal(string parameterName)
    {
        return Parameter<SqlParameter, decimal>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of decimal type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override decimal? GetDecimalOrNull(string parameterName)
    {
        return Parameter<SqlParameter, decimal>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of double type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override double GetDouble(string parameterName)
    {
        return Parameter<SqlParameter, double>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of double type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override double? GetDoubleOrNull(string parameterName)
    {
        return Parameter<SqlParameter, double>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of short type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override short GetInt16(string parameterName)
    {
        return Parameter<SqlParameter, short>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of short type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override short? GetInt16OrNull(string parameterName)
    {
        return Parameter<SqlParameter, short>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of int type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override int GetInt32(string parameterName)
    {
        return Parameter<SqlParameter, int>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of int type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override int? GetInt32OrNull(string parameterName)
    {
        return Parameter<SqlParameter, int>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of long type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override long GetInt64(string parameterName)
    {
        return Parameter<SqlParameter, long>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of long type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override long? GetInt64OrNull(string parameterName)
    {
        return Parameter<SqlParameter, long>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Single".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override float GetSingle(string parameterName)
    {
        return Parameter<SqlParameter, float>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Returns output parameter value of single type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override float? GetSingleOrNull(string parameterName)
    {
        return Parameter<SqlParameter, float>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Returns output parameter value of string type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>Output parameter value.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override string GetString(string parameterName)
    {
        SqlParameter par = GetParameter(parameterName);
        return Convert.IsDBNull(par.Value) ? string.Empty : (string)par.Value;
    }

    /// <summary>
    /// Returns output parameter value XML.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <returns>XmlReader with output parameter Xml content.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="IndexOutOfRangeException" />
    public override XmlReader? GetXml(string parameterName)
    {
        SqlParameter par = GetParameter(parameterName);
        if (!Convert.IsDBNull(par))
        {
            SqlXml? xml = par.Value as SqlXml;
            return xml?.CreateReader();
        }

        return null;
    }

    /// <summary>
    /// Executes statement to query rows in SQL Server table.
    /// </summary>
    /// <param name="setter">Delegate to set order of columns in query result.</param>
    /// <param name="filler">Delegate to fill data obtained in query.</param>
    /// <param name="behavior"><see cref="CommandBehavior"/> of query.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="DbException"/>
    /// <exception cref="ConstraintException"/>
    public override void Read(SetOrdinal setter, Fill filler, CommandBehavior behavior)
    {
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(filler);

        if (Mode == ConnectionMode.Normal && ((behavior & CommandBehavior.CloseConnection) == 0))
        {
            behavior |= CommandBehavior.CloseConnection;
        }

        CheckCommand();

        try
        {
            OpenConnection();

            SqlCommand command = Mode != ConnectionMode.MultipleResultsets ? Command : GetCommand();
            using SqlDataReader reader = command.ExecuteReader(behavior);

            try
            {
                setter(reader);
                while (reader.Read())
                {
                    filler(reader);
                }
            }
            finally
            {
                if (Mode == ConnectionMode.MultipleResultsets)
                {
                    Command.CopyParameters(command);
                }
            }
        }
        catch (SqlException ex)
        {
            HandleException(ex);
        }
    }

    /// <summary>
    /// Executes asynchronously statement to query rows in database table.
    /// </summary>
    /// <param name="setter">Delegate to set order of columns in query result.</param>
    /// <param name="filler">Delegate to fill data obtained in query.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> for request. for request.</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="SqlException"/>
    public override async Task ReadAsync(SetOrdinal setter, Fill filler, CommandBehavior behavior, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(filler);

        OpenConnection();

        if (Mode == ConnectionMode.Normal && ((behavior & CommandBehavior.CloseConnection) == 0))
        {
            behavior |= CommandBehavior.CloseConnection;
        }

        try
        {
            await OpenConnectionAsync(cancellationToken);

            SqlCommand command = Mode != ConnectionMode.MultipleResultsets ? Command : GetCommand();
            using SqlDataReader reader = await command.ExecuteReaderAsync(behavior, cancellationToken);

            try
            {
                setter(reader);
                while (await reader.ReadAsync(cancellationToken))
                {
                    filler(reader);
                }
            }
            finally
            {
                if (Mode == ConnectionMode.MultipleResultsets)
                {
                    Command.CopyParameters(command);
                }
            }
        }
        catch (SqlException ex)
        {
            HandleException(ex);
        }
    }

    /// <summary>
    /// Executes SQL Server statement for scalar query that returns XML.
    /// </summary>
    /// <param name="filler">Delegate to fill data in XML format retrieved in query.</param>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="SqlException" />
    public override void ScalarXml(FillXml filler)
    {
        ArgumentNullException.ThrowIfNull(filler);
        
        CheckCommand();

        try
        {
            OpenConnection();
            filler(Command.ExecuteXmlReader());
        }
        catch (SqlException ex)
        {
            HandleException(ex);
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Executes asynchronously SQL Server statement for scalar query that returns XML.
    /// </summary>
    /// <param name="filler">Delegate to fill data in XML format retrieved in query.</param>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Data.SqlClient.SqlException" />
    public override async Task ScalarXmlAsync(FillXmlAsync filler)
    {
        await Task.Factory.StartNew(() => ScalarXmlAsync(filler, CancellationToken.None));
    }

    /// <summary>
    /// Executes asynchronously database statement for scalar query that returns XML.
    /// </summary>
    /// <param name="filler">Delegate to fill data in XML format retrieved in query.</param>
    /// <param name="cancellationToken">Cancellation token for request.</param>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="SqlException" />
    public override async Task ScalarXmlAsync(FillXmlAsync filler, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filler);

        CheckCommand();

        try
        {
            await OpenConnectionAsync(cancellationToken);
            await filler(await Command.ExecuteXmlReaderAsync(cancellationToken));
        }
        catch (SqlException ex)
        {
            HandleException(ex);
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Set value for parameter of boolean type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, bool? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlBoolean(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of byte type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, byte? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlByte(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of <see cref="DateTime"/>.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, DateTime? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDateTime(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of decimal type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, decimal? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDecimal(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of double type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, double? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDouble(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of short type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, short? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt16(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of int type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, int? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt32(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of long type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, long? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt64(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of single type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, float? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlSingle(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Set value for parameter of string type.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value">Parameter value.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            GetParameter(parameterName).Value = new SqlString(value);
        }
        else
        {
            GetParameter(parameterName).Value = null;
        }

        return this;
    }

    /// <summary>
    /// Set value for XML parameter.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="value"><see cref="XmlReader"/> with parameter value content.</param>
    /// <remarks>For <seealso cref="IDbCommand.Prepare">prepared</seealso> instructions.</remarks>
    /// <exception cref="ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, XmlReader value)
    {
        GetParameter(parameterName).Value = value != null ? new SqlXml(value) : null;
        return this;
    }

    #endregion
}
