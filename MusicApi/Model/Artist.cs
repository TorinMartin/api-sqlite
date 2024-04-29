using System.ComponentModel.DataAnnotations;

namespace MusicApi.Model;

public class Artist : BaseEntity
{
    [Required, MaxLength(256)]
    public required string Name { get; set; }
}