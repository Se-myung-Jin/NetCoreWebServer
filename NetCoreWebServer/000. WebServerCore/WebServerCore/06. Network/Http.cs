using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

namespace WebServerCore
{
	public class Http : IDisposable
	{
		// private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		private static bool isInitialized = false;
		private HttpClientHandler handler;
		private SocketsHttpHandler socketHandler;
		private HttpClient client;
		private TimeSpan Timeout;
		private bool isDisposed;
		
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
		
		public Http(TimeSpan timeout, bool useSocketHandle = false)
		{
			HttpMessageHandler handle = null;
			if (useSocketHandle)
			{
				socketHandler = new SocketsHttpHandler
				{
					SslOptions = new System.Net.Security.SslClientAuthenticationOptions
					{
						// Leave certs unvalidated for debugging
						RemoteCertificateValidationCallback = delegate { return true; },
					},
					MaxConnectionsPerServer = int.MaxValue,
					PooledConnectionLifetime = TimeSpan.FromMinutes(1),
				};
				
				handle = socketHandler;
			}
			else
			{
				handler = new HttpClientHandler();
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					handler.ServerCertificateCustomValidationCallback = delegate { return true; };
				}
				handler.MaxConnectionsPerServer = int.MaxValue;
				handle = handler;
			}
			
			client = new HttpClient(handle);
			client.DefaultRequestHeaders.ExpectContinue = false;
			
			Timeout = timeout;
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
        
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed) return;

            if (disposing)
            {
	            socketHandler?.Dispose();
                handler?.Dispose();
                client?.Dispose();
            }

            this.isDisposed = true;
        }

        public async Task<Result> RequestAsync(
			HttpMethod method,
			string url,
			NameValueCollection queryParameters = null,
			WebHeaderCollection headers = null,
			byte[] body = null,
			string contentType = "application/x-www-form-urlencoded",
			int retry = 0,
			TimeSpan? timeout = null
		)
		{
			for (int i = 0; i <= retry; i++)
			{
				try
				{
					return await _RequestAsync(method, url, queryParameters, headers, body, contentType, timeout);
				}
				catch (Exception)
				{
					if (i >= retry) throw;
				}
			}
			return null;
		}

		private async Task<Result> _RequestAsync(
			HttpMethod method,
			string url,
			NameValueCollection queryParameters,
			WebHeaderCollection headers,
			byte[] body,
			string contentType,
			TimeSpan? timeout
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
							request.Headers.Add(key, headers[key]);
					}

					if (body != null && body.Length > 0)
					{

						content = new ByteArrayContent(body);

						request.Content = content;
						request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
					}

					using var cts = new CancellationTokenSource();
					timeout ??= Timeout;
					cts.CancelAfter(timeout.Value);
					
					using (var response = await client.SendAsync(request, cts.Token))
					{
						var data = await response.Content.ReadAsByteArrayAsync(cts.Token);

						byte[] result = data;

						return new Result()
						{
							StatusCode = response.StatusCode,
							Response = result
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

		public async Task<Result> GetAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, string contentType = null, int retry = 0)
		{
			return await RequestAsync(HttpMethod.Get, url, queryParameters, headers, null, contentType, retry: retry);
		}

		public async Task<Result> PostAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, byte[] body = null, string contentType = null, int retry = 0)
		{
			if (contentType == null) contentType = "application/x-www-form-urlencoded";
			return await RequestAsync(HttpMethod.Post, url, queryParameters, headers, body, contentType, retry);
		}

		public async Task<Result> PutAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, byte[] body = null, string contentType = null, int retry = 0)
		{
			if (contentType == null) contentType = "application/x-www-form-urlencoded";
			return await RequestAsync(HttpMethod.Put, url, queryParameters, headers, body, contentType, retry: retry);
		}

		public async Task<Result> DeleteAsync(string url, NameValueCollection queryParameters = null, WebHeaderCollection headers = null, int retry = 0)
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
					sb.Append("&");

				sb.Append(key + "=" + queryParameters.Get(key));
			}

			return sb.ToString();
		}
	}
}
