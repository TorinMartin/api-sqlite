using System.ComponentModel.DataAnnotations;

namespace MusicApi.Model;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
}