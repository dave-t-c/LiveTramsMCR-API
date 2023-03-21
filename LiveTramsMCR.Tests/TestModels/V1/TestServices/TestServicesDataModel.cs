using System;
using System.Diagnostics;
using System.Linq;
using LiveTramsMCR.Models.V1.Resources;
using LiveTramsMCR.Models.V1.Services;
using LiveTramsMCR.Tests.Mocks;
using LiveTramsMCR.Tests.Resources.ResourceLoaders;
using NUnit.Framework;
using ImportedResources = LiveTramsMCR.Tests.Resources.ResourceLoaders.ImportedResources;
using ResourcesConfig = LiveTramsMCR.Tests.Resources.ResourceLoaders.ResourcesConfig;

namespace LiveTramsMCR.Tests.TestModels.V1.TestServices;

public class TestServicesDataModel
{
    private ResourcesConfig? _resourcesConfig;
    private ImportedResources? _importedResources;
    private IRequester? _requester;
    private ServicesDataModel? _servicesDataModel;
    private MockStopsRepository? _mockStopsRepository;

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
        _requester = new MockServiceRequester();
        _mockStopsRepository = new MockStopsRepository(_importedResources.ImportedStops);
        _servicesDataModel = new ServicesDataModel(_mockStopsRepository, _requester);
    }

    [TearDown]
    public void TearDown()
    {
        _resourcesConfig = null;
        _importedResources = null;
        _requester = null;
        _servicesDataModel = null;
    }

    /// <summary>
    /// Request services for 'BMR".
    /// This is one of the values returned in the mock service requester, with 1 destination expected.
    /// </summary>
    [Test]
    public void TestRequestExpectedService()
    {
        var result = _servicesDataModel?.RequestServices("BMR");
        Assert.NotNull(result);
        Debug.Assert(result != null, nameof(result) + " != null");
        Assert.AreEqual(1, result.Destinations.Count);
    }

    /// <summary>
    /// Request services for a null stop.
    /// This should throw an argument null exception.
    /// </summary>
    [Test]
    public void TestNullServiceLocationName()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>()
                .And.Message.EqualTo("Value cannot be null. (Parameter 'stop')"),
            delegate
            {
                Debug.Assert(_servicesDataModel != null, nameof(_servicesDataModel) + " != null");
                _servicesDataModel.RequestServices(null);
            });
    }

    /// <summary>
    /// Request departure board services.
    /// This should return 3 trams matching the destinations and wait times.
    /// </summary>
    [Test]
    public void TestRequestDepartureBoardServices()
    {
        var result = _servicesDataModel?.RequestDepartureBoardServices("BMR");
        Assert.IsNotNull(result);
        var trams = result?.Trams;
        Assert.AreEqual(3, trams?.Count);
        Assert.AreEqual(3, trams?.Distinct().Count());
        var firstTram = trams?.First();
        Assert.AreEqual("0", firstTram?.Wait);
        var finalTram = trams?.Last();
        Assert.AreEqual("23", finalTram?.Wait);
    }
    
}