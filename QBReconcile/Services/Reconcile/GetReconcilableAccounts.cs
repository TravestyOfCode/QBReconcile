using System.Xml.Linq;

namespace QBReconcile.Services.Reconcile;

public class GetReconcilableAccounts : IRequest<Result<List<string>>>
{

}

public class GetReconcilableAccountsHandler(QBConnection conn, ILogger<GetReconcilableAccountsHandler> logger)
    : IRequestHandler<GetReconcilableAccounts, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(GetReconcilableAccounts request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await Task.Run(() => conn.ProcessRequest(Resource.ReconcilableAccountQuery));

            if (response != null)
            {
                return ParseResponse(response);
            }

            return Error.ServerError();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling request: {request}", request);

            return Error.ServerError();
        }
    }

    private List<string> ParseResponse(string response)
    {
        var accounts = new List<string>();

        var doc = XDocument.Parse(response);
        foreach (var element in doc.Descendants("AccountRet"))
        {
            var name = element.Element("FullName")?.Value;

            if (!name.IsNullOrEmpty())
            {
                accounts.Add(name!);
            }
        }
        return accounts;
    }
}
