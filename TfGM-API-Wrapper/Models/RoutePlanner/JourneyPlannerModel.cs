using System;
using System.Collections.Generic;
using System.Linq;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Models.RoutePlanner;

/// <summary>
/// Data model for the Journey Planner package.
/// This identifies the used stops and uses the Journey planner
/// class to plan a journey.
/// </summary>
public class JourneyPlannerModel: IJourneyPlannerModel
{
    private readonly IJourneyPlanner _journeyPlanner;
    private ImportedResources _importedResources;
    private readonly StopLookup _stopLookup;
    
    /// <summary>
    /// Creates a journey planner model that can be used
    /// for planning journeys between stops.
    /// </summary>
    /// <param name="importedResources"></param>
    /// <param name="journeyPlanner"></param>
    public JourneyPlannerModel(ImportedResources importedResources, IJourneyPlanner journeyPlanner)
    {
        _importedResources = importedResources;
        _journeyPlanner = journeyPlanner;
        _stopLookup = new StopLookup(importedResources);
    }
    
    
    /// <summary>
    /// Plans a journey between an origin stop name or TLAREF and
    /// a destination stop name or TLAREF, identifying any relevant interchange information.
    /// </summary>
    /// <param name="origin">Journey start stop name or TLAREF</param>
    /// <param name="destination">Journey end stop name or TLAREF</param>
    /// <returns>Planned journey including relevant interchange information</returns>
    public PlannedJourney PlanJourney(string origin, string destination)
    {
        if (origin is null)
            throw new ArgumentNullException(nameof(origin));
        var originStop = _stopLookup.LookupStop(origin);
        var destinationStop = _stopLookup.LookupStop(destination);
        return _journeyPlanner?.PlanJourney(originStop, destinationStop);
    }
}