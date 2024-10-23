using SLA.Domain.Infra.Enumerators;

namespace SLA.Domain.Infra.Data
{
    public class FieldsFilter
    {
        public string Name { get; set; }
        public TypeCode Type { get; set; }
        public TypeSQLFilterEnum Method { get; set; }
        public string Value { get; set; }

        public static List<FieldsFilter> FKeyFilter(string Name, TypeCode Type, TypeSQLFilterEnum Method, string Value)
        {
            List<FieldsFilter> fkey = new List<FieldsFilter>();
            fkey.Add(new FieldsFilter()
            {
                Name = Name,
                Type = Type,
                Method = Method,
                Value = Value
            });
            return fkey;
        }
    }
}
