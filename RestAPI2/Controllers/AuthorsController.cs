using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RestAPI2.Helper;
using RestAPI2.Models;
using RestAPI2.ResourceParametres;
using RestAPI2.Services;
using RESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RESTfulAPI.Controllers
{
    [ApiController]
    [Route("api/authors")]
    
    public class AuthorsController : ControllerBase
    {
        public AuthorsController(ICourseLibraryRepository courseLibraryRepository
           , IMapper mapper,IPropertyMappingServices propertyMappingServices,IPropertyCheckerService propertyCheckerService)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper;
            this.propertyMappingServices = propertyMappingServices;
            this.propertyCheckerService = propertyCheckerService;
        }

        public readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper mapper;
        private readonly IPropertyMappingServices propertyMappingServices;
        private readonly IPropertyCheckerService propertyCheckerService;

        [HttpGet(Name ="GetAuthors")]
        [HttpHead]     
        
        public IActionResult GetAuthors([FromQuery]AuthorsResourceParametres authorsResourceParametres)
        {

            if(!propertyMappingServices.ValidMappingService<AuthorDto,Author>(authorsResourceParametres.orderBy))
            {
                return BadRequest();
            }

            if (!propertyCheckerService.TypeHasProperties<AuthorDto>(authorsResourceParametres.Fields))
            {
                return BadRequest();
            }
            var authorfromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParametres);
           
            var paginationMetdata = new
            {
                totalCount = authorfromRepo.totalCount,
                pageSize = authorfromRepo.PageSize,
                currentPage = authorfromRepo.currentPage,
                totalPages = authorfromRepo.totalPages,
               
            };

            Response.Headers.Add("x-pagination", JsonSerializer.Serialize(paginationMetdata));

            var links = CreateLinksForAuthors(authorsResourceParametres ,authorfromRepo.HasNext,authorfromRepo.HasPrevios);
            var shapedAuthors = mapper.Map<IEnumerable<AuthorDto>>(authorfromRepo).ShapeData(authorsResourceParametres.Fields);

            var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
            {
                var authorAsDictionary = author as IDictionary<string, object>;
                var authorLinks = CreateLinkForAuthors((Guid)authorAsDictionary["Id"], null);
                authorAsDictionary.Add("links", authorLinks);
                return authorAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedAuthorsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
          

        }
        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId,string fields,
                    
            [FromHeader(Name ="Acceot")] string mediaType)

        {


            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parseMediaType))
            {
                return BadRequest();
            }
            var authorfromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorfromRepo == null)
            {
                return NotFound();
            }

            if (!propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }

            if (parseMediaType.MediaType == "application/vnd.marvin.hateos+json")
            {
                var links = CreateLinkForAuthors(authorId, fields);

                var linkedResourceToReturn = (mapper.Map<AuthorDto>(authorfromRepo).ShapeDataObiect(fields))
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);
                return Ok(linkedResourceToReturn);
            }

            return Ok(mapper.Map<AuthorDto>(authorfromRepo).ShapeDataObiect(fields));
                   
        }

        [HttpPost(Name ="CreateAuthor")]
        public ActionResult<AuthorDto> CreateAuthor(AuthorCreationDto auhtor)
        {
            var authorEntity = mapper.Map<CourseLibrary.API.Entities.Author>(auhtor);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            
            var authorToReturn = mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinkForAuthors(authorToReturn.Id, null);
            var linksToReturn = authorToReturn.ShapeDataObiect(null) as IDictionary<string, object>;
            linksToReturn.Add("links", links);
            
            return CreatedAtRoute("GetAuthor", new { authorId = linksToReturn["Id"] }, linksToReturn);


        }


        [HttpOptions]
        public ActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET.Options,Post");
            return Ok();
        }


        private IEnumerable<LinkDto> CreateLinkForAuthors(Guid authorId, string fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetAuthor", new { authorId }),
                    "self", "Get"));
            }

            else
            {
                links.Add(
                  new LinkDto(Url.Link("GetAuthor", new { authorId, fields }),
                    "self,",
                    "get"));
            }

            links.Add(
                new LinkDto(Url.Link("DeleteAuthor", new { authorId }),
                "delete_Author",
                "DELETE"));


            links.Add(
                new LinkDto(Url.Link("CreateCourseForAuthor", new { authorId }),
                "CreateCourse",
                "POST"));


            links.Add(
                new LinkDto(Url.Link("GetCourseForAuthor", new { authorId }),
                "GetCourse",
                "GET"));


            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParametres authorsResourceParametres,
            bool hasNext,bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParametres, ResourceUriType.Current),
                "self", "GET"));

            if(hasNext)
            {
                links.Add
                    (new LinkDto(CreateAuthorsResourceUri(authorsResourceParametres, ResourceUriType.NextPage), "NextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add
                    (new LinkDto(CreateAuthorsResourceUri(authorsResourceParametres, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }

        [HttpDelete("{authorId}",Name ="DeleteAuthor")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);
            if (authorFromRepo ==null)
            {
                return NotFound();
            }
            _courseLibraryRepository.DeleteAuthor(authorFromRepo);
            _courseLibraryRepository.Save();
            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParametres authorsResourceParametres,ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields=authorsResourceParametres.Fields,
                            orderBy=authorsResourceParametres.orderBy,
                            pageNumber = authorsResourceParametres.PageNumber-1,
                            pageSize = authorsResourceParametres.PageSize,
                            mainCategory = authorsResourceParametres.mainCategory,
                            searchQuery = authorsResourceParametres.querySearch
                        });
                    
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParametres.Fields,
                            orderBy = authorsResourceParametres.orderBy,
                            pageNumber = authorsResourceParametres.PageNumber + 1,
                            pageSize = authorsResourceParametres.PageSize,
                            mainCategory = authorsResourceParametres.mainCategory,
                            searchQuery = authorsResourceParametres.querySearch
                        });
              case ResourceUriType.Current:
                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParametres.Fields,
                            orderBy = authorsResourceParametres.orderBy,
                            pageNumber = authorsResourceParametres.PageNumber ,
                            pageSize = authorsResourceParametres.PageSize,
                            mainCategory = authorsResourceParametres.mainCategory,
                            searchQuery = authorsResourceParametres.querySearch
                        });
                    
            }
        }
    }

    
}
