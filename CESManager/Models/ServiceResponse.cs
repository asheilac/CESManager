using System.Net;

namespace CESManager.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success => StatusCode == CESManagerStatusCode.Ok;
        public string Message { get; set; }
        public CESManagerStatusCode StatusCode { get; set; }
    }
}