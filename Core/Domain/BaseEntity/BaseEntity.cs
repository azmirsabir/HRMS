using System.ComponentModel.DataAnnotations;
using Core.Domain.Users;

namespace Core.Domain.BaseEntity
{
    public class BaseEntity<T>
    {
        [Key] public T Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
