using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RestAPI2.ValidationAttriputes;
namespace RestAPI2.Models
{
    [TitleAndDescriptionShouldn_tMatch]
    public class CourseForCreationDto :CourseForManipulationDto// IValidatableObject
    {


       

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "the title and description shiuldn't be the same",
        //            new[] { "CourseForCreatioDto" });

        //    }
        //}
    }
}
