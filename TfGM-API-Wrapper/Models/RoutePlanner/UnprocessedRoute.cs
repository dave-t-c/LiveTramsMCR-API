using System.Collections.Generic;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// POCO Object used for storing routes as they are imported from the resource JSON
/// </summary>
public class UnprocessedRoute
{
    /// <summary>
    /// Name of the route, e.g. Purple
    /// </summary>
    public string name { get; set; }
    
    /// <summary>
    /// Hex string of the route colour, e.g. "#7B2082"
    /// </summary>
    public string colour { get; set; }
    
    /// <summary>
    /// List of stops on the line, in order from one destination to the other
    /// </summary>
    public List<string> stops { get; set; }
}