using System.ComponentModel.DataAnnotations;

namespace MusicApi.Model;

public class Song : BaseEntity
{
    [Required]
    public required int Track { get; set; }
    
    [Required, MaxLength(256)]
    public required string Name { get; set; }
    
    [Required]
    public required Album Album { get; set; }
}