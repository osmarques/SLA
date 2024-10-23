using SLA.AppTest.Models;
using SLA.Domain.Infra.Connection;
using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Data.Filters;
using SLA.Domain.Infra.Interfaces;
using SLA.Infra.PostgreSQL.Connector;
using SLA.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SLA.AppTest
{
    public static class PostgresTest
    {
        public static async Task<bool> TestAsync() 
        {
            Console.WriteLine("=========================================================");
            Console.WriteLine("= >> INICIANDO APP DE TESTES                            =");
            Console.WriteLine("=========================================================");

            // Verificando conexão com banco de dados
            Console.WriteLine("= >> VERIFICANDO CONEXÃO COM BANCO DE DADOS             =");
            Console.WriteLine("= >> Carregando configuração                            =");
            var config = File.ReadAllText("D:\\Projetos\\Azure\\SLA\\SLA.AppTest\\Connection.json");

            Console.WriteLine("= >> Convertendo dados para aplicação                   =");
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            List<DBConnectionModel>? connList = JsonSerializer.Deserialize<List<DBConnectionModel>>(config, options);

            var conn = connList.Where(r => r.Type == Domain.Infra.Enumerators.ConnectorEnum.PostgreSQL).First();

            Console.WriteLine("= >> Criando connector                                  =");
            IConnector connector = new PostgreSQLConnector(conn.Connection);

            Console.WriteLine("= >> Verificando status da conexão                      =");
            connector.Open();
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
            Console.WriteLine("=========================================================");

            // Verificar consulta de registros basica
            Console.WriteLine("= >> Verificando consulta basica de registros           =");
            var reg = connector.Count("SELECT * FROM pai_paises").Return;
            Console.WriteLine($"= >> Numero de registro encontrados: {reg}                =");
            Console.WriteLine("=========================================================");

            // Verificando consulta de registro com paginação
            Console.WriteLine($"= >> Verificando consulta de registro com paginação     =");
            DataService<PaisesModel> Paises = new DataService<PaisesModel>(connector);
            var data = Paises.ReadList();
            Console.WriteLine($"= >> Registros encontrados: {data.Return.Count()}                         =");
            Console.WriteLine("=========================================================");
            data.Return.ForEach(r => {
                Console.WriteLine($"{r.codigo}|{r.nome}|{r.continente}|{r.ibge}|{r.ddi}");
            });
            Console.WriteLine("=========================================================");
            data = await Paises.ReadListAsync();
            Console.WriteLine($"= >> Registros encontrados: {data.Return.Count()}                         =");
            Console.WriteLine("=========================================================");

            var cont = Paises.Count();
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
                Name = "codigo",
                Type = TypeCode.Int64,
                Method = SLA.Domain.Infra.Enumerators.TypeSQLFilterEnum.Equal,
                Value = "3"
            });
            filter.Property.Add(fields);
            data = Paises.ReadList(filter);

            data.Return.ForEach(r => {
                Console.WriteLine($"{r.codigo}|{r.nome}|{r.continente}|{r.ibge}|{r.ddi}");
            });
            Console.WriteLine("=========================================================");
            // Verificando consulta de registro especifico por PK
            Console.WriteLine($"= >> Verificando consulta de registro especifico por PK =");
            var rec = Paises.Read(20).Return;
            Console.WriteLine("=========================================================");
            Console.WriteLine($"{rec.codigo}|{rec.nome}|{rec.continente}|{rec.ibge}|{rec.ddi}");
            Console.WriteLine("=========================================================");
            Console.WriteLine($"= >> Alterando o registro retornado ====================");
            var continente = rec.continente;
            rec.continente = "MEUOVO"; // Barein
            Paises.Update(rec);
            // Verificando consulta de registro especifico por Filter
            //Console.WriteLine("=========================================================");

            // FIM
            Console.WriteLine("= >> PROCESSO FINALIZADO                                =");
            Console.WriteLine("=========================================================");

            return true;
        }
    }
}
