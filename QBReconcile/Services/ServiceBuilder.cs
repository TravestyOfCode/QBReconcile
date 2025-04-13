using Microsoft.Extensions.DependencyInjection;
using QBReconcile.Services.Reconcile;

namespace QBReconcile.Services;

public static class ServiceBuilder
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<GetReconcilableAccounts, Result<List<string>>>>(s => new GetReconcilableAccountsHandler(s.GetRequiredService<QBConnection>(), s.GetRequiredService<ILogger<GetReconcilableAccountsHandler>>()));

        return services;
    }
}
