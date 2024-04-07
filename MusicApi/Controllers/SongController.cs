using Microsoft.AspNetCore.Mvc;
using MusicApi.Services;

namespace MusicApi.Controllers;

public record SongCreationRequest(int Track, string Name, int AlbumId);
public record SongUpdateRequest(int Id, int? Track, string? Name, int? AlbumId);

[ApiController]
[Route("v1/songs")]
public class SongController : BaseController
{
    private readonly SongService _songService;

    public SongController(SongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _songService.GetAsync(s => s.Album, s => s.Album.Artist);
        return await HandleServiceResult(result);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var result = await _songService.GetAsync(id, s => s.Album, s => s.Album.Artist);
        return await HandleServiceResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddSongAsync([FromBody]SongCreationRequest request)
    {
        var result = await _songService.CreateAsync(request);
        return await HandleServiceResult(result);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteSongAsync(int id)
    {
        var result = await _songService.DeleteAsync(id);
        return await HandleServiceResult(result);
    }

    [HttpPut("id")]
    public async Task<IActionResult> UpdateSongAsync(SongUpdateRequest request)
    {
        var result = await _songService.UpdateAsync(request);
        return await HandleServiceResult(result);
    }
}