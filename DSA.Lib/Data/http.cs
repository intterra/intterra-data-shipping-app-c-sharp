﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSA.Lib.Data
{
    public static class Http
    {
        /// <summary>
        /// Makes Post reqest using WebClient
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string Post(string uri, NameValueCollection pairs)
        {
            pairs = pairs ?? new NameValueCollection();
            string response = null;
            using (WebClient client = new WebClient())
            {
                var bytes = client.UploadValues(uri, pairs);
                response = Encoding.UTF8.GetString(bytes);
            }
            return response;
        }

        /// <summary>
        /// Makes Get request using WebClient
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string Get(string uri, NameValueCollection pairs)
        {
            pairs = pairs ?? new NameValueCollection();
            string query = "";
            for (var i = 0; i < pairs.AllKeys.Length; i++)
            {
                query += string.IsNullOrWhiteSpace(query) ? "?" : "&";
                query += pairs.GetKey(i);
                query += "=";
                var value = pairs.GetValues(i)?.FirstOrDefault();
                query += !string.IsNullOrWhiteSpace(value) ? value : "";
            }

            string response = null;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(uri + query);
            }
            return response;
        }
    }
}
