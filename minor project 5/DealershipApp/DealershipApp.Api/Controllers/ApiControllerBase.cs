using DealershipApp.Business;
using Microsoft.AspNetCore.Mvc;

namespace DealershipApp.Api.Controllers;

/// <summary>
/// Shared helper that turns a business-layer SaveResult into the right HTTP response.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// 201 Created after an insert, 200 OK after an update,
    /// 400 Bad Request with field messages when validation fails, 404 Not Found for an unknown ID.
    /// </summary>
    /// <param name="result">What the business layer did.</param>
    /// <param name="loadSaved">Reads the saved record back, so the client sees exactly what was stored.</param>
    protected async Task<ActionResult<TModel>> SaveResponseAsync<TModel>(
        SaveResult result,
        Func<int, Task<TModel?>> loadSaved) where TModel : class
    {
        if (result.Status == SaveStatus.Invalid)
        {
            var problem = new ValidationProblemDetails(new Dictionary<string, string[]>(result.Errors))
            {
                Title = "The record was not saved because some values are invalid.",
                Status = StatusCodes.Status400BadRequest
            };
            return BadRequest(problem);
        }

        if (result.Status == SaveStatus.NotFound)
        {
            return NotFound();
        }

        var saved = await loadSaved(result.Id);
        if (saved is null)
        {
            return NotFound();
        }

        if (result.Status == SaveStatus.Inserted)
        {
            return CreatedAtAction("GetById", new { id = result.Id }, saved);
        }

        return Ok(saved);
    }
}
