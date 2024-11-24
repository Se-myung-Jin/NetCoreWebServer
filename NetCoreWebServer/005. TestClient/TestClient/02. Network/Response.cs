using System.Net;
using System.Text;

namespace TestClient
{
    public class Response
    {
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Data { get; set; }

        public string ResponseString
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(Data);
                }
                catch (Exception ex)
                {
                }

                return default;
            }
        }

        public Response() { }

        public Response(HttpStatusCode statusCode, byte[] response)
        {
            StatusCode = statusCode;
            Data = response;
        }

        public Response(Response result)
        {
            StatusCode = result.StatusCode;
            Data = result.Data;
        }

        public virtual void Set(HttpStatusCode statusCode, byte[] response)
        {
            StatusCode = statusCode;
            Data = response;
        }

        public virtual void Set(Response result)
        {
            StatusCode = result.StatusCode;
            Data = result.Data;
        }

        public T ToObject<T>() where T : Response
        {
            try
            {
                string json = Encoding.UTF8.GetString(Data);

            }
            catch (Exception ex)
            {
            }

            return default;
        }

        public override string ToString()
        {
            return string.Format("StatusCode={0}, Response={1}", StatusCode, ResponseString);
        }
    }
}
