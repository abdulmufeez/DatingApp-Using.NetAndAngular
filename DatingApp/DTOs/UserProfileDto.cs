using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Entities;

namespace DatingApp.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        [Required]
        public string KnownAs { get; set; }
        public DateTime ProfileCreatedAt { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }          
        public int ApplicationUserId { get; set; }
    }
}