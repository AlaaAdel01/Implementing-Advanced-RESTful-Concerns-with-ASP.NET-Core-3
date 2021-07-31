using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RestAPI2.Helper;
using RestAPI2.Models;
using RESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorCollectionController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public AuthorCollectionController(ICourseLibraryRepository courseLibraryRepository,IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAutorCollection(IEnumerable<AuthorCreationDto> Authors)
        {
            var AuthorCollection = mapper.Map<IEnumerable<Author>>(Authors);

            foreach(var author in AuthorCollection )
            {
                courseLibraryRepository.AddAuthor(author);
            }
            courseLibraryRepository.Save();

            var authorsToReturn = mapper.Map<IEnumerable<AuthorDto>>(Authors);
            var idsAsString = string.Join(",", authorsToReturn.Select(a => a.Id)).ToString();
            return CreatedAtRoute("GetAuthorsCollection", new { ids = idsAsString }, authorsToReturn);

          
                
        }

        [HttpGet("({ids})", Name = "GetAuthorsCollection")]
        public IActionResult GetAuthorsCollection(
            [FromRoute]
            [ModelBinder (binderType:typeof(ArrayModelBinder))]IEnumerable<Guid>ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var entities = courseLibraryRepository.GetAuthors(ids);

            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }


            var authorsToReturn = mapper.Map<IEnumerable<AuthorDto>>(entities);

            return Ok(authorsToReturn);

        }
    }
}
