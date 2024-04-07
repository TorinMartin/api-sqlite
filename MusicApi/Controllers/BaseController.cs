using Microsoft.AspNetCore.Mvc;
using MusicApi.Services;

namespace MusicApi.Controllers;

public abstract class BaseController : ControllerBase
{
    protected ValueTask<IActionResult> HandleServiceResult<T>(ServiceResult<T> result) => new (result.HasError ? BadRequest(result.Error) : Ok(result.Result));
}