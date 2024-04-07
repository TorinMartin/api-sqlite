namespace MusicApi.Model;

public class Album : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int YearReleased { get; set; }
    public Artist Artist { get; set; } = null!;
}