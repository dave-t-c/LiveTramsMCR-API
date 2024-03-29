using System;
using System.Collections.Generic;
using System.IO;
using LiveTramsMCR.Models.V1.Stops;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Loads in the Stop data from the resources folder.
/// </summary>
public class StopLoader
{
    private readonly ResourcesConfig? _resourcesConfig;

    /// <summary>
    ///     Create a new StopLoader Object, which can import the required Stops.
    /// </summary>
    /// <param name="resourcesConfig"></param>
    public StopLoader(ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new LoaderHelper();

        if (resourcesConfig.StopResourcePath == null)
            return;

        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));

        _resourcesConfig.StopResourcePath = LoaderHelper.CheckFileRequirements(resourcesConfig.StopResourcePath,
            nameof(resourcesConfig.StopResourcePath));
    }

    /// <summary>
    ///     Imports the Stops from the Stops resources file.
    ///     The results from this are then returned with a GET to '/api/stops'.
    /// </summary>
    public List<Stop> ImportStops()
    {
        if (_resourcesConfig?.StopResourcePath == null)
            return new List<Stop>();

        using var reader = new StreamReader(_resourcesConfig.StopResourcePath!);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<List<Stop>>(jsonString)!;
    }
}