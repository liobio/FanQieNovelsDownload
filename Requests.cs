using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace FanQieNovelsDownload
{
    public class Requests
    {
        HttpClient client;
        public Requests()
        {
            var socketsHttpHandler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,// 默认为true,是否允许重定向
                MaxAutomaticRedirections = 50,//最多重定向几次,默认50次
                MaxConnectionsPerServer = 100,//连接池中统一TcpServer的最大连接数
                UseCookies = false,// 是否自动处理cookie

            };
            Console.WriteLine(socketsHttpHandler.ResponseHeaderEncodingSelector);
            client = new HttpClient(socketsHttpHandler);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml");
        }
        public HttpResponseMessage Get(string str)
        {
            HttpResponseMessage response = client.GetAsync(str).Result;
            return response;
        }


    }
    public static class RequestsExtension
    {
        //静态方法
        public static string Text(this HttpResponseMessage r)  //this关键字
        {

            return r.Content.ReadAsStringAsync().Result;
        }
    }
}
