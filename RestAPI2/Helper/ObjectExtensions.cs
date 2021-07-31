using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RestAPI2.Helper
{
    public static class ObjectExtensions
    {

        public static ExpandoObject ShapeDataObiect<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();



            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                return dataShapedObject;

            }

          
                var fieldAfterSplit = fields.Split(",");
                foreach (var field in fieldAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfos = typeof(TSource).GetProperty(
                        propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfos == null)
                    {
                        throw new Exception($"propperty{propertyName}wasn't found on " +
                            $"{typeof(TSource)}");
                    }


                
                    var propertyValue = propertyInfos.GetValue(source);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfos.Name, propertyValue);
                


            }



            return dataShapedObject;
        }
    }
}
