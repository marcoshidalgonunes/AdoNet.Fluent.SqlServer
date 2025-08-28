using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface IXMLService
{
    StoreSurvey Find(int id);

    Task<StoreSurvey> FindAsync(int id);    

    List<StoreSurvey> Read(int id);

    Task<List<StoreSurvey>> ReadAsync(int id);

    void Update(int id, StoreSurvey storeSurvey);

    Task UpdateAsync(int id, StoreSurvey storeSurvey);
}
