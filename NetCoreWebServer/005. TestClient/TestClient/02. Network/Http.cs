﻿using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace TestClient
{
    public class Http : IDisposable
    {
        private static bool isInitialized = false;

        static Http()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (isInitialized == false)
            {
                isInitialized = true;
                ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                ServicePointManager.EnableDnsRoundRobin = true;
            }
        }

        private HttpClientHandler handler;
        private HttpClient client;

        public Http(TimeSpan timeout)
        {
            handler = new HttpClientHandler();
            handler.MaxConnectionsPerServer = int.MaxValue;

            client = new HttpClient(handler);
            client.Timeout = timeout;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            }

            client.DefaultRequestHeaders.ExpectContinue = false;
        }

        ~Http()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed) return;

            if (disposing)
            {
                handler.Dispose();
                client.Dispose();
            }

            this.isDisposed = true;
        }

        public TimeSpan Timeout
        {
            get
            {
                return client.Timeout;
            }
            set
            {
                client.Timeout = value;
            }
        }

        public async Task<Response> RequestAsync(
            HttpMethod method,
            string url,
            NameValueCollection queryParameters = null,
            WebHeaderCollection headers = null,
            byte[] body = null,
            string contentType = "application/x-www-form-urlencoded",
            int retry = 0
        )
        {
            for (int i = 0; i <= retry; i++)
            {
                try
                {
                    return await RequestAsync(method, url, queryParameters, headers, body, contentType);
                }
                catch (Exception)
                {
                    if (i >= retry) throw;
                }
            }

            return null;
        }

        private async Task<Response> RequestAsync(
            HttpMethod method,
            string url,
            NameValueCollection queryParameters,
            WebHeaderCollection headers,
            byte[] body,
            string contentType
        )
        {
            string parameterString = GetParameterString(queryParameters, true);
            url += parameterString;

            ByteArrayContent content = null;

            try
            {
                using (var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(url);
                    request.Method = method;

                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var key in headers.AllKeys)
                        {
                            request.Headers.Add(key, headers[key]);
                        }
                    }

                    if (body != null && body.Length > 0)
                    {
                        content = new ByteArrayContent(body);

                        request.Content = content;
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }

                    using (var response = await client.SendAsync(request))
                    {
                        var data = await response.Content.ReadAsByteArrayAsync();

                        byte[] result;

                        result = data;

                        return new Response()
                        {
                            StatusCode = response.StatusCode,
                            Data = result
                        };
                    }
                }
            }
            finally
            {
                content?.Dispose();
            }
        }

        private static async Task<WebResponse> GetResponseWithoutExceptionAsync(WebRequest request)
        {
            try
            {
                return await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                return ex.Response;
            }
        }

        public async Task<Response> GetAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, string contentType = null, int retry = 0)
        {
            return await RequestAsync(HttpMethod.Get, url, queryParameters, headers, null, contentType, retry: retry);
        }

        public async Task<Response> PostAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, byte[] body = null, string contentType = null, int retry = 0)
        {
            if (contentType == null) contentType = "application/x-www-form-urlencoded";
            return await RequestAsync(HttpMethod.Post, url, queryParameters, headers, body, contentType, retry);
        }

        public async Task<Response> PutAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, byte[] body = null, string contentType = null, int retry = 0)
        {
            if (contentType == null) contentType = "application/x-www-form-urlencoded";
            return await RequestAsync(HttpMethod.Put, url, queryParameters, headers, body, contentType, retry: retry);
        }

        public async Task<Response> DeleteAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, int retry = 0)
        {
            return await RequestAsync(HttpMethod.Delete, url, queryParameters, headers, retry: retry);
        }

        private static string GetParameterString(NameValueCollection queryParameters, bool withQuestion)
        {
            if (queryParameters == null || queryParameters.Count == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();

            bool isFirst = true;
            foreach (var key in queryParameters.AllKeys)
            {
                if (isFirst)
                {
                    isFirst = false;

                    if (withQuestion) sb.Append("?");
                }
                else
                {
                    sb.Append("&");
                }

                sb.Append(key + "=" + queryParameters.Get(key));
            }

            return sb.ToString();
        }
    }
}
