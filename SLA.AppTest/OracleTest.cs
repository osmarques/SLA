using SLA.AppTest.Models;
using SLA.Domain.Infra.Connection;
using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Data.Filters;
using SLA.Domain.Infra.Enumerators;
using SLA.Domain.Infra.Interfaces;
using SLA.Service.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using SLA.Infra.Oracle.Connector;

namespace SLA.AppTest
{
    public static class OracleTest
    {

        public static async Task TestAsync()
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("= >> INICIANDO APP DE TESTES                            =");
            Console.WriteLine("=========================================================");

            // Verificando conexão com banco de dados
            Console.WriteLine("= >> VERIFICANDO CONEXÃO COM BANCO DE DADOS             =");
            Console.WriteLine("= >> Carregando configuração                            =");
            var config = File.ReadAllText("..\\..\\..\\Connection.json");

            Console.WriteLine("= >> Convertendo dados para aplicação                   =");
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            List<DBConnectionModel>? Connections = new List<DBConnectionModel>();
            Connections = JsonSerializer.Deserialize<List<DBConnectionModel>>(config, options);
            DBConnectionModel? conn = Connections.Where(r => r.Type == ConnectorEnum.OracleDB).FirstOrDefault();

            Console.WriteLine("= >> Criando connector                                  =");
            IConnector connector = new OracleConnector(conn.Connection);


            Console.WriteLine("= >> Verificando status da conexão                      =");
            connector.Open();
            /*
            if (connector != null)
            {
                if (connector.Active())
                {
                    Console.WriteLine("= >> Conexão ATIVA!                                     =");
                }
                else
                    Console.WriteLine("= >> Conexão INATIVA!                                   =");
            }
            else
            {
                Console.WriteLine("= >> Conexão INATIVA!                                   =");
            }
            connector.Close();
            */
            Console.WriteLine("=========================================================");

            // Verificar consulta de registros basica
            Console.WriteLine("= >> Verificando consulta basica de registros           =");
            var reg = connector.Count("{\"Collection\" : \"locations\"}").Return;
            Console.WriteLine($"= >> Numero de registro encontrados: {reg}                =");
            Console.WriteLine("=========================================================");


            // Verificando consulta de registro com paginação
            Console.WriteLine($"= >> Verificando consulta de registro com paginação     =");
            DataService<LocationModel> Locais = new DataService<LocationModel>(connector);
            var data = Locais.ReadList();
            Console.WriteLine($"= >> Registros encontrados: {data.Return.Count()}                         =");
            Console.WriteLine("=========================================================");
            data.Return.ForEach(r => {
                Console.WriteLine($"{r.id}|{r.nome}|{r.etiqueta}|{r.Type}");
            });
            Console.WriteLine("=========================================================");
            data = await Locais.ReadListAsync();
            Console.WriteLine($"= >> Registros encontrados: {data.Return.Count()}                         =");
            Console.WriteLine("=========================================================");

            var cont = Locais.Count();
            Console.WriteLine($"= >> Registros encontrados: {cont.Return}                         =");
            Console.WriteLine("=========================================================");
            var contasync = await Locais.CountAsync();
            Console.WriteLine($"= >> Registros encontrados: {cont.Return}                         =");
            Console.WriteLine("=========================================================");

            // Verificando consulta a partir do Filter Collection
            Console.WriteLine($"= >> Verificando consulta a partir do Filter Collection =");
            DataFilterCollection filter = new DataFilterCollection();
            filter.Property.Add(new DataFilterLimit(5));
            // filter.Property.Add(new DataFilterPage(2));
            var fields = new DataFilterFields();
            fields.Fields.Add(new FieldsFilter()
            {
                Name = "id",
                Type = TypeCode.Int32,
                Method = SLA.Domain.Infra.Enumerators.TypeSQLFilterEnum.Equal,
                Value = "2"
            });
            filter.Property.Add(fields);
            data = Locais.ReadList(filter);

            data.Return.ForEach(r => {
                Console.WriteLine($"{r.id}|{r.nome}|{r.etiqueta}|{r.Type}");
            });
            Console.WriteLine("=========================================================");
            /*
            // Verificando consulta de registro especifico por PK
            Console.WriteLine($"= >> Verificando consulta de registro especifico por PK =");
            var rec = Paises.Read(20).Return;
            Console.WriteLine("=========================================================");
            Console.WriteLine($"{rec.codigo}|{rec.nome}|{rec.continente}|{rec.ibge}|{rec.ddi}");
            // Verificando consulta de registro especifico por Filter
            //Console.WriteLine("=========================================================");

            // FIM
            Console.WriteLine("= >> PROCESSO FINALIZADO                                =");
            Console.WriteLine("=========================================================");
            */
        }
    }
}


