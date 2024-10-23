using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Data.Filters;
using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Instruction;
using SLA.Domain.Infra.Interfaces;
using System.Text.Json;

namespace SLA.Domain.Infra.Extensions.NonRelational
{
    public static class DataExtensionMongo
    {
        #region Methods
        public static string TypeValueToString(TypeCode typeCode, dynamic Value)
        {
            string result = "";

            switch (typeCode)
            {
                case TypeCode.Int16:
                    result = Convert.ToInt16(Value).ToString();
                    break;
                case TypeCode.Int32:
                    result = Convert.ToInt32(Value).ToString();
                    break;
                case TypeCode.Int64:
                    result = Convert.ToInt64(Value).ToString();
                    break;
                case TypeCode.Double:
                    result = Value.ToString().Replace(".", "").Replace(",", ".");
                    break;
                case TypeCode.String:
                    result = $"'{Value.ToString()}'";
                    break;
                case TypeCode.DateTime:
                    var datetime = Convert.ToDateTime(Value);

                    // Verifica tipo de campo
                    if (datetime.TimeOfDay != DateTime.Parse("00:00:00").TimeOfDay)
                    {
                        // DateTime
                        if (datetime.Date != DateTime.Parse("0001-01-01").Date)
                        {
                            result = $"'{datetime.ToString("yyyy-MM-dd HH:mm:ss")}'";
                        }
                        else
                        {
                            // Time
                            result = $"'{datetime.ToString("HH:mm:ss")}'";
                        }
                    }
                    else
                    {
                        // Date
                        result = $"'{datetime.ToString("yyyy-MM-dd")}'";
                    }
                    break;
                case TypeCode.Boolean:
                    result = Value.ToString();
                    break;
                default:
                    result = $"'{Value.ToString()}'";
                    break;
            }
            return result;
        }

        public static string FilterByKey(this DataModel model, dynamic Value)
        {
            string result = string.Empty;

            // Obtem os campos da classe
            FieldsModel key = new FieldsModel();
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                // Captura os dados da chave primaria
                var Arg = prop.CustomAttributes.Where(a => a.AttributeType.Name == "Core").ToList();
                if (Arg.Count > 0)
                {
                    var ArgF = Arg[0].NamedArguments.ToList();
                    var ArgK = ArgF.Where(a => a.MemberName == "PrimaryKey").ToList();
                    if (ArgK.Count == 1)
                    {
                        key.Name = prop.Name;
                        Type property = prop.PropertyType;
                        key.Type = System.Type.GetTypeCode(property);
                        key.Value = prop.GetValue(model, null).ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(key.Name))
            {
                var val = Value;
                string value;

                // Verifica se a propriedade possui valor, ignorar nulos
                if (val != null)
                {
                    switch (key.Type)
                    {
                        case TypeCode.Int16:
                            value = val.ToString();
                            break;
                        case TypeCode.Int32:
                            value = val.ToString();
                            break;
                        case TypeCode.Int64:
                            value = val.ToString();
                            break;
                        case TypeCode.Double:
                            value = val.ToString();
                            break;
                        case TypeCode.String:
                            value = $"'{val.ToString()}'";
                            break;
                        case TypeCode.DateTime:
                            var datetime = Convert.ToDateTime(val);

                            // Verifica tipo de campo
                            if (datetime.TimeOfDay != DateTime.Parse("00:00:00").TimeOfDay)
                            {
                                // DateTime
                                if (datetime.Date != DateTime.Parse("0001-01-01").Date)
                                {
                                    value = $"'{datetime.ToString("yyyy-MM-dd HH:mm:ss")}'";
                                }
                                else
                                {
                                    // Time
                                    value = $"'{datetime.ToString("HH:mm:ss")}'";
                                }
                            }
                            else
                            {
                                // Date
                                value = $"'{datetime.ToString("yyyy-MM-dd")}'";
                            }
                            break;
                        case TypeCode.Boolean:
                            value = val.ToString();
                            break;
                        default:
                            value = $"'{val.ToString()}'";
                            break;
                    }
                    // Adiciona o registro a lista
                    result = $" WHERE {key.Name} {FilterBy(TypeSQLFilterEnum.Equal, key.Type, value)}";
                }
            }

            return result;
        }

        public static string FilterBy(TypeSQLFilterEnum TypeFilter, TypeCode Type, string Value)
        {
            string result;

            switch (TypeFilter)
            {
                case TypeSQLFilterEnum.Equal:          // ==
                    {
                        if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.Like:         // LIKE %V%
                    {
                        if (Type == TypeCode.String) result = $"ILIKE '%{Value}%'";
                        else if (Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.Starts:         // LIKE V%
                    {
                        if (Type == TypeCode.String) result = $"ILIKE '{Value}%'";
                        else if (Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.Ends:        // LIKE %V
                    {
                        if (Type == TypeCode.String) result = $"ILIKE '%{Value}'";
                        else if (Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.Bigger:          // >
                    {
                        if (Type == TypeCode.Int16 || Type == TypeCode.Int32 || Type == TypeCode.Int64 || Type == TypeCode.Double) result = $"> {Value}";
                        else if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.BiggerEqual:     // >=
                    {
                        if (Type == TypeCode.Int16 || Type == TypeCode.Int32 || Type == TypeCode.Int64 || Type == TypeCode.Double) result = $">= {Value}";
                        else if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.Less:          // <
                    {
                        if (Type == TypeCode.Int16 || Type == TypeCode.Int32 || Type == TypeCode.Int64 || Type == TypeCode.Double) result = $"< {Value}";
                        else if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                case TypeSQLFilterEnum.LessEqual:     // <=
                    {
                        if (Type == TypeCode.Int16 || Type == TypeCode.Int32 || Type == TypeCode.Int64 || Type == TypeCode.Double) result = $"<= {Value}";
                        else if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;

                default:
                    {
                        if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"= '{Value}'";
                        else result = $"= {Value}";
                    }
                    break;
            }

            return result;
        }

        public static string FilterBy(TypeSQLFilterEnum TypeFilter, TypeCode Type, List<string> Value)
        {
            string result;

            switch (TypeFilter)
            {
                case TypeSQLFilterEnum.NotIn:         // NOT IN ()
                    {
                        if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"NOT IN ('{string.Join("', '", Value)}')";
                        else result = $"IN ({string.Join(", ", Value)})";
                    }
                    break;
                case TypeSQLFilterEnum.In:             // IN ()
                    {
                        if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"IN ('{string.Join("', '", Value)}')";
                        else result = $"IN ({string.Join(", ", Value)})";
                    }
                    break;
                case TypeSQLFilterEnum.Between:          // BETWEEN
                    {
                        if (Value.Count == 2)
                        {
                            if (Type == TypeCode.DateTime)
                            {
                                DateTime D0 = Convert.ToDateTime(Value[0]);
                                DateTime D1 = Convert.ToDateTime(Value[1]);

                                result = D0 < D1 ? $"BETWEEN '{Value[0]}' AND '{Value[1]}')" : $"BETWEEN '{Value[1]}' AND '{Value[0]}')";
                            }
                            else if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"IN ('{string.Join("', '", Value)}')";
                            else result = $"IN ({string.Join(", ", Value)})";
                        }
                        else
                        {
                            if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"IN ('{string.Join("', '", Value)}')";
                            else result = $"IN ({string.Join(", ", Value)})";
                        }
                    }
                    break;

                default:
                    {
                        if (Type == TypeCode.String || Type == TypeCode.DateTime) result = $"IN ('{string.Join("', '", Value)}')";
                        else result = $"IN ({string.Join(", ", Value)})";
                    }
                    break;
            }

            return result;
        }

        public static string GetFieldFilters(this IDataFilterCollection Filter, string Sintaxe)
        {
            string conditional = "AND";
            List<FieldsFilter> fieldsFilter = new List<FieldsFilter>();
            var config = Filter.Property.Where(r => r.Type == TypeDataFilterEnum.Conditional || r.Type == TypeDataFilterEnum.Filter);
            foreach (var c in config)
            {
                switch (c.Type)
                {
                    case TypeDataFilterEnum.Conditional:
                        {
                            conditional = (c as DataFilterConditional).AndConditional ? "AND" : "OR";
                        }
                        break;
                    case TypeDataFilterEnum.Filter:
                        {
                            fieldsFilter = (c as DataFilterFields).Fields;
                        }
                        break;
                }
            }

            if (fieldsFilter.Count > 0)
            {
                string filter = "";
                List<string> filters = new List<string>();
                List<string> filtered = new List<string>();
                foreach (var fields in fieldsFilter)
                {
                    // Verifica se o campo ja foi filtrado, caso de duplicados
                    if (!filtered.Exists(x => x == fields.Name))
                    {
                        // Verifica se o filtro possui campos duplicados
                        var count = fieldsFilter.Where(x => x.Name == fields.Name).Count();
                        if (count > 1)
                        {
                            // Adiciona o campo a lista de duplicados
                            filtered.Add(fields.Name);

                            List<string> Values = new List<string>();
                            foreach (var dup in fieldsFilter.Where(x => x.Name == fields.Name).ToList())
                            {
                                Values.Add(dup.Value);
                            }
                            filters.Add($"{fields.Name} {FilterBy(fields.Method, fields.Type, Values)}");
                        }
                        else
                        {
                            filters.Add($"{fields.Name} {FilterBy(fields.Method, fields.Type, fields.Value)}");
                        }
                    }
                }

                filter = string.Join($" {conditional} ", filters);

                Sintaxe += " WHERE " + filter;
            }

            return Sintaxe;
        }

        public static string GetLimitSettings(this IDataFilterCollection Filter, string Sintaxe)
        {
            int limit = -1;
            int offset = -1;

            var config = Filter.Property.Where(r => r.Type == TypeDataFilterEnum.Limit || r.Type == TypeDataFilterEnum.Page);
            foreach (var c in config)
            {
                switch (c.Type)
                {
                    case TypeDataFilterEnum.Limit:
                        {
                            limit = (c as DataFilterLimit).Limit;
                        }
                        break;
                    case TypeDataFilterEnum.Page:
                        {
                            offset = (c as DataFilterPage).Page;
                        }
                        break;
                }
            }

            if (limit > 0)
            {
                if (offset > 0)
                {
                    Sintaxe += $" LIMIT {limit} OFFSET {offset}";
                }
                else
                {
                    Sintaxe += $" LIMIT {limit}";
                }
            }
            return Sintaxe;
        }

        public static string GetPrimaryKey(this IDataFilterCollection Filter, DataModel model, string Sintaxe)
        {
            dynamic? key = null;

            var config = Filter.Property.Where(r => r.Type == TypeDataFilterEnum.Key);
            foreach (var c in config)
            {
                switch (c.Type)
                {
                    case TypeDataFilterEnum.Key:
                        {
                            key = (c as DataFilterKey).Key;
                        }
                        break;
                }
            }

            if (key != null)
            {
                Sintaxe += FilterByKey(model, key);
            }
            return Sintaxe;
        }
        #endregion


        public static string MongoFind(this DataModel model)
        {
            // Iniciar o processo de montagem do SQL
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Collection não definida.";

            List<string> Fields = model.GetFields();

            // Monta o Regex
            MongoInstruction instruction = new MongoInstruction() 
            { 
                Collection = model.Table,
                Regex = "{}"
            };                        

            return JsonSerializer.Serialize(instruction);
        }

        public static string MongoFind(this DataModel model, IDataFilterCollection filter)
        {
            // Iniciar o processo de montagem do SQL
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Collection não definida.";

            List<string> Fields = model.GetFields();

            // Verifica a collection de filtros
            if (filter.Property.Exists(r => r.Type == TypeDataFilterEnum.Key))
            {
                Sintaxe = filter.GetPrimaryKey(model, Sintaxe);
            }
            else
            {
                Sintaxe = filter.GetFieldFilters(Sintaxe);
                Sintaxe = filter.GetLimitSettings(Sintaxe);
            }
            //var filter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);            

            // Monta o Regex
            MongoInstruction instruction = new MongoInstruction()
            {
                Collection = model.Table,
                Regex = "{}"
            };

            return JsonSerializer.Serialize(instruction);
        }

        public static string MongoCount(this DataModel model) 
        {
            // Iniciar o processo de montagem do SQL
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Collection não definida.";

            List<string> Fields = model.GetFields();

            // Monta o Regex
            MongoInstruction instruction = new MongoInstruction()
            {
                Collection = model.Table,
                Regex = "{}"
            };

            return JsonSerializer.Serialize(instruction);
        }
    }
}
