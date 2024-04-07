using System.Linq.Expressions;
using Moq;
using MusicApi.Controllers;
using MusicApi.Data;
using MusicApi.Model;
using MusicApi.Services;

namespace MusicApiTests;

public class AlbumServiceTests
{
    private AlbumService _albumService = null!;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var mockAlbumRepo = MockRepository<Album>();
        var mockArtistRepo = MockRepository<Artist>();
        
        _albumService = new AlbumService(mockAlbumRepo, mockArtistRepo);
    }

    [Test]
    public async Task Invalid_ArtistId_HasError()
    {
        const string expectedError = "Invalid artist id";
        
        var request = new AlbumCreationRequest("Shenanigans", 2002, 999);
        var result = await _albumService.CreateAsync(request);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error, Is.EqualTo(expectedError));
        });
    }
    
    private static IRepository<T> MockRepository<T>() where T : BaseEntity
    {
        var mock = new Mock<IRepository<T>>();
        mock.Setup(x => x.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((int id, Expression<Func<T, object>>[] includes) => null);
        mock.Setup(x => x.DeleteAsync(It.IsAny<int>())).ReturnsAsync(0);
        mock.Setup(x => x.InsertAsync(It.IsAny<T>())).ReturnsAsync(0);
        mock.Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<Action<T>>())).ReturnsAsync(0);
        return mock.Object;
    }
}