using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OsmSharp.UI.Map.Layers.HttpClients;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using OkHttp;
using System.Net.Http;
using System.Net;

namespace OsmSharp.Android.UI.Net
{
    /// <summary>
    /// Represents and wraps a native http client.
    /// </summary>
    public class HttpClient : IHttpClient, IDisposable
    {
        /// <summary>
        /// Holds the native http client.
        /// </summary>
        private OkHttpClient _nativeClient;

        /// <summary>
        /// Holds the request message.
        /// </summary>
        private HttpRequestMessage _request;

        /// <summary>
        /// Creates a new native http client.
        /// </summary>
        public HttpClient(OkHttpClient nativeClient)
        {
            _nativeClient = nativeClient;
            _request = new HttpRequestMessage();
        }

        /// <summary>
        /// Returns the stream for the given uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Stream GetStream(string uri)
        {
            // set request url.
            _request.RequestUri = new Uri(uri);

            var javaUri = _request.RequestUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);
            var url = new Java.Net.URL(javaUri);
            Java.Net.HttpURLConnection rq;
            try
            {
                rq = _nativeClient.Open(url);
            }
            catch (Java.Net.UnknownHostException e)
            {
                throw new WebException("Name resolution failure", e, WebExceptionStatus.NameResolutionFailure, null);
            }
            rq.RequestMethod = _request.Method.Method.ToUpperInvariant();

            foreach (var kvp in _request.Headers) { rq.SetRequestProperty(kvp.Key, kvp.Value.FirstOrDefault()); }

            if (_request.Content != null)
            {
                throw new NotSupportedException("HttpContent not supported.");
            }

            var ret = default(HttpResponseMessage);

            try
            {
                ret = new HttpResponseMessage((HttpStatusCode)rq.ResponseCode);
            }
            catch (Java.Net.UnknownHostException e)
            {
                throw new WebException("Name resolution failure", e, WebExceptionStatus.NameResolutionFailure, null);
            }
            catch (Java.Net.ConnectException e)
            {
                throw new WebException("Connection failed", e, WebExceptionStatus.ConnectFailure, null);
            }

            return rq.InputStream;
        }

        /// <summary>
        /// Adds a new media type to the accept header.
        /// </summary>
        /// <param name="mediaType"></param>
        public void AddAcceptHeader(string mediaType)
        {
            _request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaType));
        }

        /// <summary>
        /// Clears all media types from the accept header.
        /// </summary>
        public void ClearAcceptHeader()
        {
            _request.Headers.Accept.Clear();
        }

        /// <summary>
        /// Sets the useragent.
        /// </summary>
        /// <param name="userAgent"></param>
        public void SetUserAgent(string userAgent)
        {
            _request.Headers.UserAgent.Clear();
            _request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(userAgent, "4"));
        }

        /// <summary>
        /// Diposes all resources associated with this client.
        /// </summary>
        public void Dispose()
        {
            _request.Dispose();
            _nativeClient.Dispose();
        }
    }
}