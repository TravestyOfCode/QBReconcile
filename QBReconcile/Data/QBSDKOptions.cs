namespace QBReconcile.Data;

/// <summary>
/// The configuration class for a connection to QB using the SDK.
/// </summary>
public class QBSDKOptions
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string Name = "QBSDK";

    /// <summary>
    /// The AppId used to connect to QuickBooks
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// The QuickBooks file to connect to. If empty, will use whatever file
    /// is currently open in QuickBooks.
    /// </summary>
    public string? QBFile { get; set; }
}
