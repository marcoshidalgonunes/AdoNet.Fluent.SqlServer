using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Xml;
using AdoNet.Fluent;
using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal sealed class XMLService(ISqlServerStatementBuilder builder) : IXMLService
{
    private StoreSurvey _storeSurvey = new();

    private readonly List<StoreSurvey> _storesSurvey = [];

    private const string FindDemographics = @"
        SELECT Demographics from Sales.Store 
        WHERE BusinessEntityID = @BusinessEntityID
    ";

    private const string SelectDemographics = @"
        SELECT Demographics from Sales.Store 
        WHERE SalesPersonID = @SalesPersonID
    ";

    private const string UpdateDemographics = @"
        UPDATE Sales.Store
            SET Demographics = @Demographics
        WHERE SalesPersonID = @SalesPersonID
    ";

    private int ordDemographics;

    private static XmlNodeReader ConvertToXml(StoreSurvey storeSurvey)
    {
        XmlDocument doc = new();

        XmlElement root = doc.CreateElement("StoreSurvey", "http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/StoreSurvey");
        doc.AppendChild(root);

        string? namespaceUri = doc.DocumentElement?.NamespaceURI;

        XmlElement annualSales = doc.CreateElement("AnnualSales", namespaceUri);
        annualSales.InnerText = storeSurvey.AnnualSales.ToString();
        root.AppendChild(annualSales);

        XmlElement annualRevenue = doc.CreateElement("AnnualRevenue", namespaceUri);
        annualRevenue.InnerText = storeSurvey.AnnualRevenue.ToString();
        root.AppendChild(annualRevenue);

        XmlElement bankName = doc.CreateElement("BankName", namespaceUri);
        bankName.InnerText = storeSurvey.BankName ?? string.Empty;
        root.AppendChild(bankName);

        XmlElement businessType = doc.CreateElement("BusinessType", namespaceUri);
        businessType.InnerText = storeSurvey.BusinessType ?? string.Empty;
        root.AppendChild(businessType);

        XmlElement yearOpened = doc.CreateElement("YearOpened", namespaceUri);
        yearOpened.InnerText = storeSurvey.YearOpened.ToString();
        root.AppendChild(yearOpened);

        XmlElement specialty = doc.CreateElement("Specialty", namespaceUri);
        specialty.InnerText = storeSurvey.Specialty ?? string.Empty;
        root.AppendChild(specialty);

        XmlElement squareFeet = doc.CreateElement("SquareFeet", namespaceUri);
        squareFeet.InnerText = storeSurvey.SquareFeet.ToString();
        root.AppendChild(squareFeet);

        XmlElement brands = doc.CreateElement("Brands", namespaceUri);
        brands.InnerText = storeSurvey.Brands ?? string.Empty;
        root.AppendChild(brands);

        XmlElement internet = doc.CreateElement("Internet", namespaceUri);
        internet.InnerText = storeSurvey.Internet ?? string.Empty;
        root.AppendChild(internet);

        XmlElement numberEmployees = doc.CreateElement("NumberEmployees", namespaceUri);
        numberEmployees.InnerText = storeSurvey.NumberEmployees.ToString();
        root.AppendChild(numberEmployees);

        return new XmlNodeReader(doc);
    }

    private void Fill(DbDataReader reader)
    {
        using XmlReader? salesReaderXml = reader.GetXml(ordDemographics);
        if (salesReaderXml is not null)
        {
            StoreSurvey storeSurvey = GetStoreSurvey(salesReaderXml);
            _storesSurvey.Add(storeSurvey);
        }
    }

    private void FillXml(XmlReader salesReaderXml)
    {
        _storeSurvey = GetStoreSurvey(salesReaderXml);
    }

    private async Task FillXmlAsync(XmlReader salesReaderXml)
    {
        _storeSurvey = await GetStoreSurveyAsync(salesReaderXml);
    }

    private static StoreSurvey GetStoreSurvey(XmlReader salesReaderXml)
    {
        StoreSurvey storeSurvey = new();

        while (salesReaderXml.Read())
        {
            if (salesReaderXml.NodeType == XmlNodeType.Element)
            {
                var elementLocalName = salesReaderXml.LocalName;
                salesReaderXml.Read();

                switch (elementLocalName)
                {
                    case "AnnualSales":
                        storeSurvey.AnnualSales = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "AnnualRevenue":
                        storeSurvey.AnnualRevenue = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "BankName":
                        storeSurvey.BankName = salesReaderXml.Value;
                        break;
                    case "BusinessType":
                        storeSurvey.BusinessType = salesReaderXml.Value;
                        break;
                    case "YearOpened":
                        storeSurvey.YearOpened = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Specialty":
                        storeSurvey.Specialty = salesReaderXml.Value;
                        break;
                    case "SquareFeet":
                        storeSurvey.SquareFeet = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Brands":
                        storeSurvey.Brands = salesReaderXml.Value;
                        break;
                    case "Internet":
                        storeSurvey.Internet = salesReaderXml.Value;
                        break;
                    case "NumberEmployees":
                        storeSurvey.NumberEmployees = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        return storeSurvey;
    }

    private static async Task<StoreSurvey> GetStoreSurveyAsync(XmlReader salesReaderXml)
    {
        StoreSurvey storeSurvey = new();

        while (await salesReaderXml.ReadAsync())
        {
            if (salesReaderXml.NodeType == XmlNodeType.Element)
            {
                var elementLocalName = salesReaderXml.LocalName;
                await salesReaderXml.ReadAsync();

                switch (elementLocalName)
                {
                    case "AnnualSales":
                        storeSurvey.AnnualSales = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "AnnualRevenue":
                        storeSurvey.AnnualRevenue = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "BankName":
                        storeSurvey.BankName = salesReaderXml.Value;
                        break;
                    case "BusinessType":
                        storeSurvey.BusinessType = salesReaderXml.Value;
                        break;
                    case "YearOpened":
                        storeSurvey.YearOpened = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Specialty":
                        storeSurvey.Specialty = salesReaderXml.Value;
                        break;
                    case "SquareFeet":
                        storeSurvey.SquareFeet = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    case "Brands":
                        storeSurvey.Brands = salesReaderXml.Value;
                        break;
                    case "Internet":
                        storeSurvey.Internet = salesReaderXml.Value;
                        break;
                    case "NumberEmployees":
                        storeSurvey.NumberEmployees = Convert.ToInt32(salesReaderXml.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        return storeSurvey;
    }

    private void SetOrdinal(IDataRecord reader)
    {
        ordDemographics = reader.GetOrdinal("Demographics");
    }

    #region IXMLService<StoreSurvey> members

    public StoreSurvey Find(int id)
    {
        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(FindDemographics)
            .AddInParameter("BusinessEntityID", id);

        statement.ScalarXml(FillXml);

        return _storeSurvey;
    }

    public async Task<StoreSurvey> FindAsync(int id)
    {
        using SqlServerStatement statement = builder.Build();
        statement
            .SetSql(FindDemographics)
            .AddInParameter("BusinessEntityID", id);

        await statement.ScalarXmlAsync(FillXmlAsync);

        return _storeSurvey;
    }

    public List<StoreSurvey> Read(int id)
    {
        _storesSurvey.Clear();

        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(SelectDemographics)
            .AddInParameter("SalesPersonID", id);

        statement.Read(SetOrdinal, Fill);

        return _storesSurvey;
    }

    public async Task<List<StoreSurvey>> ReadAsync(int id)
    {
        _storesSurvey.Clear();

        using SqlServerStatement statement = builder.Build();

        statement
            .SetSql(SelectDemographics)
            .AddInParameter("SalesPersonID", id);

        await statement.ReadAsync(SetOrdinal, Fill);

        return _storesSurvey;
    }

    public void Update(int id, StoreSurvey storeSurvey)
    {
        using SqlServerStatement statement = builder.Build();
        using XmlReader xmlReader = ConvertToXml(storeSurvey);

        statement
            .SetSql(UpdateDemographics)
            .AddInParameter("Demographics", xmlReader)
            .AddInParameter("SalesPersonID", id);

        statement.Execute();
    }

    public async Task UpdateAsync(int id, StoreSurvey storeSurvey)
    {
        using SqlServerStatement statement = builder.Build();
        using XmlReader xmlReader = ConvertToXml(storeSurvey);

        statement
            .SetSql(UpdateDemographics)
            .AddInParameter("Demographics", xmlReader)
            .AddInParameter("SalesPersonID", id);

        await statement.ExecuteAsync();
    }

    #endregion
}
