using SLA.Domain.Application.Enumerators;
using SLA.Domain.Infra.Data;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace SLA.Domain.Infra.Connection
{
    public static class EntityDataManipulate
    {
        #region Manipulate
        public static T GetItemDefault<T>(DataRow dr)
        {
            T obj = Activator.CreateInstance<T>();
            List<PropertyInfo> props = typeof(T).GetProperties().ToList();

            foreach (DataColumn column in dr.Table.Columns)
            {
                var prop = props.Where(r => r.Name == column.ColumnName).FirstOrDefault();
                if (prop != null)
                {
                    Type field = prop.PropertyType;
                    var value = dr[column.ColumnName];
                    try
                    {
                        if (value.GetType().Name == "DBNull")
                        {
                            value = default;
                        }
                        else
                        {
                            if (prop.PropertyType.Name.Contains("Nullable"))
                            {
                                prop.SetValue(obj, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)), null);
                            }
                            else
                            {
                                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType), null);
                            }
                        }
                    }
                    catch
                    {
                        prop.SetValue(obj, value, null);
                    }                    
                }
            }

            return (T)obj;
        }

        public static T ConvertDataRow<T>(DataTable dt)
        {
            DataRow row = dt.Rows[0];
            return GetItemDefault<T>(row);
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItemDefault<T>(row);
                data.Add(item);
            }
            return data;
        }
        
        public static T GetItemDocument<T>(DataRow dr)
        {
            T obj = Activator.CreateInstance<T>();
            List<PropertyInfo> props = typeof(T).GetProperties().ToList();

            foreach (DataColumn column in dr.Table.Columns)
            {
                var prop = props.Where(r => r.Name == column.ColumnName).FirstOrDefault();
                if (prop != null)
                {
                    Type field = prop.PropertyType;
                    var value = dr[column.ColumnName];
                    try
                    {
                        if (value.GetType().Name == "DBNull")
                        {
                            value = default;
                        }
                        else
                        {
                            if (field.FullName.Contains("System"))
                            {
                                if (prop.PropertyType.Name.Contains("Nullable"))
                                {
                                    prop.SetValue(obj, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)), null);
                                }
                                else
                                {
                                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType), null);
                                }
                            }
                            else 
                            {                                
                                var subObj = JsonSerializer.Deserialize(value.ToString(), field);
                                prop.SetValue(obj, subObj, null);
                            }
                        }
                    }
                    catch
                    {
                        prop.SetValue(obj, value, null);
                    }                    
                }
            }

            return (T)obj;
        }

        public static List<T> ConvertDocument<T>(DataTable dt, TypeModelEnum TypeData) where T : DataModel
        {           
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItemDocument<T>(row);
                item.SetType(TypeData);
                data.Add(item);
            }
            return data;
        }
        #endregion
    }
}