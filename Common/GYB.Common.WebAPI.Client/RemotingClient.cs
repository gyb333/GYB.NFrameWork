using GYB.Common.Polly;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GYB.Common.WebAPI.Client
{
    public class RemotingClient
    {


         

        public string Token { get; private set; }

        private string strTokenURL;
        private string strClient_id;
        private string strClient_secret;

        public RemotingClient(string tokenURL, string client_id, string client_secret)
        {
            strTokenURL = tokenURL;
            strClient_id = client_id;
            strClient_secret = client_secret;
            setTokenAsync().Wait();
        }



        public async Task setTokenAsync()
        {
            Token = await TokenHelper.getClientTokenAsync(strTokenURL, strClient_id, strClient_secret);
        }
        public async Task setRefreshTokenAsync()
        {
            Token = await TokenHelper.getRefreshTokenAsync(strTokenURL, strClient_id, strClient_secret, Token);
        }






        private Dictionary<string, string> CreateRequestParam(int pageNo, int pageSize, Dictionary<string, string> queryParam)
        {
            var reqParam = new Dictionary<string, string>
            {
                {"access_token", Token},
                {"output", "10"},
                {"pageNo", pageNo.ToString()},
                {"pageSize", pageSize.ToString()}
            };
            if (queryParam != null && queryParam.Count > 0)
                foreach (var q in queryParam)
                    reqParam.Add(q.Key, q.Value);

            return reqParam;
        }



        public async Task<string> SendRequest(HttpClient httpClient, string strURL, Dictionary<string, string> queryParam, int RetryCount = 3)
        {
            string result = string.Empty;
            try
            {
                await PolicyHelper.RetryExecAsync<TokenException>(async () =>
                {
                    result = await SendRequest(httpClient, strURL, queryParam);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepiton:{ex.Message}");
            }
            return result;
        }
        public async Task<string> SendRequest(HttpClient httpClient, string strURL, Dictionary<string, string> queryParam)
        {
            string result = string.Empty;
            if (queryParam != null && queryParam.Count > 0)
            {
                queryParam["access_token"] = Token;
            }
            using (var urlContent = new FormUrlEncodedContent(queryParam))
            {
                var response = await httpClient.PostAsync(strURL, urlContent);
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        public async Task<string> SendRequest(string strURL, Dictionary<string, string> queryParam)
        {
            string result = string.Empty;
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(2);

                result = await SendRequest(httpClient, strURL, queryParam);

                if (result.Contains("access_token_invalid"))
                {
                    await setTokenAsync();
                }
                else if (result.Contains("access_token_expired"))
                {
                    await setRefreshTokenAsync();
                }
                //重新请求
                result = await SendRequest(httpClient, strURL, queryParam);

                if (string.IsNullOrWhiteSpace(result))
                    throw new RemoteApiException(strURL,
                        $"远程获取数据失败,RequestParam:[{queryParam.ConvertToString()}]");
                return result;
            }
        }



    }
}
