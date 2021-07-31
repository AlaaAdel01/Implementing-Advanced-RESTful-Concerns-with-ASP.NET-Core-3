using AutoMapper;
using RestAPI2.Models;
using RESTfulAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Profiles
{
    public class AuthorsProfile:Profile

    {
        public AuthorsProfile()
        {
            CreateMap<CourseLibrary.API.Entities.Author, Models
                .AuthorDto>()
                .ForMember(
                dest=>dest.Name,
                opt=>opt.MapFrom(src=> $"{src.FirstName} {src.LastName}")
                )
                .ForMember(dest=>dest.Age,
                opt=>opt.MapFrom(src=>src.DateOfBirth.GetCurrentAge()));


            CreateMap<AuthorCreationDto, CourseLibrary.API.Entities.Author>();
                
        }
    }
}
