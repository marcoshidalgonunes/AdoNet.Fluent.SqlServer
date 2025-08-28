using System.Globalization;
using System.Resources;

namespace AdoNet.Fluent.SqlServer;

internal class ResourcesFacade
{
    private static readonly ResourceManager _resourceManager = new("AdoNet.Fluent.SqlServer.Resources.Messages", typeof(ResourcesFacade).Assembly);

    internal static string? GetString(string name) => _resourceManager?.GetString(name, culture: CultureInfo.CurrentUICulture);
}
