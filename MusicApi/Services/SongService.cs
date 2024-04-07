using MusicApi.Controllers;
using MusicApi.Data;
using MusicApi.Injection;
using MusicApi.Model;

namespace MusicApi.Services;

public interface ISongService
{
    public Task<ServiceResult<CountResult>> CreateAsync(SongCreationRequest request);
    public Task<ServiceResult<CountResult>> UpdateAsync(SongUpdateRequest request);
}

// Generic base service allows individual services to reuse very little code
[Injectable(ServiceLifetime.Transient, typeof(SongService))]
public class SongService : BaseService<Song>, ISongService
{
    private readonly IRepository<Album> _albumRepo;
    
    public SongService(IRepository<Song> songRepo, IRepository<Album> albumRepo) : base(songRepo)
    {
        _albumRepo = albumRepo;
    }

    public async Task<ServiceResult<CountResult>> CreateAsync(SongCreationRequest request)
    {
        var album = await _albumRepo.GetAsync(request.AlbumId);
        if (album is null) return await HandleServiceError<CountResult>("Invalid album id");
        
        var song = new Song { Track = request.Track, Name = request.Name, Album = album };
        return await InsertAsync(song);
    }

    public async Task<ServiceResult<CountResult>> UpdateAsync(SongUpdateRequest request)
    {
        Album? album = null;
        if (request.AlbumId is not null)
        {
            album = await _albumRepo.GetAsync((int)request.AlbumId);
            if (album is null) return await HandleServiceError<CountResult>("Invalid album id");
        }
        
        return await UpdateAsync(request.Id, song =>
        {
            if (request.Track is not null) song.Track = (int)request.Track;
            if (request.Name is not null) song.Name = request.Name;
            if (album is not null) song.Album = album;
        });
    }
}