using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using RestAPI2.Services;
namespace RestAPI2.Helper
{
    public static class IQuerableExtensions
    {
        public static IQueryable<T> ApplySort<T> (this IQueryable<T>source,string orderBy,
            Dictionary<string,PropertyMappingValue> mappingDictionary)
        {
            if (source==null)
            {
                throw new ArgumentException(nameof(source));
            }
            if (mappingDictionary == null)
            {
                throw new ArgumentException(nameof(mappingDictionary));

            }

            if(string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }
            var orderByString = string.Empty;
            var orderByAfterSplit = orderBy.Split(',');
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderbyClause = orderByClause.Trim();

                var orderDesc = trimmedOrderbyClause.EndsWith(" desc");

                var indexOfFirstSpace = trimmedOrderbyClause.IndexOf(" ");

                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderbyClause : trimmedOrderbyClause.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"key maping{propertyName} is invalid");

                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if(propertyMappingValue==null)
                {
                    throw new ArgumentException($"key maping is invalid");

                }

                foreach (var destinationProp in propertyMappingValue.DestinationProperties)
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDesc = !orderDesc;
                    }

              
                     orderByString   = orderByString +
                       (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ",")
                   + destinationProp + (orderDesc ? " desc" : " Asc");
                  }

            }
            return source.OrderBy(orderByString);
        }

    }
}
