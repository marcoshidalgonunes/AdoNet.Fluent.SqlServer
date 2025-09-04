namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal class ImageService(ISqlServerStatementBuilder builder) : IImageService
{
    private readonly ISqlServerStatementBuilder _builder = builder;

    private const string SelectImage = "SELECT ThumbNailPhoto FROM Production.ProductPhoto WHERE ProductPhotoID = @Id";

    private const string UpdateImage = "UPDATE Production.ProductPhoto SET ThumbNailPhoto = @Image WHERE ProductPhotoID = @Id";

    public byte[]? Get(int id)
    {
        SqlServerStatement _statement = _builder.Build();

        return _statement
            .SetSql(SelectImage)
            .AddInParameter("Id", id)
            .ScalarBinary();
    }

    public async Task<byte[]?> GetAsync(int id)
    {
        SqlServerStatement _statement = _builder.Build();

        return await _statement
            .SetSql(SelectImage)
            .AddInParameter("Id", id)
            .ScalarBinaryAsync();
    }

    public void Save(int id, byte[] data)
    {
        SqlServerStatement _statement = _builder.Build();

        _statement
            .SetSql(UpdateImage)
            .AddInParameter("Image", data)
            .AddInParameter("Id", id)
            .Execute();
    }

    public async Task SaveAsync(int id, byte[] data)
    {
        SqlServerStatement _statement = _builder.Build();

        await _statement
            .SetSql(UpdateImage)
            .AddInParameter("Image", data)
            .AddInParameter("Id", id)
            .ExecuteAsync();
    }
}
