using MusicApi.Controllers;
using MusicApi.Data;
using MusicApi.Injection;
using MusicApi.Model;

namespace MusicApi.Services;

public interface IAlbumService
{
    public Task<ServiceResult<CountResult>> CreateAsync(AlbumCreationRequest request);
}

[Injectable(ServiceLifetime.Transient, typeof(AlbumService))]
public class AlbumService : BaseService<Album>, IAlbumService
{
    private readonly IRepository<Artist> _artistRepo;
    
    public AlbumService(IRepository<Album> albumRepo, IRepository<Artist> artistRepo) : base(albumRepo)
    {
        _artistRepo = artistRepo;
    }
    
    public async Task<ServiceResult<CountResult>> CreateAsync(AlbumCreationRequest request)
    {
        var artist = await _artistRepo.GetAsync(request.ArtistId);
        if (artist is null) return await HandleServiceError<CountResult>("Invalid artist id");

        var album = new Album { Name = request.Name, YearReleased = request.YearReleased, Artist = artist };
        return await InsertAsync(album);
    }   
}