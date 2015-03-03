using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ExportCsvFiletTool.Entity;
using ExportCsvFiletTool.Helper;

namespace ExportCsvFiletTool.BLL
{
    public class Order
    {
        public static string GetOrderByUrl(string strUrl, string user, string pwd)
        {
            string result = string.Empty;
            CookieCollection cookies = new CookieCollection();//如何从response.Headers["Set-Cookie"];中获取并设置CookieCollection的代码略

            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("status", "any");
            //parameters.Add("limit", "1");
            //HttpWebResponse response = HttpWebResponseUtility.CreatePostHttpResponse(strUrl, parameters, 60000, null, Encoding.UTF8, cookies);

            using (HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(strUrl, 180000, null, user, pwd, cookies))
            {
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
                    {
                        result = myStreamReader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static int GetOrderCountByUrl(string strUrl, string user, string pwd)
        {
            string result = string.Empty;
            CookieCollection cookies = new CookieCollection();//如何从response.Headers["Set-Cookie"];中获取并设置CookieCollection的代码略
            using (HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(strUrl.Replace(".json", "/count.json"), 180000, null, user, pwd, cookies))
            {
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
                    {
                        result = myStreamReader.ReadToEnd();
                    }
                }
            }
            return Common.JsonStringToEntity<OrderCountEntity>(result, typeof(OrderCountEntity)).count;
        }
    }
}
