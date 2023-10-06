using System.Collections.Generic;
using System.Linq;
using System.Net;
using LiveTramsMCR.Controllers.V2;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Models.V2.RoutePlanner.JourneyPlanner;
using LiveTramsMCR.Models.V2.RoutePlanner.Responses;
using LiveTramsMCR.Models.V2.RoutePlanner.Routes;
using LiveTramsMCR.Models.V2.RoutePlanner.ServiceInformation.NextService;
using LiveTramsMCR.Models.V2.RoutePlanner.Visualisation;
using LiveTramsMCR.Models.V2.Stops;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using LiveTramsMCR.Tests.TestModels.V1.TestServices;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestControllers.V2;

/// <summary>
/// Test class for the journey planner controller v2.
/// </summary>
public class TestJourneyPlannerControllerV2
{
    private const string JourneyPlannerV2WithInterchangeResponsePath = "../../../Resources/TestJourneyPlannerV2/AltrinchamPiccadillyAshtonServices.json";
    private const string JourneyPlannerV2ResponsePath = "../../../Resources/TestJourneyPlannerV2/AltrinchamSaleServices.json";
    private const string StopsV1ResourcePath = "../../../Resources/TestRoutePlanner/stops.json";
    private const string RoutesV2ResourcePath = "../../../Resources/RoutesV2.json";
    private const string StopsV2ResourcePath = "../../../Resources/StopsV2.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private List<StopV2> _importedStopV2S = new();
    private List<RouteV2> _importedRouteV2S = new();
    private IJourneyPlannerModelV2? _journeyPlannerModelV2Interchange;
    private IJourneyPlannerModelV2? _journeyPlannerModelV2;
    private JourneyPlannerControllerV2? _journeyPlannerControllerV2;
    private JourneyPlannerControllerV2? _journeyPlannerControllerV2Interchange;
    private IJourneyPlannerV2? _journeyPlannerV2;
    private INextServiceIdentifierV2? _nextServiceIdentifierV2;
    private ServiceProcessor? _serviceProcessor;
    private ServiceProcessor? _serviceProcessorWithInterchange;
    private StopLookupV2? _stopLookupV2;

    [SetUp]
    public void SetUp()
    {
        var resourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopsV1ResourcePath,
            RoutesV2ResourcePath = RoutesV2ResourcePath,
            RouteTimesPath = RouteTimesPath,
            StopV2ResourcePath = StopsV2ResourcePath
        };

        var stopsV1Loader = new StopLoader(resourcesConfig);
        var stopsV1 = stopsV1Loader.ImportStops();

        var mockStopsV1Repository = new MockStopsRepository(stopsV1);

        var stopsV2Loader = new StopV2Loader(resourcesConfig);
        _importedStopV2S = stopsV2Loader.ImportStops();

        var mockStopsV2Repository = new MockStopsRepositoryV2(_importedStopV2S);
        _stopLookupV2 = new StopLookupV2(mockStopsV2Repository);

        var routesV1Loader = new RouteLoader(resourcesConfig, stopsV1);
        var routesV1 = routesV1Loader.ImportRoutes();

        var routesV2Loader = new RouteV2Loader(resourcesConfig);
        _importedRouteV2S = routesV2Loader.ImportRoutes();

        var routeTimesLoader = new RouteTimesLoader(resourcesConfig);
        var routeTimes = routeTimesLoader.ImportRouteTimes();

        var mockRouteRepositoryV1 = new MockRouteRepository(routesV1, routeTimes);
        var mockRouteRepositoryV2 = new MockRouteRepositoryV2(_importedRouteV2S, mockStopsV2Repository);

        var journeyVisualiserV2 = new JourneyVisualiserV2();

        _journeyPlannerV2 = new JourneyPlannerV2(mockRouteRepositoryV1, mockRouteRepositoryV2);

        var mockInterchangeHttpResponse =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(
                HttpStatusCode.OK,
                JourneyPlannerV2WithInterchangeResponsePath);
        var mockHttpResponse = ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(
            HttpStatusCode.OK,
            JourneyPlannerV2ResponsePath);

        var serviceRequesterWithInterchange = new MockServiceRequester(mockInterchangeHttpResponse!);
        var serviceRequester = new MockServiceRequester(mockHttpResponse!);

        _serviceProcessorWithInterchange = new ServiceProcessor(serviceRequesterWithInterchange, mockStopsV1Repository);
        _serviceProcessor = new ServiceProcessor(serviceRequester, mockStopsV1Repository);
        _nextServiceIdentifierV2 = new NextServiceIdentifierV2();

        _journeyPlannerModelV2Interchange = new JourneyPlannerModelV2(
            _stopLookupV2,
            _journeyPlannerV2,
            journeyVisualiserV2,
            _nextServiceIdentifierV2,
            _serviceProcessorWithInterchange);

        _journeyPlannerModelV2 = new JourneyPlannerModelV2(
            _stopLookupV2,
            _journeyPlannerV2,
            journeyVisualiserV2,
            _nextServiceIdentifierV2,
            _serviceProcessor);

        _journeyPlannerControllerV2 = new JourneyPlannerControllerV2(_journeyPlannerModelV2);
        _journeyPlannerControllerV2Interchange = new JourneyPlannerControllerV2(_journeyPlannerModelV2Interchange);
    }

    [Test]
    public void TestJourneyPlanner()
    {
        var result = _journeyPlannerControllerV2?.PlanJourney("Altrincham", "Sale");
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult!.Value as JourneyPlannerV2ResponseBodyModel;
        Assert.IsNotNull(response);
        var plannedJourney = response?.PlannedJourney;
        Assert.IsNotNull(plannedJourney);
        Assert.IsFalse(plannedJourney?.RequiresInterchange);
        var altrinchamStop = _importedStopV2S.Single(stop => stop.Tlaref == "ALT");
        var saleStop = _importedStopV2S.Single(stop => stop.Tlaref == "SAL");
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(saleStop, plannedJourney?.DestinationStop);
    }

    [Test]
    public void TestJourneyPlannerWithInterchange()
    {
        var result = _journeyPlannerControllerV2Interchange?.PlanJourney(
            "Altrincham",
            "Ashton-Under-Lyne");
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult!.Value as JourneyPlannerV2ResponseBodyModel;
        Assert.IsNotNull(response);
        var plannedJourney = response?.PlannedJourney;
        Assert.IsNotNull(plannedJourney);
        Assert.IsTrue(plannedJourney?.RequiresInterchange);
        var altrinchamStop = _importedStopV2S.Single(stop => stop.Tlaref == "ALT");
        var ashtonStop = _importedStopV2S.Single(stop => stop.Tlaref == "ASH");
        var piccadillyStop = _importedStopV2S.Single(stop => stop.Tlaref == "PIC");
        Assert.AreEqual(altrinchamStop, plannedJourney?.OriginStop);
        Assert.AreEqual(ashtonStop, plannedJourney?.DestinationStop);
        Assert.AreEqual(piccadillyStop, plannedJourney?.InterchangeStop);
    }
}