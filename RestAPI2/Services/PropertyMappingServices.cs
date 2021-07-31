using CourseLibrary.API.Entities;
using RESTfulAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI2.Services
{
    public class PropertyMappingServices : IPropertyMappingServices
    {
        private Dictionary<string, PropertyMappingValue> authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>(){"Id"}) },
                {"MainCategory",new PropertyMappingValue(new List<string>(){ "MainCategory"}) },
                {"Age",new PropertyMappingValue(new List<string>(){"DateOfBirth" },true) },
                {"Name",new PropertyMappingValue(new List<string>(){"FirstName","LastName"}) }
            };

        private IList<IPropertyMapping> proppertyMapping = new List<IPropertyMapping>();

        public PropertyMappingServices()
        {
            proppertyMapping.Add(new PropertyMapping<AuthorDto, Author>(authorPropertyMapping));
        }

        public bool ValidMappingService<TSource,TDestination>(string field)
        {
            var propertyMapping = GetPropertyMapping<AuthorDto, Author>();
            if (string.IsNullOrWhiteSpace(field))
            {
                return true;
            }

            var fieldAfterSplit = field.Split(',');

            foreach (var fields in fieldAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
   
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = proppertyMapping.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }
            throw new Exception($"cann't find the maaping value");
        }

    }
}
