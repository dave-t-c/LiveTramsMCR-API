using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using TfGM_API_Wrapper.Models;
using TfGM_API_Wrapper.Models.Resources;
using TfGM_API_Wrapper.Models.Stops;

namespace TfGM_API_Wrapper.Controllers;

/// <summary>
/// Controller for Stops related information
/// </summary>
[Route("/api/stops")]
[ApiController]
public class StopsController : Controller
{
    private readonly StopsDataModel _stopsDataModel;

    /// <summary>
    ///     Controller Constructor that allows for custom stops path file location.
    ///     This can be used for testing the controller.
    ///     Default = Default location for stop data file. This should ideally be
    ///     extracted to the properties file.
    /// </summary>
    /// <param name="stopsDataModel">Data Model used for processing stops information</param>
    public StopsController(StopsDataModel stopsDataModel)
    {
        _stopsDataModel = stopsDataModel;
    }

    /// <summary>
    ///     Returns a JSON List of all Stops.
    /// </summary>
    /// <returns>JSON List -> Stop</returns>
    [Route("/api/stops")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult GetAllStops()
    {
        return Ok(_stopsDataModel.GetStops());
    }
}