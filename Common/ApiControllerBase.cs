using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AAG.Global.Common
{
    public abstract class ApiControllerBase<T> : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly Bugsnag.IClient _bugSnag;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bugSnag"></param>
        public ApiControllerBase(
              ILogger<T> logger
            , Bugsnag.IClient bugSnag)
        {
            _logger = logger;
            _bugSnag = bugSnag;
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

            _bugSnag?.Notify(e, (report) =>
            {
                report.Event.Metadata.Add("Data", exceptionData);
            });

            _logger?.LogError("{e}", e);
        }
    }
}