using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Loads the TlarefToIds map.
/// </summary>
public class TlarefToIdsLoader
{
    private readonly ResourcesConfig? _resourcesConfig;

    /// <summary>
    ///     Creates a new loader by verifying the path given using the LoaderHelper class.
    /// </summary>
    /// <param name="resourcesConfig">Config created at startup. Injected as a service.</param>
    /// <exception cref="ArgumentNullException">Thrown if the resources config is null.</exception>
    public TlarefToIdsLoader(ResourcesConfig resourcesConfig)
    {
        var loaderHelper = new LoaderHelper();

        if (resourcesConfig.TlarefsToIdsPath == null)
            return;

        _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));

        _resourcesConfig.TlarefsToIdsPath = LoaderHelper.CheckFileRequirements(resourcesConfig.TlarefsToIdsPath,
            nameof(resourcesConfig.TlarefsToIdsPath));
    }

    /// <summary>
    ///     Imports the TlarefsToIds dict using the resources config given to the constructor
    /// </summary>
    /// <returns>Dict that maps Tlarefs to a list of IDs for that stop</returns>
    public Dictionary<string, List<int>> ImportTlarefsToIds()
    {
        if (_resourcesConfig == null)
            return new Dictionary<string, List<int>>();

        using var reader = new StreamReader(_resourcesConfig.TlarefsToIdsPath!);
        var jsonString = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonString)!;
    }
}