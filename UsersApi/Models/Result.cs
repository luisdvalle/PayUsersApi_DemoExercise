using System.Net;

namespace UsersApi.Models
{
    public class Result
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
