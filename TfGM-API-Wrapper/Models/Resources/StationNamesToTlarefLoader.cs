using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
///     Loads the StationNamesToTlarefs map using the ResourcesConfig.
/// </summary>
public class StationNamesToTlarefLoader
{
    private readonly ResourcesConfig _resourcesConfig;

    /// <summary>
    ///     Creates a new loader. Checks the ResourceConfig values provided are valid using
    ///     the LoaderHelper.
    /// </summary>
    /// <param name="resourcesConfig">ResourceConfig created at startup. Injected as a service.</param>
    /// <exception cref="ArgumentNullException">Thrown if the ResourceConfig is null.</exception>
    public StationNamesToTlarefLoader(ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new LoaderHelper();

        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));

        _resourcesConfig.StationNamesToTlarefsPath = loaderHelper.CheckFileRequirements(
            resourcesConfig.StationNamesToTlarefsPath,
            nameof(resourcesConfig.StationNamesToTlarefsPath));
    }

    /// <summary>
    ///     Import the Station Names to Tlarefs dict.
    /// </summary>
    /// <returns>StationNames to Tlarefs dict. Key is the station name.</returns>
    public Dictionary<string, string> ImportStationNamesToTlarefs()
    {
        using var reader = new StreamReader(_resourcesConfig.StationNamesToTlarefsPath);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
    }
}