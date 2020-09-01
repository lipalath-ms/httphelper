using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace OneCatalogReaderForOrg
{
    /// <summary>
    /// HttpHelper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Set TimeOut
        /// </summary>
        public static Int32 HttpRequestTimeOut = 10000;

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static HttpResponseData Get(string url)
        {
            return Invoke<Object>("GET", url, null, null);
        }
        public static HttpResponseData GetForOData(string url)
        {
            return InvokeForOData<Object>("GET", url, null, null);
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="Id">param</param>
        /// <returns></returns>
        public static HttpResponseData Get(string url, Object id)
        {
            return Invoke<Object>("GET", url, id, null);
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="Id">param（optional）</param>
        /// <param name="Data">Data {json format}</param>
        /// <returns></returns>
        public static HttpResponseData Post<T>(string url, Object id, T data)
        {
            return Invoke("POST", url, id, data);
        }

        /// <summary>
        /// PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="url">url</param>

        /// <param name="Data">Data {json format}</param>

        /// <returns></returns>

        public static HttpResponseData Put<T>(string url, T data)
        {
            return Invoke("PUT", url, null, data);
        }

        /// <summary>
        /// PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">yrl</param>
        /// <param name="Id">URL（optional）</param>
        /// <param name="Data">Data {json format}（optional）</param>
        /// <returns></returns>
        public static HttpResponseData Put<T>(string url, Object id, T data)
        {
            return Invoke("PUT", url, id, data);
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="Id">param（optional）</param>
        /// <param name="Data">Data {json format}（optional）</param>
        /// <returns></returns>
        public static HttpResponseData Delete<T>(string url, Object id, T data)
        {
            return Invoke<Object>("DELETE", url, id, data);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Method"></param>
        /// <param name="url"></param>
        /// <param name="Id"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        private static HttpResponseData Invoke<T>(string method, string url, Object id, T data)
        {
            HttpResponseData Response = new HttpResponseData()
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = string.Empty,
                Message = string.Empty,
            };
            try
            {
                string PostParam = string.Empty;
                if (data != null)
                {
                    PostParam = data.ToString();
                }
                byte[] postData = Encoding.UTF8.GetBytes(PostParam);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url + (id == null ? "" : '/' + id.ToString())));
                request.Method = method;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = HttpRequestTimeOut;
                request.ContentType = "application/json";
                request.ContentLength = postData.Length;
                if (postData.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postData, 0, postData.Length);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Response.Code = response.StatusCode;
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        Response.Data = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Message = ex.Message;
            }
            return Response;
        }

        private static HttpResponseData InvokeForOData<T>(string method, string url, Object id, T data)
        {
            HttpResponseData Response = new HttpResponseData()
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = string.Empty,
                Message = string.Empty,
            };
            try
            {
                string PostParam = string.Empty;
                if (data != null)
                {
                    PostParam = data.ToString();
                }
                byte[] postData = Encoding.UTF8.GetBytes(PostParam);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url + (id == null ? "" : '/' + id.ToString())));
                request.Method = method;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = HttpRequestTimeOut;
                request.ContentType = "application/json";
                request.ContentLength = postData.Length;                
                DateTime now = DateTime.UtcNow;
                request.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                request.Headers.Add("x-ms-version", "2019-07-07");
                request.Headers.Add("Accept", "application/json;odata=fullmetadata");
                if (postData.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postData, 0, postData.Length);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Response.Code = response.StatusCode;
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        Response.Data = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Message = ex.Message;
            }
            return Response;
        }
        /// <summary>
        /// Http Response
        /// </summary>
        public class HttpResponseData
        {
            /// <summary>
            /// Http StatusCode
            /// </summary>
            public HttpStatusCode Code { get; set; }
            /// <summary>
            /// Response Data
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// Error Message
            /// </summary>
            public string Message { get; set; }
        }
    }
}