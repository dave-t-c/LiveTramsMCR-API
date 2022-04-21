using System.Collections;

namespace TfGM_API_Wrapper.Models.Stops;

/// <summary>
///     Stores information about a single stop.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
public class Stop
{
    public string StopName { get; set; }
    public string Tlaref { get; set; }
    public ArrayList Ids { get; set; }
    public string AtcoCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Street { get; set; }
    public string RoadCrossing { get; set; }
    public string Line { get; set; }
    public string StopZone { get; set; }
}