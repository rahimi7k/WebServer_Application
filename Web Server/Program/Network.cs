﻿using Newtonsoft.Json;
using Program.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Program.IONet {

	public class Network {

		private static readonly int TIMEOUT = 5000;

		private static readonly NetworkHttpClient httpClient = new NetworkHttpClient();

		public static bool IsConnected() {
			return true;
		}

		public static string RequestGet(string url, Dictionary<string, string> param) {
			return RequestGet(url, param, (Dictionary<string, string>) null);
		}

		public static string RequestGet(string url, Dictionary<string, string> param, Dictionary<string, string> headers) {
			string response = null;
			try {
				HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url + ParamsConvert(param, true));
				webRequest.Method = HttpMethod.Get.Method;
				webRequest.Timeout = TIMEOUT;
				webRequest.AllowAutoRedirect = true;
				webRequest.MaximumAutomaticRedirections = 10;
				webRequest.UserAgent = "";
				if (headers != null) {
					foreach (KeyValuePair<string, string> header in headers) {
						webRequest.Headers.Add(header.Key, header.Value);
					}
				}
				WebResponse webResponse = webRequest.GetResponse();
				Stream Stream = webResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(Stream);
				response = streamReader.ReadToEnd();
				streamReader.Dispose();
				streamReader.Close();
				Stream.Dispose();
				Stream.Close();
				webResponse.Dispose();
				webResponse.Close();
			} catch (Exception ex) {
				Log.E(ex);
			}
			return response;
		}

		public static void RequestGet(string url, Dictionary<string, string> param, IRequest iRequest) {
			RequestGet(url, param, null, iRequest);
		}

		public static void RequestGet(string url, Dictionary<string, string> param, Dictionary<string, string> headers, IRequest iRequest) {
			new Thread(delegate() {
				string response = RequestGet(url, param, headers);
				if (iRequest != null) {
					iRequest.OnResult(response);
				}
			}).Start();
		}

		public static async Task<string> RequestGetAsync(string url, Dictionary<string, string> param) {
			return await RequestGetAsync(url, param, null);
		}

		public static async Task<string> RequestGetAsync(string url, Dictionary<string, string> param, Dictionary<string, string> headers) {
			string response = null;

			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url + ParamsConvert(param, true));
			if (headers != null) {
				foreach (KeyValuePair<string, string> header in headers) {
					requestMessage.Headers.Add(header.Key, header.Value);
				}
			}
			try {
				HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
				response = await httpResponse.Content.ReadAsStringAsync();
			} catch (Exception ex) {
				Log.E(ex);
			}
			requestMessage.Dispose();
			return response;
		}

		public static string RequestPost(string url, Dictionary<string, string> param) {
			return RequestPost(url, param, (Dictionary<string, string>) null);
		}

		public static string RequestPost(string url, Dictionary<string, string> param, Dictionary<string, string> headers) {
			string response = null;
			try {
				HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
				webRequest.Method = HttpMethod.Post.Method;
				webRequest.Timeout = TIMEOUT;
				webRequest.AllowAutoRedirect = true;
				webRequest.MaximumAutomaticRedirections = 10;
				webRequest.UserAgent = "";
				//webRequest.ContentLength = byteArray.Length;//-- Stream Add!
				webRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
				if (headers != null) {
					foreach (KeyValuePair<string, string> header in headers) {
						webRequest.Headers.Add(header.Key, header.Value);
					}
				}
				byte[] byteArray = Encoding.UTF8.GetBytes(ParamsConvert(param, false));
				Stream stream = webRequest.GetRequestStream();
				stream.Write(byteArray, 0, byteArray.Length);

				WebResponse webResponse = webRequest.GetResponse();
				stream = webResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(stream);
				response = streamReader.ReadToEnd();
				streamReader.Dispose();
				streamReader.Close();
				stream.Dispose();
				stream.Close();
				webResponse.Dispose();
				webResponse.Close();
			} catch (Exception ex) {
				Log.E(ex);
			}
			return response;
		}

		public static void RequestPost(string url, Dictionary<string, string> param, IRequest iRequest) {
			RequestPost(url, param, null, iRequest);
		}

		public static void RequestPost(string url, Dictionary<string, string> param, Dictionary<string, string> headers, IRequest iRequest) {
			new Thread(delegate () {
				string response = RequestPost(url, param, headers);
				if (iRequest != null) {
					iRequest.OnResult(response);
				}
			}).Start();
		}

		public static async Task<string> RequestPostAsync(string url, Dictionary<string, string> param) {
			return await RequestPostAsync(url, param, null);
		}

		public static async Task<string> RequestPostAsync(string url, Dictionary<string, string> param, Dictionary<string, string> headers) {
			string response = null;

			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
			if (param != null) {
				requestMessage.Content = new FormUrlEncodedContent(param);
				requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
				requestMessage.Content.Headers.ContentType.CharSet = "utf-8";
			}
			if (headers != null) {
				foreach (KeyValuePair<string, string> header in headers) {
					requestMessage.Headers.Add(header.Key, header.Value);
				}
			}
			try {
				HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
				response = await httpResponse.Content.ReadAsStringAsync();
			} catch (Exception ex) {
				Log.E(ex);
			}
			requestMessage.Dispose();
			return response;
		}

		public static async Task<string> RequestPostAsync(string url, JSONObject json) {
			return await RequestPostAsync(url, json, null);
		}

		public static async Task<string> RequestPostAsync(string url, JSONObject json, Dictionary<string, string> headers) {
			string response = null;

			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
			requestMessage.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			if (headers != null) {
				foreach (KeyValuePair<string, string> header in headers) {
					requestMessage.Headers.Add(header.Key, header.Value);
				}
			}
			try {
				HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
				response = await httpResponse.Content.ReadAsStringAsync();
			} catch (Exception) { }
			requestMessage.Dispose();
			return response;
		}

		public static async Task<byte[]> RequestPostAsync(string url, byte[] bytes) {
			return await RequestPostAsync(url, bytes, null);
		}

		public static async Task<byte[]> RequestPostAsync(string url, byte[] bytes, Dictionary<string, string> headers) {
			byte[] response = null;

			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
			requestMessage.Content = new ByteArrayContent(bytes);
			requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			if (headers != null) {
				foreach (KeyValuePair<string, string> header in headers) {
					requestMessage.Headers.Add(header.Key, header.Value);
				}
			}
			try {
				HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);
				response = await httpResponse.Content.ReadAsByteArrayAsync();
			} catch (Exception) { }
			requestMessage.Dispose();
			return response;
		}

		public static string ParamsConvert(Dictionary<string, string> param, bool appendQuestionMark) {
			if (param == null || param.Count == 0) {
				return "";
			}
			string str = "";
			foreach (KeyValuePair<string, string> item in param) {
				try {
					str += Uri.EscapeDataString(item.Key) + '=' + Uri.EscapeDataString(item.Value) + '&';
				} catch (Exception) { }
			}
			str = str.TrimEnd('&');
			return str.Length == 0 ? "" : (appendQuestionMark ? '?' + str : str);
		}

		private class NetworkHttpClient : HttpClient {

			public NetworkHttpClient() {
				Timeout = TimeSpan.FromMilliseconds(TIMEOUT);
			}
		}

		public interface IRequest {
			void OnResult(string response);
		}
	}
}
