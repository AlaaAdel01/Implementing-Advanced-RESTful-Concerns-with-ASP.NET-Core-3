using AutoMapper;
using CourseLibrary.API.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using RestAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Controllers
{
    [Route("api/authors/{authorId}/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository
           , IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository;
            this.mapper = mapper;
        }

        [HttpGet(Name =
            "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();

            }
            var courseForAuthoeRepo = courseLibraryRepository.GetCourses(authorId);
            return Ok(mapper.Map<IEnumerable<CourseDto>>(courseForAuthoeRepo));
        }


        [HttpGet("{courseId}",Name = "GetCourseForAuthor")]
       // [ResponseCache (Duration=120)]
       [HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =1000)]
       [HttpCacheValidation(MustRevalidate =false)]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();

            }
            var courseForAuthoeRepo = courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseForAuthoeRepo == null)
            {
                return NotFound();

            }
            return Ok(mapper.Map<CourseDto>(courseForAuthoeRepo));

        }

        [HttpPost(Name ="CreateCourseForAuthor")]
        public ActionResult<CourseDto> createCourseForAuthor(Guid authorId, CourseForCreationDto course)

        {
            if (!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var CourseEntity = mapper.Map<CourseLibrary.API.Entities.Course>(course);

            courseLibraryRepository.AddCourse(authorId, CourseEntity);
            courseLibraryRepository.Save();

            var courseToReturn = mapper.Map<CourseDto>(CourseEntity);
            return CreatedAtRoute("GetCourseForAuthor", new { autherid = authorId, courseId = CourseEntity.Id }, courseToReturn);

        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForDto(Guid authorId, Guid courseId, CourseForUpdateDto courseForUpdateDto)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = courseLibraryRepository.GetCourse(authorId, courseId);
            if (course == null)
            {
                var courseToAdd = mapper.Map<CourseLibrary.API.Entities.Course>(course);
                courseToAdd.Id = courseId;

                courseLibraryRepository.AddCourse(authorId, courseToAdd);
                courseLibraryRepository.Save();


                var courseToReturn = mapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseForAuthor", new { authorId, courseToAdd }, courseToReturn);
            }

            mapper.Map(course, courseForUpdateDto);
            courseLibraryRepository.UpdateCourse(course);

            courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartialUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = courseLibraryRepository.GetCourse(authorId, courseId);
            if (course == null)
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto,ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }
                var coursetoadd = mapper.Map<CourseLibrary.API.Entities.Course>(courseDto);
                coursetoadd.Id = courseId;

                courseLibraryRepository.AddCourse(authorId,coursetoadd);
                courseLibraryRepository.Save();

                var courseToReturn = mapper.Map<CourseDto>(coursetoadd);
                return CreatedAtRoute("GetCourseforAuthor", new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }
           
          var courseToPatch  = mapper.Map<CourseForUpdateDto>(course);
             patchDocument.ApplyTo(courseToPatch,ModelState);
            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }
            mapper.Map(courseToPatch, course);
            courseLibraryRepository.UpdateCourse(course);
            courseLibraryRepository.Save();
            return NoContent();


        }


        public override ActionResult ValidationProblem([ActionResultObjectValue]ModelStateDictionary modelstate)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }


        [HttpDelete("{courseId}")]
        public ActionResult DEleteCourseforAuthor(Guid authorId,Guid courseId)
        {
            if(!courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);
            if(courseFromRepo ==null)
            {
                return NotFound();
            }

            courseLibraryRepository.DeleteCourse(courseFromRepo);
            courseLibraryRepository.Save();
            return NoContent();
                   
        }
    }
}

