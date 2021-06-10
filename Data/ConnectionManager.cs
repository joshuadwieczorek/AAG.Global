using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Polly;

namespace AAG.Global.Data
{
    public class ConnectionManager : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Bugsnag.IClient _bugSnag;
        private readonly DbConnection _connection;
        private readonly Polly.Retry.RetryPolicy _retryPolicy;
        private readonly Polly.Retry.AsyncRetryPolicy _retryPolicyAsync;

        private bool isConnectionOpen
            => _connection is not null && _connection.State == ConnectionState.Open;


        public DbConnection Connection
            => _connection;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="connection"></param>
        public ConnectionManager(
              ILogger logger
            , Bugsnag.IClient bugSnag
            , DbConnection connection)
        {
            _logger = logger;
            _bugSnag = bugSnag;
            _connection = connection;

            _retryPolicy = Policy
                .Handle<Exception>(e =>
                {
                    bugSnag.Notify(e);
                    logger.LogError("{e}", e);
                    return true;
                })
                .WaitAndRetry(5, i => TimeSpan.FromMilliseconds(i * 1000));

            _retryPolicyAsync = Policy
                .Handle<Exception>(e =>
                {
                    bugSnag.Notify(e);
                    logger.LogError("{e}", e);
                    return true;
                })
                .WaitAndRetryAsync(5, i => TimeSpan.FromMilliseconds(i * 1000));
        }


        /// <summary>
        /// Open connection async.
        /// </summary>
        /// <returns></returns>
        public void Open()
        {
            if (!isConnectionOpen)
                _retryPolicy.Execute(() => _connection.Open());
        }


        /// <summary>
        /// Open connection async.
        /// </summary>
        /// <returns></returns>
        public async Task OpenAsync()
        {
            if (!isConnectionOpen)
                //await _retryPolicyAsync.ExecuteAsync(() => _connection.OpenAsync());
                await _connection.OpenAsync();
        }


        /// <summary>
        /// Close connection.
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            if (isConnectionOpen)
                _retryPolicy.Execute(() => _connection.Close());
        }


        /// <summary>
        /// Close connection async.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            if (isConnectionOpen)
                await _retryPolicyAsync.ExecuteAsync(() => _connection.CloseAsync());
        }


        /// <summary>
        /// Garbage cleanup.
        /// </summary>
        public void Dispose()
        {
            Close();
            _connection?.Dispose();
        }


        /// <summary>
        /// Query database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Query<T>(
              Func<IDbConnection, T> query
            , bool openDatabaseConnection = true)
        {
            try
            {
                if (openDatabaseConnection)
                    Open();
                T result = _retryPolicy.Execute<T>(() => query(_connection));
                if (openDatabaseConnection)
                    Close();
                return result;
            }
            catch (Exception te)
            {
                _bugSnag.Notify(te);
                _logger.LogError("{e}", te);
                throw;
            }            
        }


        /// <summary>
        /// Query database async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<T> QueryAsync<T>(
              Func<IDbConnection, Task<T>> query
            , bool openDatabaseConnection = true)
        {
            try
            {
                if (openDatabaseConnection)
                    await OpenAsync();
                T result = await _retryPolicyAsync.ExecuteAsync<T>(() => query(_connection));
                if (openDatabaseConnection)
                    await CloseAsync();
                return result;
            }
            catch (Exception te)
            {
                _bugSnag.Notify(te);
                _logger.LogError("{e}", te);
                throw;
            }
        }


        /// <summary>
        /// Execute call against database.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="openDatabaseConnection"></param>
        /// <returns></returns>
        public void Execute(
              Action<IDbConnection> process
            , bool openDatabaseConnection = true)
        {
            try
            {
                if (openDatabaseConnection)
                    Open();
                _retryPolicy.Execute(() => process(_connection));
                if (openDatabaseConnection)
                    Close();
            }
            catch (Exception te)
            {
                _bugSnag.Notify(te);
                _logger.LogError("{e}", te);
                throw;
            }
        }


        /// <summary>
        /// Execute call against database async.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="openDatabaseConnection"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(
              Func<IDbConnection, Task> process
            , bool openDatabaseConnection = true)
        {
            try
            {
                if (openDatabaseConnection)
                    await OpenAsync();
                await _retryPolicyAsync.ExecuteAsync(() => process(_connection));
                if (openDatabaseConnection)
                    await CloseAsync();
            }
            catch (Exception te)
            {
                _bugSnag.Notify(te);
                _logger.LogError("{e}", te);
                throw;
            }
        }
    }
}