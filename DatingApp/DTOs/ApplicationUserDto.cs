using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.DTOs
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}