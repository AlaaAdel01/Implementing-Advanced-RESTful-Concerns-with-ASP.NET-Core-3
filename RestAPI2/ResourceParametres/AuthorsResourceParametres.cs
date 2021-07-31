using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.ResourceParametres
{
    public class AuthorsResourceParametres
    {
        const int maxPageSize = 20;
        public string querySearch { get; set; }
        public string mainCategory { get; set; }
        private int pageSize = 10;
        public int PageNumber { get; set; } = 1;

        public int PageSize { 
            get=>pageSize;
            set=>pageSize=(value>maxPageSize)?maxPageSize:value; }
        public string orderBy { get; set; } = "Name";

        public string Fields { get; set; }
    }
}
