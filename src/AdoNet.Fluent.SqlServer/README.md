# AdoNet.Fluent.SqlServer
Classes and interfaces for SQL Server ADO.Net fluent notation.

## Purpose
This library offers another abstraction layer to ADO.Net, as [LINQ](https://learn.microsoft.com/en-us/dotnet/standard/linq/) does. It is aimed to deliver a higher performance than [Entity Framework](https://learn.microsoft.com/en-us/ef/ef6/) because combines the ease of fluent notation with direct call to [ADO,NET](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/) classes.

As pure ADO.NET, the AdoNet.Fluent.SqlServer implementation demands a knowledge of SQL Server, including SQL commands. The benefit is a code easier to be developed, suitable to be used with AI helpers such as GitHub Copilot, in order to achieve a godd development productivity along with better performance.

AdoNet.Fluent.SqlServer fits in development of containerized microservices that use SQL Server and requires high throughput.
