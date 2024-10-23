using MongoDB.Bson;
using MongoDB.Driver;
using SLA.Domain.Application.Enumerators;
using SLA.Domain.Application.Process;
using SLA.Domain.Infra.Instruction;
using SLA.Domain.Infra.Interfaces;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace SLA.Infra.MongoDBNoSQL.Connector
{
    public class MongoDBConnector : IConnector
    {
        #region Properties
        private IConnectionSettings _settings;
        private IMongoClient _connection;
        private IMongoDatabase _db;
        public TypeModelEnum Type { get; set; }
        public bool Log { get ; set; }
        #endregion

        #region Constructor
        public MongoDBConnector(IConnectionSettings connectionSettings)
        {
            _settings = connectionSettings;
            Type = TypeModelEnum.MongoDB;
        }
        #endregion

        #region Internal
        public string GetConnectionString()
        {
            if (_settings.User != "")
            {
                return $"{_settings.HostType}://{_settings.User}:{_settings.Password}@{_settings.Host}:{_settings.Port}/{_settings.DataBase}?connectTimeoutMS={_settings.Timeout}&authSource=admin";
            }
            else 
            {
                return $"{_settings.HostType}://{_settings.Host}:{_settings.Port}/{_settings.DataBase}?connectTimeoutMS={_settings.Timeout}&authSource=admin";
            }
        }

        public IDbTransaction? Transaction()
        {
            throw new NotImplementedException("Transaction()");
        }

        public void Roolback(IDbTransaction? Transaction)
        {
            throw new NotImplementedException("Roolback()");
        }

        public void Commit(IDbTransaction? Transaction)
        {
            throw new NotImplementedException("Commit()");
        }

        public DataTable CollectionToDataTable(string Collection, List<BsonDocument> Records) 
        {
            DataTable Table = new DataTable(Collection);

            Records.ForEach(r => {
                foreach (BsonElement elm in r.Elements)
                {
                    if (!Table.Columns.Contains(elm.Name))
                    {
                        Table.Columns.Add(new DataColumn(elm.Name));
                    }

                }
                DataRow dr = Table.NewRow();
                foreach (BsonElement elm in r.Elements)
                {
                    dr[elm.Name] = elm.Value;

                }
                Table.Rows.Add(dr);
            });

            return Table;
        }
        #endregion

        #region Connection
        public void Open()
        {
            _connection = new MongoClient(GetConnectionString());
            _db = _connection.GetDatabase(_settings.DataBase);
        }

        public bool Active()
        {
            throw new NotImplementedException("Active()");
        }

        public void Close()
        {
            throw new NotImplementedException("Close()");
        }
        #endregion

        #region Connection Methods
        public ReturnModel<long> Count(string Command)
        {
            ReturnModel<long> result = new ReturnModel<long>();            
            try
            {
                Open();
                MongoInstruction? instruction = JsonSerializer.Deserialize<MongoInstruction>(Command);
                IMongoCollection<BsonDocument> collection = _db.GetCollection<BsonDocument>(instruction.Collection);
                var count = collection.CountDocuments(new BsonDocument());

                result.SetSuccess("Consulta realizada com sucesso.", count);
            }
            catch (MongoClientException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(-1, e.Message, Command, e.StackTrace));
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
            }
            finally 
            {                
            }

            return result;
        }

        public async Task<ReturnModel<long>> CountAsync(string Command)
        {
            ReturnModel<long> result = new ReturnModel<long>();
            try
            {
                Open();
                MongoInstruction? instruction = JsonSerializer.Deserialize<MongoInstruction>(Command);
                IMongoCollection<BsonDocument> collection = _db.GetCollection<BsonDocument>(instruction.Collection);
                var count = await collection.CountDocumentsAsync(new BsonDocument());

                result.SetSuccess("Consulta realizada com sucesso.", count);
            }
            catch (MongoClientException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(-1, e.Message, Command, e.StackTrace));
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
            }
            finally
            {
            }

            return result;
        }

        public ReturnModel<bool> Execute(string Command)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnModel<bool>> ExecuteAsync(string Command)
        {
            throw new NotImplementedException();
        }

        public ReturnModel<dynamic> ExecuteReturning(string Command)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnModel<dynamic>> ExecuteReturningAsync(string Command)
        {
            throw new NotImplementedException();
        }

        public ReturnModel<DataTable> Query(string Command)
        {
            ReturnModel<DataTable> result = new ReturnModel<DataTable>();
            string json = string.Empty;
            try
            {
                Open();
                MongoInstruction? instruction = JsonSerializer.Deserialize<MongoInstruction>(Command);
                IMongoCollection<BsonDocument> collection = _db.GetCollection<BsonDocument>(instruction.Collection);                
                BsonDocument regex = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(instruction.Regex);
                var Data = collection.Find(regex).ToList();
                DataTable Table = CollectionToDataTable(instruction.Collection, Data);
                result.SetSuccess("Consulta realizada com sucesso.", Table);
            }
            catch (MongoClientException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(-1, e.Message, Command, e.StackTrace));                
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));                
            }
            finally
            {                
            }
            return result;
        }

        public async Task<ReturnModel<DataTable>> QueryAsync(string Command)
        {
            ReturnModel<DataTable> result = new ReturnModel<DataTable>();
            string json = string.Empty;
            try
            {
                Open();
                MongoInstruction? instruction = JsonSerializer.Deserialize<MongoInstruction>(Command);
                IMongoCollection<BsonDocument> collection = _db.GetCollection<BsonDocument>(instruction.Collection);
                BsonDocument regex = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(instruction.Regex);
                var Data = await collection.FindAsync(regex);
                DataTable Table = CollectionToDataTable(instruction.Collection, Data.ToList());
                result.SetSuccess("Consulta realizada com sucesso.", Table);
            }
            catch (MongoClientException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(-1, e.Message, Command, e.StackTrace));
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
            }
            finally
            {

            }
            return result;
        }
        #endregion
    }
}
