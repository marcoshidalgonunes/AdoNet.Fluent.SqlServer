using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Xml;
using AdoNet.Fluent.SqlServer.Extensions;
using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer;

public abstract class SqlServerDataObject : DataObject<SqlConnection, SqlCommand, SqlParameter, SqlException>
{
    #region Constructor

    protected SqlServerDataObject(string connectionString, ConnectionMode mode) 
        : base(connectionString, mode)
    {
        ForeignKeyError = 547;
        PrimaryKeyError = 2627;
        DuplicateKeyError = 2601;
        
        errorCodes = [ForeignKeyError, DuplicateKeyError, PrimaryKeyError];
    }

    #endregion

    #region Protected/Private Members

    private readonly int storedProcedureError = 50000;
    private readonly HashSet<int> errorCodes;

    /// <summary>
    /// Cria parâmetro de entrada do tipo "boolean" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, bool value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Bit, direction);
        par.Value = new SqlBoolean(value);

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "decimal" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="scale">Escala do parâmetro.</param>
    /// <param name="precision">Precisão do parâmetro.</param>
    /// <param name="direction">Direção do parâmetro ("input" ou "input/output").</param>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.ArgumentOutOfRangeException" />
    protected override void AddInOutParameter(string parameterName, byte precision, byte scale, ParameterDirection direction)
    {
        CheckParameter(parameterName, precision, scale);
        Command.Parameters.Add(CreateParameter(parameterName, direction, precision, scale));
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Byte" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, byte? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.TinyInt, direction);
        par.Value = value.HasValue ? new SqlByte(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "DateTime" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, DateTime? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.DateTime, direction);
        par.Value = value.HasValue ? new SqlDateTime(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Decimal" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <param name="scale">Escala do parâmetro.</param>
    /// <param name="precision">Precisão do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, decimal? value, byte precision, byte scale, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, direction, precision, scale);
        par.Value = value.HasValue ? new SqlDecimal(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Double" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, double? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Float, direction);
        par.Value = value.HasValue ? new SqlDouble(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Int16" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, short? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.SmallInt, direction);
        par.Value = value.HasValue ? new SqlInt16(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Int32" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, int? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Int, direction);
        par.Value = value.HasValue ? new SqlInt32(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Int64" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, long? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.BigInt, direction);
        par.Value = value.HasValue ? new SqlInt64(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Single" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    protected override void AddInOutParameter(string parameterName, float? value, ParameterDirection direction)
    {
        CheckParameter(parameterName);

        SqlParameter par = CreateParameter(parameterName, SqlDbType.Real, direction);
        par.Value = value.HasValue ? new SqlSingle(value.Value) : DBNull.Value;

        Command.Parameters.Add(par);
    }


    /// <summary>
    /// Cria parâmetro bidirecional do tipo "String" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <param name="size">Tamanho máximo da string.</param>
    /// <param name="variable">Verdadeiro se string tem tamanho variável.</param>
    /// <param name="direction">Direção do parâmetro ("input" ou "input/output").</param>
    /// <exception cref="System.ArgumentNullException" />
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
    /// Cria parâmetro numérico.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="type">Tipo <see cref="Op2b.Data.NumericType">numérico</see> do parâmetro.</param>
    /// <param name="direction">Direção do parâmetro ("input" ou "output").</param>
    /// <returns>Instância de parâmetro numérico</returns>
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
    /// Cria parâmetro do tipo "Decimal".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="direction">Direção do parâmetro ("input" ou "output").</param>
    /// <param name="scale">Escala do parâmetro.</param>
    /// <param name="precision">Precisão do parâmetro.</param>
    /// <returns>Instância de parâmetro do tipo "Decimal".</returns>
    protected override SqlParameter CreateParameter(string parameterName, ParameterDirection direction, byte precision, byte scale)
    {
        SqlParameter par = CreateParameter(parameterName, SqlDbType.Decimal, direction);
        par.Precision = precision;
        par.Scale = scale;

        return par;
    }

    /// <summary>
    /// Cria parâmetro do tipo "String".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="direction">Direção do parâmetro (entrada, saída ou ambos).</param>
    /// <param name="size">Tamanho máximo da string.</param>
    /// <param name="variable">Verdadeiro se string tem tamanho variável.</param>
    /// <returns>Instância de parâmetro do tipo "String".</returns>
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
    /// Recupera parâmtro da coleção.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Instância do parâmetro.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    protected override SqlParameter GetParameter(string parameterName)
    {
        CheckParameter(parameterName);
        return Command.Parameters[string.Concat("@", parameterName)];
    }

    /// <summary>
    /// Trata exceção disparada na chamada ao banco de dados.
    /// </summary>
    /// <param name="ex">Exceção disparada na chamada ao banco de dados.</param>
    /// <exception cref="System.InvalidOperationException" />
    /// <exception cref="System.Data.ConstraintException" />
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
    /// Cria parâmetro bidirecional do tipo "String" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <param name="size">Tamanho máximo da string.</param>
    /// <param name="variable">Verdadeiro se string tem tamanho variável.</param>
    public override IDataObject AddInOutParameter(string parameterName, string? value, int size, bool variable)
    {
        AddInOutParameter(parameterName, value, size, variable, ParameterDirection.InputOutput);
        return this;
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Xml" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName)
    {
        CheckParameter(parameterName);
        Command.Parameters.Add(CreateParameter(parameterName, ParameterDirection.Input));

        return this;
    }

    /// <summary>
    /// Cria parâmetro de entrada de dados binários para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
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
    /// Cria parâmetro de entrada para coleção de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="parameterTypeName">Tipo definido pelo usuário (UDT) no banco de dados.</param>
    /// <param name="dt">"Data Table" com a coleção de dados.</param>
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
    /// Cria parâmetro de entrada do tipo "String" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <param name="size">Tamanho máximo da string.</param>
    /// <param name="variable">Verdadeiro se string tem tamanho variável.</param>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject AddInParameter(string parameterName, string? value, int size, bool variable)
    {
        AddInOutParameter(parameterName, value, size, variable, ParameterDirection.Input);
        return this;
    }

    /// <summary>
    /// Cria parâmetro de entrada do tipo "Xml" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">XmlReader com o conteúdo XML a ser passado para o parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
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
    /// Cria parâmetro de saída do tipo "Xml" para instrução de banco de dados.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject AddOutParameter(string parameterName)
    {
        CheckParameter(parameterName);
        Command.Parameters.Add(CreateParameter(parameterName, ParameterDirection.Output));

        return this;
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída em dados binários.
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
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
    /// Retorna valor de parâmetro de saída do tipo "Boolean".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override bool GetBoolean(string parameterName)
    {
        return Parameter<SqlParameter, bool>.GetValue(GetParameter(parameterName), false);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Boolean".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override bool? GetBooleanOrNull(string parameterName)
    {
        return Parameter<SqlParameter, bool>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Byte".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override byte GetByte(string parameterName)
    {
        return Parameter<SqlParameter, byte>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Byte".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override byte? GetByteOrNull(string parameterName)
    {
        return Parameter<SqlParameter, byte>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "DateTime".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override DateTime GetDateTime(string parameterName)
    {
        return Parameter<SqlParameter, DateTime>.GetValue(GetParameter(parameterName), DateTime.MinValue);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "DateTime".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override DateTime? GetDateTimeOrNull(string parameterName)
    {
        return Parameter<SqlParameter, DateTime>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Decimal".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override decimal GetDecimal(string parameterName)
    {
        return Parameter<SqlParameter, decimal>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Decimal".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override decimal? GetDecimalOrNull(string parameterName)
    {
        return Parameter<SqlParameter, decimal>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Double".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override double GetDouble(string parameterName)
    {
        return Parameter<SqlParameter, double>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Double".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override double? GetDoubleOrNull(string parameterName)
    {
        return Parameter<SqlParameter, double>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int16".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override short GetInt16(string parameterName)
    {
        return Parameter<SqlParameter, short>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int16".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override short? GetInt16OrNull(string parameterName)
    {
        return Parameter<SqlParameter, short>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int32".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override int GetInt32(string parameterName)
    {
        return Parameter<SqlParameter, int>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int32".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override int? GetInt32OrNull(string parameterName)
    {
        return Parameter<SqlParameter, int>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int64".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override long GetInt64(string parameterName)
    {
        return Parameter<SqlParameter, long>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Int64".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override long? GetInt64OrNull(string parameterName)
    {
        return Parameter<SqlParameter, long>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Single".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override float GetSingle(string parameterName)
    {
        return Parameter<SqlParameter, float>.GetValue(GetParameter(parameterName), 0);
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Single".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override float? GetSingleOrNull(string parameterName)
    {
        return Parameter<SqlParameter, float>.GetValue(GetParameter(parameterName));
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "String".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>Valor de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
    public override string GetString(string parameterName)
    {
        SqlParameter par = GetParameter(parameterName);
        return Convert.IsDBNull(par.Value) ? string.Empty : (string)par.Value;
    }

    /// <summary>
    /// Retorna valor de parâmetro de saída do tipo "Xml".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <returns>XmlReader com o conteúdo Xml de parâmetro de saída.</returns>
    /// <exception cref="System.ArgumentNullException" />
    /// <exception cref="System.IndexOutOfRangeException" />
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
    /// Executa instrução para consultar linhas em tabelas de banco de dados.
    /// </summary>
    /// <param name="setter">"Delegate" para definir posição das colunas de dados da consulta.</param>
    /// <param name="filler">"Delegate" para preencher dados obtidos na consulta.</param>
    /// <param name="behavior">Comportamento da consulta.</param>
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
            using DbDataReader reader = command.ExecuteReader(behavior);

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
    /// Executa instrução assíncrona para consultar linhas em tabelas de banco de dados.
    /// </summary>
    /// <param name="setter">"Delegate" para definir posição das colunas de dados da consulta.</param>
    /// <param name="filler">"Delegate" para preencher dados obtidos na consulta.</param>
    /// <param name="cancellationToken">Token para monitorar pedidos de cancelamento.</param>
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
            using DbDataReader reader = await command.ExecuteReaderAsync(behavior, cancellationToken);

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
    /// Executa operação para consulta escalar em banco de dados que retorna valor do tipo "Xml".
    /// </summary>
    /// <param name="filler">"Delegate" para preencher dados em formato XML obtidos na consulta.</param>
    /// <exception cref="System.InvalidOperationException" />
    /// <exception cref="System.Data.SqlClient.SqlException" />
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
    /// Executa instrução assíncrona para consulta escalar em banco de dados que retorna valor do tipo "Xml".
    /// </summary>
    /// <param name="filler">"Delegate" para preencher dados em formato XML obtidos na consulta.</param>
    /// <exception cref="System.InvalidOperationException" />
    /// <exception cref="System.Data.SqlClient.SqlException" />
    public override async Task ScalarXmlAsync(FillXmlAsync filler)
    {
        await Task.Factory.StartNew(() => ScalarXmlAsync(filler, CancellationToken.None));
    }

    /// <summary>
    /// Executa instrução assíncrona para consulta escalar em banco de dados que retorna valor do tipo "Xml".
    /// </summary>
    /// <param name="filler">"Delegate" para preencher dados em formato XML obtidos na consulta.</param>
    /// <param name="cancellationToken">Token para monitorar pedidos de cancelamento.</param>
    /// <exception cref="System.InvalidOperationException" />
    /// <exception cref="System.Data.SqlClient.SqlException" />
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
    /// Define o valor para parâmetro do tipo "Boolean".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, bool? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlBoolean(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Byte".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, byte? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlByte(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "DateTime".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, DateTime? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDateTime(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Decimal".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, decimal? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDecimal(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Double".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, double? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlDouble(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Int16".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, short? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt16(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Int32".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, int? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt32(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Int64".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, long? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlInt64(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "Single".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, float? value)
    {
        GetParameter(parameterName).Value = value.HasValue ? new SqlSingle(value.Value) : null;
        return this;
    }

    /// <summary>
    /// Define o valor para parâmetro do tipo "String".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">Valor do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
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
    /// Define o valor para parâmetro do tipo "Xml".
    /// </summary>
    /// <param name="parameterName">Nome do parâmetro.</param>
    /// <param name="value">XmlReader com o contédo do parâmetro.</param>
    /// <remarks>Para execução de instruções preparadas.</remarks>
    /// <exception cref="System.ArgumentNullException" />
    public override IDataObject SetParameter(string parameterName, XmlReader value)
    {
        GetParameter(parameterName).Value = value != null ? new SqlXml(value) : null;
        return this;
    }

    #endregion
}
