using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Models
{
    public class LinkDto
    {
        public string Rel { get; set; }

        public string  Href { get; set; }
        public string Method { get; set; }
        public LinkDto( string href,string rel, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;

        }
    }
}
