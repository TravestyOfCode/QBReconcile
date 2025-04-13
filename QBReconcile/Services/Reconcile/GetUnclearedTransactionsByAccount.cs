using QBReconcile.Models;
using System.Xml.Linq;

namespace QBReconcile.Services.Reconcile;

public class GetUnclearedTransactionsByAccount : IRequest<Result<List<QBTransaction>>>
{
    public required string Account { get; set; }
}

public class GetUnclearedTransactionsByAccountHandler(QBConnection conn, ILogger<GetUnclearedTransactionsByAccountHandler> logger)
    : IRequestHandler<GetUnclearedTransactionsByAccount, Result<List<QBTransaction>>>
{
    public async Task<Result<List<QBTransaction>>> Handle(GetUnclearedTransactionsByAccount request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Resource.UnclearedTransactionQuery
                .Replace("@@FromDate", DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"))
                .Replace("@@ToDate", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("@@Account", request.Account);

            var queryResult = await Task.Run(() => conn.ProcessRequest(query), cancellationToken);

            if (!queryResult.IsNullOrEmpty())
            {
                return ProcessResult(queryResult!);
            }

            return Error.ServerError();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error handling request: {request}", request);

            return Error.ServerError();
        }
    }

    private List<QBTransaction> ProcessResult(string result)
    {
        var doc = XDocument.Parse(result);

        var txns = new List<QBTransaction>();

        if (doc != null)
        {
            foreach (var row in doc.Descendants("DataRow"))
            {
                var txn = ParseDataRow(row);

                if (txn != null)
                {
                    txns.Add(txn);
                }
            }
        }

        return txns;
    }

    private QBTransaction? ParseDataRow(XElement row)
    {
        var result = new QBTransaction();

        foreach (var colData in row.Elements("ColData"))
        {
            var colID = colData.Attribute("colID").AsInt(-1);

            if (colID != -1)
            {
                switch (colID)
                {
                    case 2: result.Type = colData.Attribute("value").AsString(); break;
                    case 3: result.Date = colData.Attribute("value").AsDateTime(); break;
                    case 4: result.Num = colData.Attribute("value").AsString(); break;
                    case 5: result.Name = colData.Attribute("value").AsString(); break;
                    case 6: result.Clr = colData.Attribute("value")?.Value == "Cleared" ? true : false; break;
                    case 7: result.Amount = colData.Attribute("value").AsDecimal(); break;
                }
            }

        }

        if (result.Clr == true)
        {
            return null;
        }

        return result;
    }
}