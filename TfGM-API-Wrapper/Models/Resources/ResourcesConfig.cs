namespace TfGM_API_Wrapper.Models.Resources;

/// <summary>
///     POCO for Resources Information.
///     This is injected into the StopsController.
/// </summary>
public class ResourcesConfig
{
    /// <summary>
    /// Path for the json file containing stops.
    /// </summary>
    public string StopResourcePath { get; set; }
    
    /// <summary>
    /// Path for file that maps the station names to their associated tlaref.
    /// </summary>
    public string StationNamesToTlarefsPath { get; set; }
    
    /// <summary>
    /// Path for file that maps a stations tlaref to it's associated IDs.
    /// </summary>
    public string TlarefsToIdsPath { get; set; }

    /// <summary>
    ///     Creates a Deep Copy of this resource config object.
    /// </summary>
    /// <returns>A Deep Copy of the object.</returns>
    public ResourcesConfig DeepCopy()
    {
        return new ResourcesConfig
        {
            StopResourcePath = StopResourcePath,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath
        };
    }
}