﻿using System.Reflection;

namespace SLA.Domain.Application.Extensions
{
    public static class PropertyExtensions
    {
        public static TValue? GetAttributValue<TAttribute, TValue>(this PropertyInfo prop, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            var att = prop.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

            if (att != null)
            {
                return value(att);
            }

            return default;
        }


        private static dynamic? GetPropertyValues<T>(this T obj)
        {
            dynamic? result = default;

            Type t = obj.GetType();

            PropertyInfo[] props = t.GetProperties();

            foreach (var prop in props)
            {
                if (prop.GetIndexParameters().Length == 0)
                {
                    result = prop.GetValue(obj);
                }
                else
                {
                    result = default(dynamic);
                }
            }

            return result;
        }
    }
}
