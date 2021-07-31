using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Controllers
{
   
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet (Name ="GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self", "GET"));

            links.Add(
                new LinkDto(Url.Link("GetAuthors",new { }),"authors","GET"));

            links.Add
                (new LinkDto(Url.Link("CreateAuthor", new object { }), "CreateAuthor", "POST"));


            return Ok(links);
        }
    }
}
