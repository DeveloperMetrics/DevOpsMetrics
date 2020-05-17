using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess
{
    public static class MessageUtility
    {
        public async static Task<string> SendAzureDevOpsMessage(string url, string patToken)
        {
            string responseBody = "";
            if (url.IndexOf("dev.azure.com") == -1)
            {
                throw new Exception("dev.azure.com missing from URL");
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //If we use a pat token, we can access private repos
                if (string.IsNullOrEmpty(patToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", patToken))));
                }
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //Throw a response exception
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                    }
                }
            }
            return responseBody;
        }

        public async static Task<string> SendGitHubMessage(string url, string clientId, string clientSecret)
        {
            Console.WriteLine($"Running GitHub url: {url}");
            string responseBody = "";
            if (url.IndexOf("api.github.com") == -1)
            {
                throw new Exception("api.github.com missing from URL");
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DevOpsMetrics", "0.1"));
                //If we use a id/secret, we significantly increase the rate from 60 requests an hour to 5000. https://developer.github.com/v3/#rate-limiting
                if (string.IsNullOrEmpty(clientId) == false && string.IsNullOrEmpty(clientSecret) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", clientId, clientSecret))));
                }
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    //Throw a response exception
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine(responseBody);
                    }
                    //else if (response.ReasonPhrase == "rate limit exceeded")
                    //{
                    //    //This is an error we want to bubble up in a user friendly way (i.e., not a 500)
                    //}
                    //else
                    //{
                    //    //Throw a response exception
                    //    response.EnsureSuccessStatusCode();
                    //}
                }
            }
            return responseBody;
        }
    }
}
