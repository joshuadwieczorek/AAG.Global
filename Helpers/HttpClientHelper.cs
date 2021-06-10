using System;
using System.Net.Http;

namespace AAG.Global.Helpers
{
    /// <summary>
    /// Http client helper for a singleton pattern.
    /// </summary>
    public static class HttpClientHelper
    {
        /// <summary>
        /// HTTP client instance.
        /// </summary>
        private static HttpClient client;


        /// <summary>
        /// Configure HTTP client instance.
        /// </summary>
        /// <param name="configureClient"></param>
        /// <param name="reGenerateClient"></param>
        public static void Configure(
              Action<HttpClient> configureClient
            , bool reGenerateClient = false)
        {
            if (client is not null && reGenerateClient)
            {
                client.Dispose();
                GenerateClient();
            }
            else if (client is null)
                GenerateClient();

            configureClient(client);
        }


        /// <summary>
        /// Configure and get HTTP client instance.
        /// </summary>
        /// <param name="configureClient"></param>
        /// <param name="reGenerateClient"></param>
        /// <returns></returns>
        public static HttpClient GetClient(
              Action<HttpClient> configureClient
            , bool reGenerateClient = false)
        {
            Configure(configureClient, reGenerateClient);
            return client;
        }


        /// <summary>
        /// Get HTTP client instance.
        /// </summary>
        /// <returns></returns>
        public static HttpClient GetClient()
        {
            if (client is null)
                GenerateClient();

            return client;
        }


        /// <summary>
        /// Generate new HTTP client and set field.
        /// </summary>
        private static void GenerateClient()
            => client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
    }   
}