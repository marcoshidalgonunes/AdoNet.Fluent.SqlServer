namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal interface IImageService
{ 
    byte[]? Get(int id);

    Task<byte[]?> GetAsync(int id);

    void Save(int id, byte[] data);

    Task SaveAsync(int id, byte[] data);
}
