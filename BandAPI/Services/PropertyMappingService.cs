using BandAPI.Entities;
using BandAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMapingValue> _bandPropertyMapping =
            new Dictionary<string, PropertyMapingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMapingValue(new List<string>() {"Id"}) },
                {"Name", new PropertyMapingValue(new List<string>() {"Name"}) },
                {"MainGenre", new PropertyMapingValue(new List<string>() {"MainGenre"}) },
                {"FoundedYearsAgo", new PropertyMapingValue(new List<string>() {"Founded"}, true)}
            };

        private IList<IPropertyMappingMarker> _propertyMappings =
                                                new List<IPropertyMappingMarker>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<BandDto, Band>(_bandPropertyMapping));
        }

        public Dictionary<string, PropertyMapingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings
                                    .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
                return matchingMapping.First().MappingDictionary;

            throw new Exception("No mapping was found");
        }

        public bool ValidMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(",");

            foreach(var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfSpace == -1 ? trimmedField :
                                                        trimmedField.Remove(indexOfSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }

            return true;
        }
    }
}
