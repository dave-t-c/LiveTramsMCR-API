using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LiveTramsMCR.Models.V1.Stops;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Class that represents a tram route between two Stops.
///     The stops are included as the Stop class, so all relevant information is available.
/// </summary>
[BsonIgnoreExtraElements]
public class Route
{

    /// <summary>
    ///     Creates a new route, the params are only null sanitised
    /// </summary>
    /// <param name="name">Route name, e.g. Purple</param>
    /// <param name="colour">Route hex colour, e.g. #7B2082</param>
    /// <param name="stops"></param>
    public Route(string name, string colour, List<Stop> stops)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Colour = colour ?? throw new ArgumentNullException(nameof(colour));
        Stops = stops ?? throw new ArgumentNullException(nameof(stops));
    }

    /// <summary>
    ///     Name of the route, e.g. "Purple"
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Hex colour string for the route, e.g. #7B2082
    /// </summary>
    public string Colour { get; set; }

    /// <summary>
    ///     Stops belonging to a route in the order they can be travelled between.
    /// </summary>
    public List<Stop> Stops { get; internal set; }

    /// <summary>
    ///     Identifies a list of stops that occur between two stops
    ///     E.g. for the route A -> B -> C -> D, stops between A and D
    ///     this will return B then C.
    /// </summary>
    /// <param name="start">Stop in list to start</param>
    /// <param name="end">Stop in list to end at</param>
    /// <returns>List of Interim Stops</returns>
    public List<Stop> GetStopsBetween(Stop start, Stop end)
    {
        _ = start ?? throw new ArgumentNullException(nameof(start));
        _ = end ?? throw new ArgumentNullException(nameof(end));
        if (!Stops.Contains(start))
            throw new InvalidOperationException("Route does not contain stop " + start.StopName);
        if (!Stops.Contains(end))
            throw new InvalidOperationException("Route does not contain stop " + end.StopName);
        var startIndex = Stops.IndexOf(start);
        var endIndex = Stops.IndexOf(end);
        // Use the differences in index to identify if we will be going up or down the list.
        var increment = endIndex > startIndex ? 1 : -1;
        var identifiedStops = new List<Stop>();
        for (var i = startIndex + increment; i != endIndex; i += increment)
        {
            identifiedStops.Add(Stops[i]);
        }
        return identifiedStops;
    }

    /// <summary>
    ///     Determines if this route contains a given Stop.
    /// </summary>
    /// <param name="stop">Stop to check membership of</param>
    /// <returns>True if route contains Stop, else false.</returns>
    public bool ContainsStop(Stop stop)
    {
        return Stops.Contains(stop);
    }
}