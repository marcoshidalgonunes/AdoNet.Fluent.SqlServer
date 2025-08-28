using System.Data.SqlTypes;
using System.Xml;
using Microsoft.Data.SqlClient;

namespace System.Data.Common;

public static class DbDataReaderExtensions
{
    /// <summary>
    /// Returns output parameter value XML.
    /// </summary>
    /// <param name="reader"><see cref="System.Data.Common.DbDataReader"/> that retrieves XML data </param>
    /// <param name="ordinal">Order of XML column</param>
    /// <returns>XmlReader with output parameter Xml content.</returns>
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
