namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    public static class ClientNotifier
    {
        public static void NotificationRequest(string callbackUrl, string data)
        {
            var uri = new Uri(callbackUrl);

            // var req = (HttpWebRequest)WebRequest.Create(uri);
            // req.KeepAlive = true;
            // req.Method = "POST";
            // using (var reqStream = req.GetRequestStream())
            // {
            // if (data != null)
            // {
            // var reqBytes = Encoding.UTF8.GetBytes(data);
            // reqStream.Write(reqBytes, 0, reqBytes.Length);
            // }
            // }
            //  req.GetResponse();
            var c = new HttpClient
                {
                    BaseAddress = uri
                };
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress")
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };
            c.SendAsync(request);
        }
    }
}
