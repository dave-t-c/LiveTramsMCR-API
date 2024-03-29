namespace LiveTramsMCR.Models.V1.Resources;

/// <summary>
///     Stores API Options used when requesting services
///     from the TfGM API.
///     These should be imported on project startup.
/// </summary>
public class ApiOptions
{
    /// <summary>
    ///     TfGM API Subscription key
    /// </summary>
    public string OcpApimSubscriptionKey { get; set; }

    /// <summary>
    ///     Base request URLs for live service information
    /// </summary>
    public BaseUrls BaseRequestUrls { get; set; }
}