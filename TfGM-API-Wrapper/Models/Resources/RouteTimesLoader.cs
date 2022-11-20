using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TfGM_API_Wrapper.Models.RoutePlanner;

namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
/// Loads route times information
/// </summary>
public class RouteTimesLoader
{

    private ResourcesConfig _resourcesConfig;
    
    /// <summary>
    /// Creates a new route times loader using the resources config.
    /// The resources config is used for the route times file location.
    /// </summary>
    /// <param name="resourcesConfig"></param>
    public RouteTimesLoader(ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new LoaderHelper();
        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));
        
        _resourcesConfig.RouteTimesPath = loaderHelper.CheckFileRequirements(resourcesConfig.RouteTimesPath,
            nameof(resourcesConfig.RouteTimesPath));
    }

    /// <summary>
    /// Imports route times using the resources config given to the constructor
    /// </summary>
    /// <returns>Imported Times for Routes</returns>
    public RouteTimes ImportRouteTimes()
    {
        using var reader = new StreamReader(_resourcesConfig.RouteTimesPath);
        var jsonString = reader.ReadToEnd();
        var unprocessedRouteTimes = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>> (jsonString);
        var createdRouteTimes = new RouteTimes();
        foreach (var routeName in unprocessedRouteTimes!.Keys)
        {
            createdRouteTimes.AddRoute(routeName, ProcessesRoute(unprocessedRouteTimes![routeName]));
        }
        return createdRouteTimes;
    }

    /// <summary>
    /// Converts the Stop name -> Time stamp string to
    /// a StopName -> Timestamp
    /// </summary>
    /// <param name="unprocessedRoute">Unprocessed route times to convert to timestamps</param>
    /// <returns>A stop name, TimeStamp dict</returns>
    private Dictionary<string, TimeSpan> ProcessesRoute(Dictionary<string, string> unprocessedRoute)
    {
        return unprocessedRoute.ToDictionary(
            kvp => kvp.Key, 
            kvp => TimeSpan.Parse(kvp.Value));
    }
}