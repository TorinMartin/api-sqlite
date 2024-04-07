using Microsoft.AspNetCore.Mvc;
using MusicApi.Services;

namespace MusicApi.Controllers;

public record AlbumCreationRequest(string Name, int YearReleased, int ArtistId);

[ApiController]
[Route("v1/albums")]
public class AlbumController : BaseController
{
    private readonly AlbumService _albumService;
    
    public AlbumController(AlbumService albumService)
    {
        _albumService = albumService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _albumService.GetAsync(a => a.Artist);
        return await HandleServiceResult(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAlbumAsync([FromBody]AlbumCreationRequest request)
    {
        var result = await _albumService.CreateAsync(request);
        return await HandleServiceResult(result);
    }
}