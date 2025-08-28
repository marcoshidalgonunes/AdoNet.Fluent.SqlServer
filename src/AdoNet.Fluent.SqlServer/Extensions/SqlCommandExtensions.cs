using Microsoft.Data.SqlClient;

namespace AdoNet.Fluent.SqlServer.Extensions;

internal static class SqlCommandExtensions
{
    internal static void CopyParameters(this SqlCommand destination, SqlCommand source)
    {
        int countParameters = source.Parameters.Count;
        SqlParameter[] parameters = new SqlParameter[countParameters];
        for (int i = 0; i < countParameters; i++)
        {
            parameters[i] = source.Parameters[i];
        }

        source.Parameters.Clear();
        destination.Parameters.AddRange(parameters);
    }
}
