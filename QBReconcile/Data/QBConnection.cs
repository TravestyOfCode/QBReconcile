using Microsoft.Extensions.Options;
using QBReconcile.Services;
using QBXMLRP2Lib;

namespace QBReconcile.Data;

/// <summary>
/// A class that allows a connection to QuickBooks using the QBSDK to process
/// QBXML requests.
/// </summary>
public class QBConnection(IOptions<QBSDKOptions> options, ILogger<QBConnection> logger)
{
    // The QBSDK request processor that allows acces to QB and 
    // processes the requests.
    private IRequestProcessor5? rp;

    // The ticket used when connected to QB via SDK needed
    // to process requests.
    private string? ticket;

    /// <summary>
    /// Sends a QBXML string request to QuickBooks to process.
    /// </summary>
    /// <param name="request">The string QBXML to process.</param>
    /// <returns>The QBXML response from the request or null if there was an error.</returns>
    public string? ProcessRequest(string request)
    {
        if (!Open())
        {
            return null;
        }

        return rp!.ProcessRequest(ticket, request);
    }

    /// <summary>
    /// Tries to open a connection to QuickBooks using the SDK options.
    /// </summary>
    /// <returns>true if the connection was able to be opened, otherwise false.</returns>
    private bool Open()
    {
        if (IsCorrectFileOpen())
        {
            return true;
        }

        // Close any connection we may currently have. This will ensure
        // rp and ticket are both null.
        Close();

        try
        {
            rp = new RequestProcessor3();

            rp.OpenConnection2(options.Value.AppId, options.Value.AppId, QBXMLRPConnectionType.localQBD);

            ticket = rp.BeginSession(options.Value.QBFile, QBFileMode.qbFileOpenDoNotCare);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error when trying to open a connection to QuickBooks using {options}", options.Value);
            return false;
        }

    }

    /// <summary>
    /// Check to see if we have an existing open connection and if we are
    /// connected to the file in the QBSDKOptions.
    /// </summary>
    /// <returns>true if the connection is open to the request file, otherwise false.</returns>
    private bool IsCorrectFileOpen()
    {
        if (rp == null || ticket.IsNullOrEmpty())
        {
            return false;
        }

        try
        {
            var currentFile = rp.GetCurrentCompanyFileName(ticket);

            // If the QBFile is null or empty, we don't care which file to use
            if (options.Value.QBFile.IsNullOrEmpty())
            {
                return true;
            }

            return Equals(System.IO.Path.GetFullPath(options.Value.QBFile!), System.IO.Path.GetFullPath(currentFile));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error trying to get current QB file.");
            return false;
        }
    }

    /// <summary>
    /// Close the connection to QuickBooks.
    /// </summary>
    private void Close()
    {
        EndSession();

        CloseConnection();
    }

    /// <summary>
    /// Ends the active session with QuickBooks if there is any.
    /// </summary>
    private void EndSession()
    {
        try
        {
            if (rp != null && !ticket.IsNullOrEmpty())
            {
                rp.EndSession(ticket);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error trying to end session with QuickBooks.");
        }
        finally
        {
            ticket = null;
        }
    }

    /// <summary>
    /// Closed the active connection with QuickBooks if any.
    /// </summary>
    private void CloseConnection()
    {
        try
        {
            rp?.CloseConnection();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error closing connection to QuickBooks");
        }
        finally
        {
            rp = null;
        }
    }
}
