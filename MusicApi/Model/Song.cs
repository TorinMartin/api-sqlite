namespace MusicApi.Model;

public class Song : BaseEntity
{
    public int Track { get; set; }
    public string Name { get; set; } = string.Empty;
    public Album Album { get; set; } = null!;
}