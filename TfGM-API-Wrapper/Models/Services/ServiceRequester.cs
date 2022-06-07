using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TfGM_API_Wrapper.Models.Resources;

namespace TfGM_API_Wrapper.Models.Services;

/// <summary>
/// Handles requests for services from the TfGM API.
/// </summary>
public class ServiceRequester : IRequester
{
    private readonly ApiOptions _apiOptions;

    /// <summary>
    /// Creates a new ServiceRequester using an injected IConfiguration.
    /// config has a default of null in case it is not injected or has not been configured.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="apiOptions"></param>
    public ServiceRequester(ApiOptions apiOptions)
    {
        _apiOptions = apiOptions;
    }

    /// <summary>
    /// Requests services for a list of given IDs.
    /// </summary>
    /// <param name="ids">List of IDs for a stop</param>
    /// <returns>List of Unformatted Services returned from the request</returns>
    public List<UnformattedServices> RequestServices(List<int> ids)
    {
        var unformattedServices = new List<UnformattedServices>();
        foreach (var id in ids) unformattedServices.Add(RequestId(id).Result);

        return unformattedServices;
    }

    /// <summary>
    /// Requests the service information from a given ID using the TfGM API.
    /// </summary>
    /// <param name="id">ID to request service info for</param>
    /// <returns>Unformatted Service for the given ID</returns>
    private async Task<UnformattedServices> RequestId(int id)
    {
        var client = new HttpClient();
        
        // Request headers
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiOptions.OcpApimSubscriptionKey);

        var uri = $"https://api.tfgm.com/odata/Metrolinks({id})";

        var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<UnformattedServices>(responseJson);
    }
}