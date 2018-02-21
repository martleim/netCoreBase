using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GenericAirways.Helpers
{
    public class DeepCopy
    {

        public static object CloneInternal(object entity, int depth)
        {
            if (entity == null)
                return null;

            var cloned = Activator.CreateInstance(entity.GetType());

            Func<Type, Type> getStructureType = input =>
            {
                var type = input.GetGenericArguments().Single();
                if (typeof(IList<>).MakeGenericType(type).IsAssignableFrom(input))
                {
                    return typeof(List<>).MakeGenericType(type);
                }
                else if (typeof(ICollection<>).MakeGenericType(type).IsAssignableFrom(input))
                {
                    return typeof(Collection<>).MakeGenericType(type);
                }
                return typeof(List<>).MakeGenericType(type);
            };

            foreach (var property in cloned.GetType().GetProperties())
            {
                Type propertyType = property.PropertyType;
                if (propertyType.Namespace == "System" && property.CanWrite)
                {
                    property.SetValue(cloned, property.GetValue(entity));
                }
                else if (depth > 0 && property.CanWrite && typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    if (typeof(IList).IsAssignableFrom(propertyType))
                    {
                        var collection = (IList)Activator.CreateInstance(propertyType);
                        var value = property.GetValue(entity);
                        foreach (var element in value as IEnumerable)
                        {
                            collection.Add(CloneInternal(element, depth - 1));
                        }
                        property.SetValue(cloned, collection);
                    }
                    else if (propertyType.IsGenericType)
                    {
                        var type = propertyType.GetGenericArguments().Single();
                        if (typeof(IList<>).MakeGenericType(type).IsAssignableFrom(propertyType) ||
                            typeof(ISet<>).MakeGenericType(type).IsAssignableFrom(propertyType) ||
                            typeof(ICollection<>).MakeGenericType(type).IsAssignableFrom(propertyType))
                        {
                            var collection = Activator.CreateInstance(getStructureType(propertyType));
                            var addMethod = collection.GetType().GetMethod("Add");
                            var value = property.GetValue(entity);
                            foreach (var element in value as IEnumerable)
                            {
                                addMethod.Invoke(collection, new[] { CloneInternal(element, depth - 1) });
                            }
                            property.SetValue(cloned, collection);
                        }
                    }
                }
                else if (depth > 0 && property.CanWrite && typeof(object).IsAssignableFrom(propertyType))
                {
                    property.SetValue(cloned, CloneInternal(property.GetValue(entity), depth--));
                }
            }
            return cloned;
        }

    }

}