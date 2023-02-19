using System.Linq;
using LiveTramsMCR.Controllers;
using LiveTramsMCR.Models.Resources;
using LiveTramsMCR.Models.RoutePlanner;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers;

/// <summary>
/// Test class for the 
/// </summary>
public class TestJourneyPlannerController
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IJourneyPlanner? _journeyPlanner;
    private IJourneyPlannerModel? _journeyPlannerModel;
    private JourneyPlannerController? _journeyPlannerController;
    
    [SetUp]
    public void SetUp()
    {
        _resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = "../../../Resources/TestRoutePlanner/stops.json",
            StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json",
            TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json",
            RoutesResourcePath = "../../../Resources/TestRoutePlanner/routes.json",
            RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json"
        };
        _importedResources = new ResourceLoader(_resourcesConfig).ImportResources();
        _journeyPlanner = new JourneyPlanner(_importedResources.ImportedRoutes, _importedResources.ImportedRouteTimes);
        _journeyPlannerModel = new JourneyPlannerModel(_importedResources, _journeyPlanner);
        _journeyPlannerController = new JourneyPlannerController(_journeyPlannerModel);
    }
    
    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _journeyPlannerController = null;
    }

    
    /// <summary>
    /// Test to identify a planned journey between Altrincham and Piccadilly.
    /// This should return an Ok result with no interchange being required,
    /// with the expected start and end stops.
    /// </summary>
    [Test]
    public void TestHandleAltrinchamPiccadillyRoute()
    {
        var result = _journeyPlannerController?.PlanJourney("Altrincham", "Piccadilly");
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult?.StatusCode);
        var plannedJourney = okResult?.Value as PlannedJourney;
        Assert.NotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        var altrinchamStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Altrincham");
        var piccadillyStop = _importedResources?.ImportedStops.First(stop => stop.StopName == "Piccadilly");
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(piccadillyStop, plannedJourney?.DestinationStop);
    }

    /// <summary>
    /// Test to plan a journey with an invalid origin name.
    /// This should return a 400 bad request
    /// </summary>
    [Test]
    public void TestPlanJourneyInvalidOriginName()
    {
        var result = _journeyPlannerController?.PlanJourney("AAAA", "Piccadilly");
        Assert.NotNull(result);
        var resultObj = result as ObjectResult;
        Assert.NotNull(resultObj);
        Assert.AreEqual(400, resultObj?.StatusCode);
    }
}