using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infra.Domain.Entities
{
    [Index(nameof(Identifier), IsUnique = true)]
    public abstract class BaseEntity : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string Identifier { get; set; } = Guid.NewGuid().ToString();
    }
}
