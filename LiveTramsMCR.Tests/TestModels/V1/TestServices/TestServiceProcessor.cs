using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

/// <summary>
///     Test class for the ServiceProcessor.
/// </summary>
public class TestServiceProcessor
{
    private const string StopResourcePathConst = "../../../Resources/TestRoutePlanner/stops.json";
    private const string StationNamesToTlarefsPath = "../../../Resources/Station_Names_to_TLAREFs.json";
    private const string TlarefsToIdsPath = "../../../Resources/TLAREFs_to_IDs.json";
    private const string RoutesPath = "../../../Resources/TestRoutePlanner/routes.json";
    private const string RouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";
    private const string InternalServerErrorResponsePath = "../../../Resources/ExampleApiInternalServerErrorResponse.json";
    private const string UpdateStopsStopResourcePathConst = "../../../Resources/TestStopUpdater/stops.json";
    private const string UpdateStopsRoutesResourcePath = "../../../Resources/TestStopUpdater/routes.json";
    private const string UpdateStopsRouteTimesPath = "../../../Resources/TestRoutePlanner/route-times.json";
    private const string UpdateStopsApiResponsePath = "../../../Resources/TestStopUpdater/ApiResponse.json";
    private ImportedResources? _importedResources;
    private ImportedResources? _updateStopsImportedResources;
    private MockServiceRequester? _mockServiceRequester;
    private MockServiceRequester? _mockServiceRequesterInternalServerError;
    private ResourceLoader? _resourceLoader;
    private ResourceLoader? _updateStopsResourceLoader;
    private ServiceProcessor? _serviceProcessor;
    private ServiceProcessor? _serviceProcessorInternalServerError;
    private ResourcesConfig? _validResourcesConfig;
    private ResourcesConfig? _updateStopsResourceConfig;
    private MockStopsRepository? _mockStopsRepository;
    private MockStopsRepository? _mockStopsRepositoryUpdateStops;
    private MockRouteRepository? _mockRouteRepositoryUpdateStops;
    private MockRouteRepository? _mockRouteRepository;

    [SetUp]
    public void SetUp()
    {
        _validResourcesConfig = new ResourcesConfig
        {
            StopResourcePath = StopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = RoutesPath,
            RouteTimesPath = RouteTimesPath
        };

        _updateStopsResourceConfig = new ResourcesConfig
        {
            StopResourcePath = UpdateStopsStopResourcePathConst,
            StationNamesToTlarefsPath = StationNamesToTlarefsPath,
            TlarefsToIdsPath = TlarefsToIdsPath,
            RoutesResourcePath = UpdateStopsRoutesResourcePath,
            RouteTimesPath = UpdateStopsRouteTimesPath
        };

        _resourceLoader = new ResourceLoader(_validResourcesConfig);
        _updateStopsResourceLoader = new ResourceLoader(_updateStopsResourceConfig);
        _importedResources = _resourceLoader.ImportResources();
        _updateStopsImportedResources = _updateStopsResourceLoader.ImportResources();

        var mockHttpResponse =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK, ValidApiResponsePath);

        var mockHttpResponseInternalServerError =
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.InternalServerError,
                InternalServerErrorResponsePath);
        
        var mockHttpResponseGetAllStops = 
            ImportServicesResponse.ImportHttpResponseMessageWithUnformattedServices(HttpStatusCode.OK,
                UpdateStopsApiResponsePath);
        
        _mockServiceRequester = new MockServiceRequester(mockHttpResponse!);

        _mockServiceRequesterInternalServerError = new MockServiceRequester(
            mockHttpResponseInternalServerError!, 
            mockHttpResponseGetAllStops!);
            
        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _mockStopsRepositoryUpdateStops = new MockStopsRepository(_updateStopsImportedResources.ImportedStops);

        _mockRouteRepository = new MockRouteRepository(_importedResources.ImportedRoutes, _importedResources.ImportedRouteTimes);
        _mockRouteRepositoryUpdateStops = new MockRouteRepository(_updateStopsImportedResources.ImportedRoutes,
            _updateStopsImportedResources.ImportedRouteTimes);
        
        _serviceProcessor = new ServiceProcessor(_mockServiceRequester, _mockStopsRepository);

        _serviceProcessorInternalServerError = new ServiceProcessor(
            _mockServiceRequesterInternalServerError, 
            _mockStopsRepositoryUpdateStops);
    }

    [TearDown]
    public void TearDown()
    {
        _mockServiceRequester = null;
        _validResourcesConfig = null;
        _resourceLoader = null;
        _importedResources = null;
        _mockServiceRequester = null;
        _serviceProcessor = null;
        _serviceProcessorInternalServerError = null;
    }


    /// <summary>
    ///     This should take a stop name, and then return the expected formatted services.
    ///     This will use tge MockServiceRequester so we can test an expected value as the data
    ///     will be static.
    ///     This should return FormattedServices.
    /// </summary>
    [Test]
    public void TestProcessServicesStopName()
    {
        Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
        var result = _serviceProcessor.RequestServices("BMR");
        Assert.NotNull(result);
        Assert.AreEqual(1, result.Destinations.Count);
    }

    /// <summary>
    ///     Test to use the service processor with a null stop name.
    ///     This should throw a null argument exception
    /// </summary>
    [Test]
    public void TestServiceProcessorNullName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                Debug.Assert(_serviceProcessor != null, nameof(_serviceProcessor) + " != null");
                _serviceProcessor.RequestServices(null);
            });
    }

    /// <summary>
    /// Test to request services in the format for a departure board.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoard()
    {
        var result = _serviceProcessor?.RequestDepartureBoardServices("BMR");
        Assert.IsNotNull(result);
        var trams = result?.Trams;
        Assert.IsNotNull(trams);
        Assert.AreEqual(3, trams?.Count);
        var firstTram = trams?.First();
        Assert.AreEqual("0", firstTram?.Wait);
        Assert.AreEqual(3, trams?.Distinct().Count());
        Assert.AreEqual("23", trams?.Last().Wait);
    }

    
    /// <summary>
    /// Test to try and request departure board services with a null stop name.
    /// This should throw an arg null exception.
    /// </summary>
    [Test]
    public void TestServicesDepartureBoardNullStop()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                _serviceProcessor?.RequestDepartureBoardServices(null);
            });
    }

    /// <summary>
    /// Test to try and retrieve live services when the IDs are outdated.
    /// This should update the IDs to that given in the mock response
    /// This should throw an invalid operation exception
    /// </summary>
    [Test]
    public void TestServiceIDsOutdated()
    {
        Assert.Throws(Is.TypeOf<InvalidOperationException>()
            .And.Message.EqualTo("Retry in 5s"), delegate
        {
            _serviceProcessorInternalServerError?.RequestServices("Example 1");
        });
    }
}