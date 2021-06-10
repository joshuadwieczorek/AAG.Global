using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Polly;
using System.Collections.Generic;
using System.Collections;

namespace AAG.Global.Common
{
    /// <summary>
    /// Base logger without ILogger type.
    /// </summary>
    public abstract class BaseActor
    {
        protected readonly ILogger logger;
        protected readonly Bugsnag.IClient bugSnag;

        protected readonly object threadLock;
        protected virtual Polly.Retry.AsyncRetryPolicy AsyncRetryPolicy { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        /// <param name="parametersAreRequired"></param>
        public BaseActor(
              ILogger logger
            , Bugsnag.IClient bugSnag
            , bool parametersAreRequired = true)
        {
            threadLock = new object();

            if (parametersAreRequired)
            {
                if (logger is null)
                    throw new ArgumentNullException(nameof(logger));

                if (bugSnag is null)
                    throw new ArgumentNullException(nameof(bugSnag));
            }

            this.logger = logger;
            this.bugSnag = bugSnag;
        }


        /// <summary>
        /// Log error.
        /// </summary>
        /// <param name="e"></param>
        protected void LogError(Exception e)
        {
            var exceptionData = new Dictionary<string, object>();
            foreach (DictionaryEntry item in e.Data)
                exceptionData.Add(item.Key.ToString(), item.Value);

            bugSnag?.Notify(e, (report) =>
            {
                report.Event.Metadata.Add("Data", exceptionData);
            });

            logger?.LogError("{e}", e);
        }


        /// <summary>
        /// Do work with: nothing.
        /// </summary>
        /// <param name="doWork"></param>
        /// <returns></returns>
        protected void DoWork(Action doWork)
        {
            try
            {
                doWork();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }



        /// <summary>
        /// Do work with: input only.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="doWork"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected void DoWork<I>(
              Action<I> doWork
            , I parameter)
        {
            try
            {
                doWork(parameter);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        /// <summary>
        /// Do work with: output only.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="doWork"></param>
        /// <returns></returns>
        protected O DoWork<O>(Func<O> doWork)
        {
            try
            {
                return doWork();
            }
            catch (Exception e)
            {
                LogError(e);
                return default;
            }
        }


        /// <summary>
        /// Do work with: output, input.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="doWork"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected O DoWork<I,O>(
              Func<I, O> doWork
            , I parameter)
        {
            try
            {
                return doWork(parameter);
            }
            catch (Exception e)
            {
                LogError(e);
                return default;
            }
        }


        /// <summary>
        /// Do work async with: output only.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="doWork"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected async Task<O> DoWorkAsync<O>(Func<Task<O>> doWork)
        {
            try
            {
                return await doWork();
            }
            catch (Exception e)
            {
                LogError(e);
                return default;
            }
        }


        /// <summary>
        /// Do work async with: output {Task}, input.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="doWork"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected async Task DoWorkAsync<I, O>(
              Func<I, Task> doWork
            , I parameter)
        {
            try
            {
                await doWork(parameter);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        /// <summary>
        /// Do work async with: output {Task<O>}, input.
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="doWork"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected async Task<O> DoWorkAsync<I, O>(
                  Func<I, Task<O>> doWork
                , I parameter)
        {
            try
            {
                return await doWork(parameter);
            }
            catch (Exception e)
            {
                LogError(e);
                return default;
            }
        }
    }


    /// <summary>
    /// Base logger class with ILogger type. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseActor<T> : BaseActor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public BaseActor(
              ILogger<T> logger
            , Bugsnag.IClient bugSnag) : base(logger, bugSnag) { }

        
    }
}