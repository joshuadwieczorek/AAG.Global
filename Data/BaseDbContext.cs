using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using Polly;
using Microsoft.Extensions.Configuration;
using AAG.Global.ExtensionMethods;
using AAG.Global.Common;

namespace AAG.Global.Data
{
    public class BaseDbContext : BaseActor, IDisposable
    {        
        protected readonly Bugsnag.IClient _bugSnag;              
        protected Boolean connectionOpened;
        protected readonly Object threadLock;
        protected readonly Polly.Retry.AsyncRetryPolicy retryPolicy;

        [Obsolete("connection is deprecated, please use the ConnectionManager instead.")]
        protected readonly SqlConnection connection;

        protected ILogger logger { get; }
        public ConnectionManager ConnectionManager { get; init; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="connectionString"></param>
        public BaseDbContext(
              IConfiguration configuration
            , ILogger logger
            , Bugsnag.IClient bugSnag
            , string connectionString) : base (logger, bugSnag)
        {
            this.logger = logger;
            _bugSnag = bugSnag;
            connection = new SqlConnection(connectionString);

            // START: Added new connection manager.
            ConnectionManager = new ConnectionManager(logger, bugSnag, connection);
            // END: Added new connection manager.


            connectionOpened = false;

            var maxRetryAttempts = configuration["RetryPolicyMaxAttemptsMillisecods"].ToInt(3);

            retryPolicy = Policy
                .Handle<Exception>(e => 
                {
                    bugSnag.Notify(e);
                    logger.LogError("{e}", e);
                    return true;
                })
                .WaitAndRetryAsync(maxRetryAttempts, i => TimeSpan.FromMilliseconds(i * maxRetryAttempts));
        }


        /// <summary>
        /// Open sql connection.
        /// </summary>
        /// <returns></returns>
        [Obsolete("OpenConnectionAsync is deprecated, please use the ConnectionManager instead.")]
        public async Task OpenConnectionAsync()
        {
            await connection.OpenAsync();
            connectionOpened = true;
        }


        /// <summary>
        /// Close sql connection.
        /// </summary>
        /// <returns></returns>
        [Obsolete("CloseConnectionAsync is deprecated, please use the ConnectionManager instead.")]
        public async Task CloseConnectionAsync()
        {
            await connection.CloseAsync();
            connectionOpened = false;
        }


        /// <summary>
        /// Query async wrapper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getData"></param>
        /// <returns></returns>
        [Obsolete("Query is deprecated, please use the ConnectionManager instead.")]
        protected async Task<O> Query<O>(Func<IDbConnection, Task<O>> getData)
        {
            try
            {
                if (!connectionOpened)
                    await connection.OpenAsync();
                O result = await getData(connection);
                if (connectionOpened)
                    await connection.CloseAsync();
                return result;
            }
            catch (TimeoutException te)
            {
                _bugSnag.Notify(te);
                logger.LogError("{e}", te);
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), te);
            }
            catch (SqlException se)
            {
                _bugSnag.Notify(se);
                logger.LogError("{e}", se);
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), se);
            }
        }


        /// <summary>
        /// Execute async wrapper.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        [Obsolete("Execute is deprecated, please use the ConnectionManager instead.")]
        protected async Task Execute(Func<IDbConnection, Task> process)
        {
            try
            {
                if (!connectionOpened)
                    await connection.OpenAsync();
                await process(connection);
                if (connectionOpened)
                    await connection.CloseAsync();
            }
            catch (TimeoutException te)
            {
                _bugSnag.Notify(te);
                logger.LogError("{e}", te);
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), te);
            }
            catch (SqlException se)
            {
                _bugSnag.Notify(se);
                logger.LogError("{e}", se);
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), se);
            }
        }


        /// <summary>
        /// Execute query.
        /// </summary>
        /// <param name="storedProcedure">Stored procedure name.</param>
        /// <param name="parameters">Nullable stored procedure parameters.</param>
        /// <returns></returns>
        public async Task Execute(
              string storedProcedure
            , object parameters = null
            , bool openDatabaseConnection = true)
        {
            if (parameters is not null)
                await ConnectionManager.ExecuteAsync(connection => (connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure)), openDatabaseConnection);
            else
                await ConnectionManager.ExecuteAsync(connection => (connection.ExecuteAsync(storedProcedure, commandType: CommandType.StoredProcedure)), openDatabaseConnection);
        }



        /// <summary>
        /// Run query.
        /// </summary>
        /// <param name="storedProcedure">Stored procedure name.</param>
        /// <param name="parameters">Nullable stored procedure parameters.</param>
        /// <returns></returns>
        public async Task<R> QuerySingle<R>(
              string text
            , object parameters = null
            , CommandType commandType = CommandType.StoredProcedure
            , bool openDatabaseConnection = true)
        {
            if (parameters is not null)
                return await ConnectionManager.QueryAsync<R>(async connection => (await connection.QueryFirstOrDefaultAsync<R>(text, parameters, commandType: commandType)), openDatabaseConnection);
            else
                return await ConnectionManager.QueryAsync<R>(async connection => (await connection.QueryFirstOrDefaultAsync<R>(text, commandType: commandType)), openDatabaseConnection);
        }


        /// <summary>
        /// Run query.
        /// </summary>
        /// <param name="storedProcedure">Stored procedure name.</param>
        /// <param name="parameters">Nullable stored procedure parameters.</param>
        /// <returns></returns>
        public async Task<IEnumerable<R>> QueryMulti<R>(
              string text
            , object parameters = null
            , CommandType commandType = CommandType.StoredProcedure
            , bool openDatabaseConnection = true)
        {
            IEnumerable<R> results = null;

            if (parameters is not null)
                results = await ConnectionManager.QueryAsync<List<R>>(
                    async connection => (await connection.QueryAsync<R>(text, parameters, commandType: commandType)).ToList(),
                    openDatabaseConnection
                );
            else
                results = await ConnectionManager.QueryAsync<List<R>>(
                    async connection => (await connection.QueryAsync<R>(text, commandType: CommandType.StoredProcedure)).ToList(),
                    openDatabaseConnection
                );

            return results;
        }


        /// <summary>
        /// Bulk copy data to database.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task BulkCopy(
              DataTable table
            , bool openDatabaseConnection = true)
        {
            await ConnectionManager.ExecuteAsync(async (connection) =>
            {
                var conn = connection as SqlConnection;
                using SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                foreach (DataColumn column in table.Columns)
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.BulkCopyTimeout = 0;
                try
                {
                    await bulkCopy.WriteToServerAsync(table);
                }
                catch(Exception e)
                {
                    _bugSnag.Notify(e);
                    logger.LogError("{e}", e);
                }
            }, openDatabaseConnection);
        }


        /// <summary>
        /// Bulk copy data to database async.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task BulkCopyAsync(
              DataTable table
            , bool openDatabaseConnection = true)
        {
            await ConnectionManager.ExecuteAsync(async (connection) =>
            {
                var conn = connection as SqlConnection;
                using SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                foreach (DataColumn column in table.Columns)
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                bulkCopy.DestinationTableName = table.TableName;
                await bulkCopy.WriteToServerAsync(table);
            }, openDatabaseConnection);
        }


        // Garbage cleanup.
        public void Dispose()
        {
            if (connection is null)
                return;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            connection.Dispose();

            ConnectionManager.Dispose();
        }
    }


    public class BaseDbContext<T> : BaseDbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="connectionString"></param>
        public BaseDbContext(
              IConfiguration configuration
            , ILogger<T> logger
            , Bugsnag.IClient bugSnag
            , string connectionString) : base(configuration, logger, bugSnag, connectionString) { }
    }
}