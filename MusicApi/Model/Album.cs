using System.ComponentModel.DataAnnotations;

namespace MusicApi.Model;

public class Album : BaseEntity
{
    [Required, MaxLength(256)]
    public required string Name { get; set; }
    
    [Required]
    public required int YearReleased { get; set; }

    [Required]
    public required Artist Artist { get; set; }
}