using SLA.Domain.Application.Enumerators;
using SLA.Domain.Infra.Attributes;
using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Extensions.NonRelational;
using SLA.Domain.Infra.Extensions.Relational.Oracle;
using SLA.Domain.Infra.Extensions.Relational.PostgreSql;
using SLA.Domain.Infra.Interfaces;

namespace SLA.Domain.Infra.Extensions
{
    public static class DataExtension
    {
        #region Metodos internos
        public static List<string> GetFields(this DataModel model)
        {
            var fields = new List<string>();

            // Obtem os campos da classe excluindo os registros a serem ignorados
            var ignore = model.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Core)));
            List<string> list = new List<string>();

            foreach (var prop in ignore)
            {
                var Arg = prop.CustomAttributes.Where(a => a.AttributeType.Name == "Core").ToList();
                if (Arg.Count > 0)
                {
                    var attribute = new Core(Arg[0].NamedArguments.ToList());

                    if (attribute.Ignore) list.Add(prop.Name);
                }
            }

            // Obtem os campos da classe
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (!list.Exists(r => r == prop.Name))
                {
                    fields.Add(prop.Name);
                }
            }

            return fields;
        }

        public static string GetPrimaryKeyProperty(this DataModel model)
        {
            var type = model.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(Core), false);
                if (attributes.Length > 0)
                {
                    var coreAttribute = (Core)attributes[0];
                    if (coreAttribute.PrimaryKey)
                    {
                        return property.Name;
                    }
                }
            }

            return string.Empty;
        }
        #endregion

        #region Metodos de Sintaxe
        public static string Select(this DataModel model) 
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type) 
            {
                case TypeModelEnum.Postgres:             
                    {
                        Sintaxe = model.PostgreSQLSelect();                        
                    }
                    break;
                case TypeModelEnum.Oracle: 
                    {                        
                        Sintaxe = model.OracleSelect();
                    }
                    break;
                case TypeModelEnum.MongoDB:
                    {
                        Sintaxe = model.MongoFind();
                    }
                    break;
            }            
            return Sintaxe;
        }

        public static string Select(this DataModel model, IDataFilterCollection filter)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type) 
            {
                case TypeModelEnum.Postgres: 
                    {
                        Sintaxe = model.PostgreSQLSelect(filter);
                    }
                    break;
                case TypeModelEnum.Oracle:
                    {
                        Sintaxe = model.OracleSelect(filter);
                    }
                    break;
                case TypeModelEnum.MongoDB:
                    {
                        Sintaxe = model.MongoFind(filter);
                    }
                    break;
            }            
            return Sintaxe;
        }

        public static string Count(this DataModel model) 
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type) 
            {
                case TypeModelEnum.Postgres: 
                    {
                        Sintaxe = model.PostgreSQLCount();                        
                    }
                    break;
                case TypeModelEnum.Oracle:
                    {
                        Sintaxe = model.OracleCount();
                    }
                    break;
                case TypeModelEnum.MongoDB:
                    {
                        Sintaxe = model.MongoCount();
                    }
                    break;
            }            
            return Sintaxe;
        }

        public static string Insert(this DataModel model, bool ReturningKey)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type) 
            {
                case TypeModelEnum.Postgres: 
                    {
                        Sintaxe = DataExtensionPostgres.PostgreSQLInsert(model, ReturningKey);
                    }
                    break;
                case TypeModelEnum.Oracle:
                    {
                        Sintaxe = DataExtensionOracle.OracleInsert(model, ReturningKey);
                    }
                    break;
            }            
            return Sintaxe;
        }

        public static string Update(this DataModel model, IDataFilterCollection? Filter = null)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type)
            {
                case TypeModelEnum.Postgres:                    
                    {
                        Sintaxe = DataExtensionPostgres.PostgreSQLUpdate(model, Filter);                        
                    }
                    break;
                case TypeModelEnum.Oracle:
                    {
                        Sintaxe = DataExtensionOracle.OracleUpdate(model, Filter);
                    }
                    break;
            }
            return Sintaxe;
        }

        public static string Delete(this DataModel model, IDataFilterCollection? Filter = null)
        {
            // Iniciar o processo de montagem da Sintaxe
            string Sintaxe = string.Empty;

            switch (model.Type) 
            {
                case TypeModelEnum.Postgres: 
                    {
                        Sintaxe = DataExtensionPostgres.PostgreSQLDelete(model, Filter);                        
                    }
                    break;
                case TypeModelEnum.Oracle:
                    {
                        Sintaxe = DataExtensionOracle.OracleDelete(model, Filter);
                    }
                    break;
            }            
            return Sintaxe;            
        }       
        #endregion
    }
}
