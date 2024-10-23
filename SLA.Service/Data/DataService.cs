using SLA.Domain.Application.Enumerators;
using SLA.Domain.Application.Process;
using SLA.Domain.Infra.Connection;
using SLA.Domain.Infra.Data;
using SLA.Domain.Infra.Data.Filters;
using SLA.Domain.Infra.Extensions;
using SLA.Domain.Infra.Interfaces;
using System.Data;

namespace SLA.Service.Data
{
    public class DataService<T> : IDataSet<T> where T : DataModel
    {
        #region Properties
        private readonly IConnector _connector;
        private T _tableEntity;
        #endregion

        #region Contructor
        public DataService(IConnector connector)
        {
            _connector = connector;
            _tableEntity = Activator.CreateInstance<T>();
            _tableEntity.SetType(_connector.Type);
        }
        #endregion

        #region Sincrono
        /// <summary>
        /// Obtem a contagem de registro do Model T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é a contagem de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// List<T> Lista = service.Count();
        /// </example>
        public ReturnModel<long> Count() 
        {
            ReturnModel<long> result = new ReturnModel<long>();
            try
            {
                var Query = _connector.Count(_tableEntity.Count());
                if (Query.IsSuccessful())
                {
                        result.SetSuccess("Contagem obtida com sucesso.", Query.Return);
                }
                else
                {
                    result.SetFail("Falha ao realizar a contagem: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao realizar a contagem.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem uma lista de registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é uma Lista de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// List<T> Lista = service.ReadList();
        /// </example>
        public ReturnModel<List<T>> ReadList()
        {
            ReturnModel<List<T>> result = new ReturnModel<List<T>>();
            try
            {                
                var Query = _connector.Query(_tableEntity.Select());
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        List<T> data = new List<T>();
                        switch (_tableEntity.Type) 
                        {
                            case TypeModelEnum.Postgres: 
                                {
                                    data = EntityDataManipulate.ConvertDataTable<T>(Table);
                                }
                                break;
                            case TypeModelEnum.MongoDB: 
                                {
                                    data = EntityDataManipulate.ConvertDocument<T>(Table, TypeModelEnum.MongoDB);
                                }
                                break;
                        }
                        
                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else 
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }                    
                }
                else
                {
                    result.SetFail("Falha ao obter a lista: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter a lista.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem uma lista de registro T, utilizando uma lista de atributos a serem filtrados
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é uma Lista de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// IDataFilterCollection filter() = new DataFilterCollection();
        /// FieldsFilter field = new FieldsFilter() 
        /// {
        ///     Name = "Nome",
        ///     Type = string,
        ///     Method = TypeSQLFilterEnum.Equal,
        ///     Value = "Brasil"
        /// }
        /// Filter.Property.Add(new DataFilterFields(field));
        /// List<T> Lista = service.ReadList(Filter);
        /// </example>
        public ReturnModel<List<T>> ReadList(IDataFilterCollection Filters)
        {
            ReturnModel<List<T>> result = new ReturnModel<List<T>>();
            try
            {
                var Query = _connector.Query(_tableEntity.Select(Filters));
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        var data = EntityDataManipulate.ConvertDataTable<T>(Table);
                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }
                }
                else
                {
                    result.SetFail("Falha ao obter a lista: " + Query.Message);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter a lista.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem um registro T, utilizando uma lista de atributos a serem filtrados
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é um registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// IDataFilterCollection filter() = new DataFilterCollection();
        /// Filter.Property.Add(new DataFilterKey(1));
        /// List<T> Lista = service.Read(Filter);
        /// </example>
        public ReturnModel<T> Read(long Id)
        {
            ReturnModel<T> result = new ReturnModel<T>();
            try
            {
                IDataFilterCollection Filter = new DataFilterCollection();
                Filter.Property.Add(new DataFilterKey(Id));
                var Query = _connector.Query(_tableEntity.Select(Filter));
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        var data = EntityDataManipulate.ConvertDataRow<T>(Table);
                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }
                }
                else
                {
                    result.SetFail("Falha ao obter o registro: " + Query.Message);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Grava um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja gravado</typeparam>
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é o registro gravado do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;                
        /// var Record = service.Create(Data);
        /// </example>
        public ReturnModel<T> Create(T Data)
        {
            ReturnModel<T> result = new ReturnModel<T>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = _connector.ExecuteReturning(Data.Insert(true));
                if (Query.IsSuccessful())
                {
                    var Instruction = Read(Query.Return);
                    if (Instruction.Validate())
                    {                        
                        result.SetSuccess("Registro cadastrado com sucesso.", Instruction.Return);
                    }
                    else
                    {
                        result.SetFail($"Falha ao recuperar o registro inserido. Detalhes: {Instruction.Message}", Instruction.Erro);
                    }
                }
                else
                {
                    result.SetFail("Falha ao inserir o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao inserir o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Atualiza um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja gravado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é um booleano sobre a atualização do registro.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// var Record = service.Update(Data);
        /// </example>
        public ReturnModel<bool> Update(T Data)
        {
            ReturnModel<bool> result = new ReturnModel<bool>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = _connector.Execute(Data.Update());
                if (Query.IsSuccessful())
                {
                    result.SetSuccess("Registro atualizado com sucesso.", true);
                }
                else
                {
                    result.SetFail("Falha ao atualizar o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao atualizar o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Remove um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja remover</typeparam>        
        /// <returns>Retorna um ReturnModel<bool>, onde o atributo RETURN é um booleano sobre a remoção do registro.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// var Record = service.Delete(Data);
        /// </example>
        public ReturnModel<bool> Delete(T Data)
        {
            ReturnModel<bool> result = new ReturnModel<bool>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = _connector.Execute(Data.Delete());
                if (Query.IsSuccessful())
                {
                    result.SetSuccess("Registro removido com sucesso.", Query.IsSuccessful());
                }
                else
                {
                    result.SetFail("Falha ao remover o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao remover o registro.", erro);
            }
            return result;
        }
        #endregion

        #region Assincrono
        /// <summary>
        /// Obtem a contagem de registro do Model T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é a contagem de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// List<T> Lista = await service.CountAsync();
        /// </example>
        public async Task<ReturnModel<long>> CountAsync() 
        {
            ReturnModel<long> result = new ReturnModel<long>();
            try
            {
                var Query = await _connector.CountAsync(_tableEntity.Count());
                if (Query.IsSuccessful())
                {                    
                    result.SetSuccess("Contagem obtida com sucesso.", Query.Return);
                }
                else
                {
                    result.SetFail("Falha ao realizar a contagem: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao realizar a contagem.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem uma lista de registro T Assincrona
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETORNO é uma Lista de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// List<T> Lista = await service.ReadListAsync();
        /// </example>
        public async Task<ReturnModel<List<T>>> ReadListAsync()
        {
            ReturnModel<List<T>> result = new ReturnModel<List<T>>();
            try
            {
                var Query = await _connector.QueryAsync(_tableEntity.Select());
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        List<T> data = new List<T>();
                        switch (_tableEntity.Type)
                        {
                            case TypeModelEnum.Postgres:
                                {
                                    data = EntityDataManipulate.ConvertDataTable<T>(Table);
                                }
                                break;
                            case TypeModelEnum.MongoDB:
                                {
                                    data = EntityDataManipulate.ConvertDocument<T>(Table, TypeModelEnum.MongoDB);
                                }
                                break;
                        }

                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }
                }
                else
                {
                    result.SetFail("Falha ao obter a lista: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter a lista.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem uma lista de registro T, utilizando uma lista de atributos a serem filtrados
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é uma Lista de registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// IDataFilterCollection filter() = new DataFilterCollection();
        /// FieldsFilter field = new FieldsFilter() 
        /// {
        ///     Name = "Nome",
        ///     Type = string,
        ///     Method = TypeSQLFilterEnum.Equal,
        ///     Value = "Brasil"
        /// }
        /// Filter.Property.Add(new DataFilterFields(field));
        /// List<T> Lista = await service.ReadListAsync(Filter);
        /// </example>
        public async Task<ReturnModel<List<T>>> ReadListAsync(IDataFilterCollection Filters)
        {
            ReturnModel<List<T>> result = new ReturnModel<List<T>>();
            try
            {
                var Query = await _connector.QueryAsync(_tableEntity.Select(Filters));
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        var data = EntityDataManipulate.ConvertDataTable<T>(Table);
                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }
                }
                else
                {
                    result.SetFail("Falha ao obter a lista: " + Query.Message);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter a lista.", erro);
            }
            return result;
        }

        /// <summary>
        /// Obtem um registro T, utilizando uma lista de atributos a serem filtrados
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja ser recuperado</typeparam>        
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é um registro do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// IDataFilterCollection filter() = new DataFilterCollection();
        /// Filter.Property.Add(new DataFilterKey(1));
        /// List<T> Lista = await service.ReadAsync(Filter);
        /// </example>
        public async Task<ReturnModel<T>> ReadAsync(long Id)
        {
            ReturnModel<T> result = new ReturnModel<T>();
            try
            {
                IDataFilterCollection Filter = new DataFilterCollection();
                Filter.Property.Add(new DataFilterKey(Id));
                var Query = await _connector.QueryAsync(_tableEntity.Select(Filter));
                if (Query.IsSuccessful())
                {
                    DataTable Table = Query.Return;
                    if (Table != null)
                    {
                        var data = EntityDataManipulate.ConvertDataRow<T>(Table);
                        result.SetSuccess("Lista obtida com sucesso.", data);
                    }
                    else
                    {
                        result.SetFail("Falha ao obter a lista: Dados inexistentes");
                    }
                }
                else
                {
                    result.SetFail("Falha ao obter o registro: " + Query.Message);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao obter o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Grava um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja gravado</typeparam>
        /// <typeparam name="IDataFilterCollection">Collection que registra os tipos de filtros a serem aplicados</typeparam>
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é o registro gravado do tipo T.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// var Record = await service.CreateAsync(Data);
        /// </example>
        public async Task<ReturnModel<T>> CreateAsync(T Data)
        {
            ReturnModel<T> result = new ReturnModel<T>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = await _connector.ExecuteReturningAsync(Data.Insert(true));
                if (Query.IsSuccessful())
                {
                    var Instruction = Read(Query.Return);
                    if (Instruction.Validate())
                    {
                        result.SetSuccess("Registro cadastrado com sucesso.", Instruction.Return);
                    }
                    else
                    {
                        result.SetFail($"Falha ao recuperar o registro inserido. Detalhes: {Instruction.Message}", Instruction.Erro);
                    }
                }
                else
                {
                    result.SetFail("Falha ao inserir o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao inserir o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Atualiza um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja gravado</typeparam>        
        /// <returns>Retorna um ReturnModel<T>, onde o atributo RETURN é um booleano sobre a atualização do registro.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// var Record = await service.UpdateAsync(Data);
        /// </example>
        public async Task<ReturnModel<bool>> UpdateAsync(T Data)
        {
            ReturnModel<bool> result = new ReturnModel<bool>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = await _connector.ExecuteAsync(Data.Update());
                if (Query.IsSuccessful())
                {
                    result.SetSuccess("Registro atualizado com sucesso.", true);
                }
                else
                {
                    result.SetFail("Falha ao atualizar o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao atualizar o registro.", erro);
            }
            return result;
        }

        /// <summary>
        /// Remove um registro T
        /// </summary>
        /// <typeparam name="T">Model do registro que deseja remover</typeparam>        
        /// <returns>Retorna um ReturnModel<bool>, onde o atributo RETURN é um booleano sobre a remoção do registro.</returns>
        /// <example>
        /// DataService<T> service = new DataService<T>;
        /// var Record = await service.DeleteAsync(Data);
        /// </example>
        public async Task<ReturnModel<bool>> DeleteAsync(T Data)
        {
            ReturnModel<bool> result = new ReturnModel<bool>();
            Data.SetType(_tableEntity.Type);
            try
            {
                var Query = await _connector.ExecuteAsync(Data.Delete());
                if (Query.IsSuccessful())
                {
                    result.SetSuccess("Registro removido com sucesso.", Query.IsSuccessful());
                }
                else
                {
                    result.SetFail("Falha ao remover o registro: " + Query.Message, Query.Erro);
                }
            }
            catch (Exception e)
            {
                ErrorModel erro = new ErrorModel() { Code = 0, Message = e.Message, Detail = e.Source, Stack = e.StackTrace };
                result.SetError("Erro ao remover o registro.", erro);
            }
            return result;
        }
        #endregion
    }
}