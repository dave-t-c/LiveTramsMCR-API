using System;
using LiveTramsMCR.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiveTramsMCR.Controllers;

/// <summary>
///     Controller for handling service requests.
/// </summary>
[Route("/api/services")]
[ApiController]
public class ServiceController : Controller
{
    private readonly IServicesDataModel _servicesDataModel;

    /// <summary>
    ///     Secrets, such as access keys from the dotnet user-secrets storage
    ///     are loaded into the IConfiguration on start up.
    /// </summary>
    public ServiceController(IServicesDataModel servicesDataModel)
    {
        _servicesDataModel = servicesDataModel;
    }

    /// <summary>
    /// Retrieves the services for a given stop
    /// </summary>
    /// <param name="stop">Stop name or Tlaref for stop</param>
    /// <returns>FormattedServices -> Services for the specified stop</returns>
    [Route("/api/services/{stop}")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (FormattedServices), statusCode: StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Stop Name or TLAREF provided")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occured")]
    [HttpGet]
    public IActionResult GetService(string stop)
    {
        FormattedServices result;
        try
        {
            result = _servicesDataModel.RequestServices(stop);
        }
        catch (ArgumentException)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid Stop Name or TLAREF" });
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves services for a given stop in a departure board format.
    /// </summary>
    /// <param name="stop">Stop name or Tlaref</param>
    /// <returns>Service information formatted for use with a departure board</returns>
    [Route("/api/services/departure-boards/{stop}")]
    [Produces("application/json")]
    [SwaggerResponse (type:typeof (FormattedDepartureBoardServices), statusCode: StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Stop Name or TLAREF provided")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "An internal server error occured")]
    [HttpGet]
    public IActionResult GetDepartureBoardService(string stop)
    {
        FormattedDepartureBoardServices result;
        try
        {
            result = _servicesDataModel.RequestDepartureBoardServices(stop);
        }
        catch (ArgumentException)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Invalid Stop Name or TLAREF" });
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return Ok(result);
    }
}