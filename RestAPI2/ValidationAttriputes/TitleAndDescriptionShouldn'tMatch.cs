using RestAPI2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.ValidationAttriputes
{
    public class TitleAndDescriptionShouldn_tMatch :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            var context = (CourseForManipulationDto)validationContext.ObjectInstance;

            if (context.Title==context.Description)
            {
                return new ValidationResult(ErrorMessage, 
                    new[] { nameof(CourseForManipulationDto) });
            }
            return ValidationResult.Success;

        }
    }
}
