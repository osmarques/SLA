using Npgsql;
using SLA.Domain.Application.Enumerators;
using SLA.Domain.Application.Process;
using SLA.Domain.Infra.Interfaces;
using System.Data;

namespace SLA.Infra.PostgreSQL.Connector
{
    public class PostgreSQLConnector : IConnector
    {
        #region Properties
        private IConnectionSettings _settings;        
        private IDbConnection _connection;
        public TypeModelEnum Type { get; set; }
        public bool Log { get; set; } = false;
        #endregion

        #region Constructor
        public PostgreSQLConnector(IConnectionSettings connectionSettings)
        {
            _settings = connectionSettings;
            Type = TypeModelEnum.Postgres;
        }
        #endregion

        #region Internal
        public string GetConnectionString()
        {
            return $"Server={_settings.Host};Port={_settings.Port};User ID={_settings.User};Password={_settings.Password};Database={_settings.DataBase};CommandTimeout={_settings.Timeout}";
        }

        public IDbTransaction? Transaction()
        {
            if (_settings.UseTransaction)
            {
                return _connection.BeginTransaction();
            }
            return default;
        }

        public void Roolback(IDbTransaction? Transaction)
        {
            if (_settings.UseTransaction && Transaction != null)
            {
                try
                {
                    Transaction.Rollback();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Transaction.Rollback >> {e.Message}");
                }
            }
        }

        public void Commit(IDbTransaction? Transaction)
        {
            if (_settings.UseTransaction && Transaction != null)
            {
                Transaction.Commit();
            }
        }
        #endregion

        #region Connection
        public void Open()
        {
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
        }

        public bool Active() 
        {
            if (_connection != null) 
            {
                if (_connection.State != ConnectionState.Open) 
                { 
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void Close() 
        { 
            _connection.Close();
        }
        #endregion

        #region Connection Methods
        public ReturnModel<long> Count(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<long> result = new ReturnModel<long>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                NpgsqlDataReader Reader = QueryCommand.ExecuteReader();
                DataTable Table = new DataTable();
                Table.Load(Reader);
                Commit(ExecuteTransaction);

                long count = Table.Rows.Count;

                result.SetSuccess("Consulta realizada com sucesso.", count);
            }
            catch (NpgsqlException e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public async Task<ReturnModel<long>> CountAsync(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<long> result = new ReturnModel<long>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                NpgsqlDataReader Reader = await QueryCommand.ExecuteReaderAsync();
                DataTable Table = new DataTable();
                Table.Load(Reader);
                Commit(ExecuteTransaction);

                long count = Table.Rows.Count;

                result.SetSuccess("Consulta realizada com sucesso.", count);
            }
            catch (NpgsqlException e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public ReturnModel<bool> Execute(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<bool> result = new ReturnModel<bool>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                var process = QueryCommand.ExecuteNonQuery();

                Commit(ExecuteTransaction);                

                result.SetSuccess("Instrução realizada com sucesso.", true);
            }
            catch (NpgsqlException e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public async Task<ReturnModel<bool>> ExecuteAsync(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<bool> result = new ReturnModel<bool>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                var Reader = await QueryCommand.ExecuteNonQueryAsync();
                
                Commit(ExecuteTransaction);

                result.SetSuccess("Instrução realizada com sucesso.", true);
            }
            catch (NpgsqlException e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                if (Log) Console.WriteLine(e.Message);
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public ReturnModel<dynamic> ExecuteReturning(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<dynamic> result = new ReturnModel<dynamic>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            //            
            try
            {
                // Realiza a instrução
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                NpgsqlDataReader Reader = QueryCommand.ExecuteReader();
                int affected = Reader.RecordsAffected;
                DataTable Table = new DataTable();
                Table.Load(Reader);
                Commit(ExecuteTransaction);

                // Captura os resultados
                if (Table.Rows.Count > 0)
                {
                    DataRow drs = Table.Rows[0];
                    var value = drs.ItemArray[0];
                    if (value.GetType().Name == "DBNull")
                    {
                        result.SetSuccess($"Instrução executada com sucesso. Foram alterada(s) {affected} linha(s).", value);
                    }
                    else
                    {
                        result.SetFail("Nenhum registro afetado para ínstrução requisitada.");
                    }
                }
                else
                {
                    result.SetFail("Nenhum registro afetado para ínstrução requisitada.");
                }
            }
            catch (NpgsqlException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public async Task<ReturnModel<dynamic>> ExecuteReturningAsync(string Command)
        {
            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            ReturnModel<dynamic> result = new ReturnModel<dynamic>();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            //            
            try
            {
                // Realiza a instrução
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                QueryCommand.CommandTimeout = _settings.Timeout;
                NpgsqlDataReader Reader = await QueryCommand.ExecuteReaderAsync();
                int affected = Reader.RecordsAffected;
                DataTable Table = new DataTable();                
                Table.Load(Reader);                
                Commit(ExecuteTransaction);

                // Captura os resultados
                if (Table.Rows.Count > 0)
                {
                    DataRow drs = Table.Rows[0];
                    var value = drs.ItemArray[0];
                    if (value.GetType().Name == "DBNull")
                    {
                        result.SetSuccess($"Instrução executada com sucesso. Foram alterada(s) {affected} linha(s).", value);
                    }
                    else
                    {
                        result.SetFail("Nenhum registro afetado para ínstrução requisitada.");
                    }
                }
                else
                {
                    result.SetFail("Nenhum registro afetado para ínstrução requisitada.");
                }
            }
            catch (NpgsqlException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;            
        }

        public ReturnModel<DataTable> Query(string Command)
        {
            ReturnModel<DataTable> result = new ReturnModel<DataTable>();
            string json = string.Empty;

            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();            
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                NpgsqlDataReader Reader = QueryCommand.ExecuteReader();
                DataTable Table = new DataTable();
                Table.Load(Reader);
                Commit(ExecuteTransaction);                

                result.SetSuccess("Consulta realizada com sucesso.", Table);
            }
            catch (NpgsqlException e)
            {                
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {                
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }

        public async Task<ReturnModel<DataTable>> QueryAsync(string Command)
        {
            ReturnModel<DataTable> result = new ReturnModel<DataTable>();
            string json = string.Empty;

            if (Log) Console.WriteLine($"SQL: {Command}");
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
            NpgsqlTransaction? ExecuteTransaction;
            ExecuteTransaction = Transaction() as NpgsqlTransaction;
            try
            {
                NpgsqlCommand QueryCommand = new NpgsqlCommand(Command, _connection as NpgsqlConnection);
                NpgsqlDataReader Reader = await QueryCommand.ExecuteReaderAsync();
                DataTable Table = new DataTable();
                Table.Load(Reader);
                Commit(ExecuteTransaction);

                result.SetSuccess("Consulta realizada com sucesso.", Table);
            }
            catch (NpgsqlException e)
            {
                result.SetFail("Falha ao executar a instrução.", new ErrorModel(e.ErrorCode, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            catch (Exception e)
            {
                result.SetError("Erro ao executar a instrução.", new ErrorModel(0, e.Message, Command, e.StackTrace));
                Roolback(ExecuteTransaction);
            }
            finally
            {
                if (result.IsSuccessful()) _connection.Close();
            }
            return result;
        }
        #endregion
    }
}
