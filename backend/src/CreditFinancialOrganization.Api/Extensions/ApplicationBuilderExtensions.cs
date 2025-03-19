using CreditFinancialOrganization.Infrastructure.Data.Initializers;

namespace CreditFinancialOrganization.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> InitializeAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeAsync();

        return app;
    }
}