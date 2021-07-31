using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Helper
{
    public class PageList<T>:List<T>
    {
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public int PageSize { get; set; }
        public int totalCount { get; set; }
        public bool HasPrevios=>(currentPage>1);
        public bool HasNext => (currentPage < totalPages);

        public PageList(List<T> items,int count ,int pageNumber ,int pageSize)
        {
            totalCount = count;
            PageSize = pageSize;
            currentPage = pageNumber;
            totalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
                
        }

        public static PageList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(items, count, pageNumber, pageSize);

        }
      }
}
