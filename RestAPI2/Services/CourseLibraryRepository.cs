using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using RestAPI2.Helper;
using RestAPI2.ResourceParametres;
using RestAPI2.Services;
using RESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;
        private readonly IPropertyMappingServices propertyMappingServices;

        public CourseLibraryRepository(CourseLibraryContext context,IPropertyMappingServices propertyMappingServices )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.propertyMappingServices = propertyMappingServices;
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course); 
        }         

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }
  
        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToList();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }
        
        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }


       
        public PageList<Author> GetAuthors(AuthorsResourceParametres para)
        {
            

            var collection = _context.Authors as IQueryable<Author>;

            if (!string.IsNullOrWhiteSpace(para.mainCategory))
            {
                var mainCategory =para. mainCategory.Trim();
                collection = collection.Where(a => a.MainCategory
                                                    == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(para.querySearch))
            {
               var searchQuery = para.querySearch.Trim();
               collection=collection.Where(a => a.MainCategory
                                                  .Contains(para.querySearch)
                     || a.FirstName.Contains(para. querySearch)
                     || a.LastName.Contains(para. querySearch)
                    );
            }

            if (!string.IsNullOrWhiteSpace(para.orderBy))
            {
                var authorsPropertyMappingDectionary = propertyMappingServices.GetPropertyMapping<AuthorDto, Author>();

             collection= collection.ApplySort(para.orderBy, authorsPropertyMappingDectionary);
            }
            return PageList<Author>.Create(collection,
               para.PageNumber,
                para.PageSize);
        }
        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList<Author>();
        }
         
        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               // dispose resources when needed
            }
        }
    }
}
