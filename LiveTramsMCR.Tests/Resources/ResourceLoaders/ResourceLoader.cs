namespace LiveTramsMCR.Tests.Resources.ResourceLoaders;

/// <summary>
///     Loads the file resources for the system using the file loaders.
/// </summary>
public class ResourceLoader
{
    private readonly ResourcesConfig _config;
    private readonly RouteTimesLoader _routeTimesLoader;
    private readonly RouteV2Loader _routeV2Loader;
    private readonly StationNamesToTlarefLoader _stationNamesToTlarefLoader;
    private readonly StopLoader _stopLoader;
    private readonly StopV2Loader _stopV2Loader;
    private readonly TlarefToIdsLoader _tlarefToIdsLoader;

    /// <summary>
    ///     Creates a new resources loader using the given resources configuration.
    ///     No null checks are performed on the resource config given.
    /// </summary>
    /// <param name="resourcesConfig">Resource config used for importing resources</param>
    public ResourceLoader(ResourcesConfig resourcesConfig)
    {
        _config = resourcesConfig;
        _stopLoader = new StopLoader(resourcesConfig);
        _stopV2Loader = new StopV2Loader(resourcesConfig);
        _stationNamesToTlarefLoader = new StationNamesToTlarefLoader(resourcesConfig);
        _tlarefToIdsLoader = new TlarefToIdsLoader(resourcesConfig);
        _routeTimesLoader = new RouteTimesLoader(resourcesConfig);
        _routeV2Loader = new RouteV2Loader(resourcesConfig);
    }

    /// <summary>
    ///     Loads the system resources into a ImportedResources object.
    /// </summary>
    /// <returns>ImportedResources for the system</returns>
    public ImportedResources ImportResources()
    {
        var importedStops = _stopLoader.ImportStops();
        var routeLoader = new RouteLoader(_config, importedStops);
        return new ImportedResources
        {
            ImportedStops = importedStops,
            ImportedStopsV2 = _stopV2Loader.ImportStops(),
            StationNamesToTlaref = _stationNamesToTlarefLoader.ImportStationNamesToTlarefs(),
            TlarefsToIds = _tlarefToIdsLoader.ImportTlarefsToIds(),
            ImportedRoutes = routeLoader.ImportRoutes(),
            ImportedRouteTimes = _routeTimesLoader.ImportRouteTimes(),
            ImportedRoutesV2 = _routeV2Loader.ImportRoutes()
        };
    }
}