using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace WebServerCore
{
    public class Result
    {
        protected static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Response { get; set; }

        public string ResponseString
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(Response);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return default;
            }
        }

        public Result() { }

        public Result(HttpStatusCode statusCode, byte[] response)
        {
            StatusCode = statusCode;
            Response = response;
        }

        public Result(Result result)
        {
            StatusCode = result.StatusCode;
            Response = result.Response;
        }

        public virtual void Set(HttpStatusCode statusCode, byte[] response)
        {
            StatusCode = statusCode;
            Response = response;
        }

        public virtual void Set(Result result)
        {
            StatusCode = result.StatusCode;
            Response = result.Response;
        }

        public T ToObject<T>() where T : Result
        {
            try
            {
                string json = Encoding.UTF8.GetString(Response);

                var obj = JsonConvert.DeserializeObject<T>(json);
                obj.StatusCode = this.StatusCode;
                obj.Response = this.Response;

                return obj;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return default;
        }

        public override string ToString()
        {
            return string.Format("StatusCode={0}, Response={1}", StatusCode, ResponseString);
        }
    }
}