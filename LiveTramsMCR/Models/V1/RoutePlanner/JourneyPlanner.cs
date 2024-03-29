using System.Collections.Generic;
using LiveTramsMCR.Models.V1.RoutePlanner.Data;
using LiveTramsMCR.Models.V1.Stops;

namespace LiveTramsMCR.Models.V1.RoutePlanner;

/// <summary>
///     Plans routes between Stop objects.
/// </summary>
public class JourneyPlanner : IJourneyPlanner
{
    private readonly JourneyTimeFinder _journeyTimeFinder;
    private readonly RouteIdentifier _routeIdentifier;

    /// <summary>
    ///     Create a new route planner with a list of available routes.
    /// </summary>
    /// <param name="routeRepository">Repository for retrieving route times</param>
    public JourneyPlanner(IRouteRepository routeRepository)
    {
        _routeIdentifier = new RouteIdentifier(routeRepository);
        _journeyTimeFinder = new JourneyTimeFinder(routeRepository);
    }

    /// <summary>
    ///     Finds a route between an Origin and Destination Stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of Journey</param>
    /// <returns>List of possible Planned Routes</returns>
    public PlannedJourney PlanJourney(Stop origin, Stop destination)
    {
        var interchangeIsRequired = _routeIdentifier.IsInterchangeRequired(origin, destination);
        var plannedJourney = interchangeIsRequired
            ? PlanJourneyWithInterchange(origin, destination)
            : PlanJourneyWithoutInterchange(origin, destination);

        plannedJourney.RequiresInterchange = interchangeIsRequired;
        plannedJourney.OriginStop = origin;
        plannedJourney.DestinationStop = destination;
        plannedJourney.TotalJourneyTimeMinutes =
            plannedJourney.MinutesFromOrigin + plannedJourney.MinutesFromInterchange;
        return plannedJourney;
    }


    /// <summary>
    ///     Plans a journey where it is known an interchange is not required.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>Planned journey without an interchange between origin and destination stops</returns>
    private PlannedJourney PlanJourneyWithoutInterchange(Stop origin, Stop destination)
    {
        var originRoutes = _routeIdentifier.IdentifyRoutesBetween(origin, destination);
        var originStops = _routeIdentifier
            .IdentifyIntermediateStops(origin, destination, originRoutes[0]);
        var terminiFromOrigin = new HashSet<Stop>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(_routeIdentifier
                .IdentifyRouteTerminus(origin, destination, route));
        }

        var minutesFromOrigin = IdentifyJourneyTime(originRoutes[0], origin, destination);
        return new PlannedJourney
        {
            RoutesFromOrigin = originRoutes,
            StopsFromOrigin = originStops,
            TerminiFromOrigin = terminiFromOrigin,
            MinutesFromOrigin = minutesFromOrigin
        };
    }

    /// <summary>
    ///     Plans a journey where it is known an interchange is required.
    ///     This identifies and handles the interchange stop.
    /// </summary>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">End of journey</param>
    /// <returns>Journey information including relevant </returns>
    private PlannedJourney PlanJourneyWithInterchange(Stop origin, Stop destination)
    {
        var interchangeStop = _routeIdentifier.IdentifyInterchangeStop(origin, destination);
        var originRoutes = _routeIdentifier.IdentifyRoutesBetween(origin, interchangeStop);
        var originStops = _routeIdentifier
            .IdentifyIntermediateStops(origin, interchangeStop, originRoutes[0]);
        var terminiFromOrigin = new HashSet<Stop>();
        foreach (var route in originRoutes)
        {
            terminiFromOrigin.Add(_routeIdentifier
                .IdentifyRouteTerminus(origin, interchangeStop, route));
        }


        var interchangeRoutes = _routeIdentifier.IdentifyRoutesBetween(interchangeStop, destination);
        var interchangeStops = _routeIdentifier
            .IdentifyIntermediateStops(interchangeStop, destination, interchangeRoutes[0]);
        var terminiFromInterchange = new HashSet<Stop>();
        foreach (var route in interchangeRoutes)
        {
            terminiFromInterchange.Add(_routeIdentifier
                .IdentifyRouteTerminus(interchangeStop, destination, route));
        }

        var minutesFromOrigin = IdentifyJourneyTime(originRoutes[0], origin, interchangeStop);
        var minutesFromInterchange = IdentifyJourneyTime(interchangeRoutes[0],
            interchangeStop, destination);
        return new PlannedJourney
        {
            InterchangeStop = interchangeStop,
            RoutesFromOrigin = originRoutes,
            RoutesFromInterchange = interchangeRoutes,
            StopsFromOrigin = originStops,
            StopsFromInterchange = interchangeStops,
            TerminiFromOrigin = terminiFromOrigin,
            TerminiFromInterchange = terminiFromInterchange,
            MinutesFromOrigin = minutesFromOrigin,
            MinutesFromInterchange = minutesFromInterchange
        };
    }

    /// <summary>
    ///     Identifies the journey time between an origin and interchange / destination stop.
    /// </summary>
    /// <param name="route">Route being taken</param>
    /// <param name="origin">Start of journey</param>
    /// <param name="destination">Destination / Interchange of journey</param>
    /// <returns>Integer of minutes between origin and destination</returns>
    private int IdentifyJourneyTime(Route route, Stop origin, Stop destination)
    {
        return _journeyTimeFinder.FindJourneyTime(route.Name,
            origin.StopName, destination.StopName);
    }
}