using System;
using Newtonsoft.Json;

namespace AAG.Global.Contracts
{
    public class ApiResponse
    {
        [JsonProperty("serverTime")]
        public DateTime ServerTime { get; set; } = DateTime.Now;

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


        public ApiResponse(
              string message = null
            , int status = 200)
        {
            Status = status;
            Message = message;
        }
    }


    public class ApiResponse<T> : ApiResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        public ApiResponse(
              T data
            , string message = null
            , int status = 200) : base(message, status)
        {
            Data = data;
        }
    }
}