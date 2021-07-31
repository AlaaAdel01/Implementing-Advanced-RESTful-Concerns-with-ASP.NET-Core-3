using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Models
{
    public abstract class  CourseForManipulationDto
    {

        [Required]
        [MaxLength(1500)]
        public string Title { get; set; }

       
        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}
