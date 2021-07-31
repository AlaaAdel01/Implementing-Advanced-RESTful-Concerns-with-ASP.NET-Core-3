using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Models
{
    public class CourseForUpdateDto:CourseForManipulationDto
    {
        [Required(ErrorMessage ="you should fill out the description")]
        public override string Description { get => base.Description    ; set => base.Description = value; }

    }
}
