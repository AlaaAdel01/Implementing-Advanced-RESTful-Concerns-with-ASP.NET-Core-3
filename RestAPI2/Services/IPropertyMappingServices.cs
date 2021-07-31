using System.Collections.Generic;

namespace RestAPI2.Services
{
    public interface IPropertyMappingServices
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        public bool ValidMappingService<TSource, TDestination>(string field);
    }
}