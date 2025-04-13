using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QBReconcile.Services;
using QBReconcile.Services.Reconcile;

namespace QBReconcile.Controllers;

public class ReconcileController(IServiceProvider services) : Controller
{
    public async Task<IActionResult> Index(GetReconcilableAccounts request, CancellationToken cancellationToken)
    {
        var handler = services.GetRequiredService<IRequestHandler<GetReconcilableAccounts, Result<List<string>>>>();

        var result = await handler.Handle(request, cancellationToken);

        if (result.WasSuccessful)
        {
            return View(result.Value);
        }

        return StatusCode(result.StatusCode);
    }
}
