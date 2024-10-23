using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Data.Filters;
using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Extensions;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Extensions.Relational.Oracle
{
    public static class DataExtensionOracle
    {
        #region
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
                        key.Type = Type.GetTypeCode(property);
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

        private static string MakeFilterNumeric(TypeSQLFilterEnum TypeFilter, string Value)
        {
            string result = string.Empty;
            Console.WriteLine($"DATA:[{TypeFilter}][{Value}]");
            switch (TypeFilter)
            {
                case TypeSQLFilterEnum.Equal:          // ==                    
                    result = $"= '{Value}'";
                    break;
                case TypeSQLFilterEnum.Bigger:          // >                    
                    result = $"> {Value}";
                    break;
                case TypeSQLFilterEnum.BiggerEqual:     // >=                    
                    result = $">= {Value}";
                    break;
                case TypeSQLFilterEnum.Less:          // <                    
                    result = $"< {Value}";
                    break;
                case TypeSQLFilterEnum.LessEqual:     // <=                    
                    result = $"<= {Value}";
                    break;
                default:
                    result = $"= {Value}";
                    break;
            }
            return result;
        }

        private static string MakeFilterDateTime(TypeSQLFilterEnum TypeFilter, string Value)
        {
            string result = string.Empty;
            var data = DateTime.Parse(Value);
            var dataValue = data.TimeOfDay.TotalHours == 0 ? data.ToString("yyyy-MM-dd") : data.ToString("yyyy-MM-dd HH:mm:ss");

            switch (TypeFilter)
            {
                case TypeSQLFilterEnum.Equal:          // ==                    
                    result = $"= '{dataValue}'";
                    break;
                case TypeSQLFilterEnum.Bigger:          // >                    
                    result = $"> '{dataValue}'";
                    break;
                case TypeSQLFilterEnum.BiggerEqual:     // >=                    
                    result = $">= '{dataValue}'";
                    break;
                case TypeSQLFilterEnum.Less:          // <                    
                    result = $"< '{dataValue}'";
                    break;
                case TypeSQLFilterEnum.LessEqual:     // <=                    
                    result = $"<= '{dataValue}'";
                    break;
                default:
                    result = $"= '{dataValue}'";
                    break;
            }
            return result;
        }

        public static string FilterBy(TypeSQLFilterEnum TypeFilter, TypeCode Type, string Value)
        {
            string result;
            Console.WriteLine($"DATA:[{TypeFilter}][{Type}][{Value}]");
            // Verifica o tipo de dados
            switch (Type)
            {
                case TypeCode.Boolean:
                    result = $"= {Value.ToLower()}";
                    break;
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Double:
                case TypeCode.Single:
                    result = MakeFilterNumeric(TypeFilter, Value);
                    break;
                // Verifica o tipo de filtro
                case TypeCode.DateTime:
                    result = MakeFilterDateTime(TypeFilter, Value);
                    break;
                // Retorna igual
                case TypeCode.Char:
                    result = $"= '{Value}'";
                    break;
                // Retorna igual
                case TypeCode.String:
                    result = $"= '{Value}'";
                    break;
                // Retorna Null
                case TypeCode.Empty:
                    result = $"= NULL";
                    break;
                // Retorna igual
                default:
                    result = $"= '{Value}'";
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

                                if (D0.TimeOfDay.TotalHours == 0 || D1.TimeOfDay.TotalHours == 0)
                                {
                                    result = D0 < D1 ? $"BETWEEN '{D0.ToString("yyyy-MM-dd")}' AND '{D1.ToString("yyyy-MM-dd")}'" : $"BETWEEN '{D1.ToString("yyyy-MM-dd")}' AND '{D0.ToString("yyyy-MM-dd")}'";
                                }
                                else
                                {
                                    result = D0 < D1 ? $"BETWEEN '{Value[0]}' AND '{Value[1]}'" : $"BETWEEN '{Value[1]}' AND '{Value[0]}'";
                                }
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

        #region Methods
        public static string OracleSelect(this DataModel model)
        {
            // Iniciar o processo de montagem do SQL
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Tabela não definida.";

            List<string> Fields = model.GetFields();

            // Monta o SQL
            Sintaxe = $"SELECT {string.Join(", ", Fields)} FROM {model.Table}";

            return Sintaxe;
        }

        public static string OracleSelect(this DataModel model, IDataFilterCollection filter)
        {
            // Iniciar o processo de montagem do SQL
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Tabela não definida.";

            List<string> Fields = model.GetFields();

            // Monta o SQL
            Sintaxe = $"SELECT {string.Join(", ", Fields)} FROM {model.Table}";

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

            return Sintaxe;
        }

        public static string OracleCount(this DataModel model)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            if (string.IsNullOrEmpty(model.Table)) return "Tabela não definida.";

            // Monta o SQL
            Sintaxe = $"SELECT COUNT(*) FROM {model.Table}";

            return Sintaxe;
        }

        public static string OracleInsert(DataModel model, bool ReturningKey)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            // Verifica se o table está preenchida
            if (string.IsNullOrEmpty(model.Table)) return "Table não definida.";

            List<FieldsModel> Fields = new List<FieldsModel>();

            // Obtem os campos da classe
            FieldsModel key = new FieldsModel();
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                // Verifica se são campos auto-incremento
                bool Restric = false;
                var Arg = prop.CustomAttributes.Where(a => a.AttributeType.Name == "Core").ToList();
                if (Arg.Count > 0)
                {
                    // Restrito
                    var ArgF = Arg[0].NamedArguments.ToList();
                    var ArgV = ArgF.Where(a => a.MemberName == "Restrict" || a.MemberName == "Ignore").ToList();
                    if (ArgV.Count > 0) Restric = true;

                    // Captura os dados da chave primaria
                    var ArgK = ArgF.Where(a => a.MemberName == "PrimaryKey").ToList();
                    if (ArgK.Count == 1)
                    {
                        key.Name = prop.Name;
                        Type propertyType = prop.PropertyType;
                        key.Type = Type.GetTypeCode(propertyType);
                        key.Value = prop.GetValue(model, null).ToString();
                    }
                }

                if (!Restric)
                {
                    Type propertyType = prop.PropertyType;
                    TypeCode typeCode = Type.GetTypeCode(propertyType);
                    string value = "";

                    var val = prop.GetValue(model, null);

                    // Verifica se a propriedade possui valor, ignorar nulos
                    if (val != null)
                    {
                        value = TypeValueToString(typeCode, val);

                        // Adiciona o registro a lista
                        Fields.Add(new FieldsModel() { Name = prop.Name, Type = typeCode, Value = value });
                    }
                }
            }

            // Monta o esquema dos campos
            string fields;
            string values;
            List<string> ListFields = new List<string>();
            List<string> ListValues = new List<string>();
            Fields.ForEach(f => ListFields.Add(f.Name));
            Fields.ForEach(v => ListValues.Add(v.Value));
            fields = string.Join(", ", ListFields);
            values = string.Join(", ", ListValues);

            // Monta o SQL
            Sintaxe = $"INSERT INTO {model.Table} ({fields}) VALUES ({values})";

            if (ReturningKey)
            {
                Sintaxe += $" RETURNING {key.Name}";
            }

            return Sintaxe;
        }

        public static string OracleUpdate(DataModel model, IDataFilterCollection? Filter = null)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            // Verifica se o table está preenchida
            if (model.Table == "") return "Table não definida.";

            List<FieldsModel> Fields = new List<FieldsModel>();

            // Obtem os campos da classe
            FieldsModel key = new FieldsModel();
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                // Verifica se são campos auto-incremento
                bool Restric = false;
                var Arg = prop.CustomAttributes.Where(a => a.AttributeType.Name == "Core").ToList();
                if (Arg.Count > 0)
                {
                    var ArgF = Arg[0].NamedArguments.ToList();
                    var ArgR = ArgF.Where(a => a.MemberName == "Restrict" || a.MemberName == "Ignore").ToList();
                    if (ArgR.Count > 0) Restric = true;

                    // Captura os dados da chave primaria
                    var ArgK = ArgF.Where(a => a.MemberName == "PrimaryKey").ToList();
                    if (ArgK.Count == 1)
                    {
                        key.Name = prop.Name;
                        Type propertyType = prop.PropertyType;
                        key.Type = Type.GetTypeCode(propertyType);
                        key.Value = prop.GetValue(model, null).ToString();
                    }
                }

                if (!Restric)
                {
                    Type propertyType = prop.PropertyType;
                    TypeCode typeCode = Type.GetTypeCode(propertyType);
                    string? value = "";

                    var val = prop.GetValue(model, null);

                    // Verifica se a propriedade possui valor, ignorar nulos
                    if (val != null)
                    {
                        switch (typeCode)
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

                        Fields.Add(new FieldsModel() { Name = prop.Name, Type = typeCode, Value = value });
                    }
                }
            }

            string fields;
            List<string> ListFields = new List<string>();

            foreach (var Field in Fields)
            {
                ListFields.Add($"{Field.Name} = {Field.Value}");
            }
            fields = string.Join(", ", ListFields);

            // Monta o SQL
            Sintaxe = $"UPDATE {model.Table} SET {fields}";

            // Verifica se possui filtros
            if (Filter != null)
            {
                Sintaxe += Filter.GetFieldFilters(Sintaxe);
            }
            else
            {
                string keyFilter = $"{key.Name} {FilterBy(TypeSQLFilterEnum.Equal, key.Type, key.Value)}";
                Sintaxe += " WHERE " + keyFilter;
            }

            return Sintaxe;
        }

        public static string OracleDelete(DataModel model, IDataFilterCollection? Filter = null)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            // Verifica se o table está preenchida
            if (string.IsNullOrEmpty(model.Table)) return "Table não definida.";

            FieldsModel key = new FieldsModel();
            FieldsModel exclude = new FieldsModel();

            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                // Verifica se são campos auto-incremento
                var Arg = prop.CustomAttributes.Where(a => a.AttributeType.Name == "Core").ToList();
                if (Arg.Count > 0)
                {
                    // Captura os dados da chave primaria
                    var ArgF = Arg[0].NamedArguments.ToList();
                    var ArgK = ArgF.Where(a => a.MemberName == "PrimaryKey").ToList();
                    if (ArgK.Count == 1)
                    {
                        key.Name = prop.Name;
                        Type propertyType = prop.PropertyType;
                        key.Type = Type.GetTypeCode(propertyType);
                        key.Value = prop.GetValue(model, null).ToString();
                    };

                    // Verifica se possui atributo de exclusão
                    var ArgE = ArgF.Where(a => a.MemberName == "Exclude").ToList();
                    if (ArgE.Count == 1)
                    {
                        exclude.Name = prop.Name;
                        Type propertyType = prop.PropertyType;
                        exclude.Type = Type.GetTypeCode(propertyType);
                        exclude.Value = prop.GetValue(model, null).ToString();
                    }
                }
            }

            if (exclude == default)
            {
                Sintaxe = $"UPDATE {model.Table} SET {exclude.Name} = true";
            }
            else
            {
                Sintaxe = $"DELETE FROM {model.Table}";
            }

            // Verifica se possui filtros
            if (Filter != null)
            {
                Sintaxe += Filter.GetFieldFilters(Sintaxe);
            }
            else
            {
                string keyFilter = $"{key.Name} {FilterBy(TypeSQLFilterEnum.Equal, key.Type, key.Value)}";
                Sintaxe += " WHERE " + keyFilter;
            }

            return Sintaxe;
        }
        #endregion
    }
}
