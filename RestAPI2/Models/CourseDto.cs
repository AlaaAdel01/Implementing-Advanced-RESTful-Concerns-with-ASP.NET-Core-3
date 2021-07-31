using RESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Models
{
    public class CourseDto
    {
        public Guid Id { get; set; }

        
        public string Title { get; set; }

 
        public string Description { get; set; }

        public AuthorDto authorDto { get; set; }
    }
}
