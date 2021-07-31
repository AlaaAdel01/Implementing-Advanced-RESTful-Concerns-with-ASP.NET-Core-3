using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Models
{
    public class AuthorCreationDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MainCategory { get; set; }

        public ICollection<CourseForCreationDto> courses { get; set; }
        = new List<CourseForCreationDto>();
    }
}
