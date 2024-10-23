﻿using System.Text.Json;

namespace SLA.Domain.Application.Extensions
{
    public static class ListExtension
    {
        public static bool HasValue<T>(this List<T> list)
        {
            bool result = false;
            if (list != null)
            {
                result = list.Count() > 0 ? true : false;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static string ToJson<T>(this List<T> list)
        {
            string json = string.Empty;
            try
            {
                json = JsonSerializer.Serialize(list);
            }
            catch
            {
                json = "{}";
            }
            return json;
        }
    }
}
