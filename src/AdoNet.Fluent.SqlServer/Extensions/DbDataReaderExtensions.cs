using System.Data.SqlTypes;
using System.Xml;
using Microsoft.Data.SqlClient;

namespace System.Data.Common;

public static class DbDataReaderExtensions
{
    public static XmlReader? GetXml(this DbDataReader reader, int ordinal)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (reader is SqlDataReader sqlDataReader)
        {
            SqlXml sqlXml = sqlDataReader.GetSqlXml(ordinal);
            XmlReader xmlReader = sqlXml.CreateReader();

            xmlReader.MoveToContent();
            return xmlReader;
        }

        return null;
    }
}
