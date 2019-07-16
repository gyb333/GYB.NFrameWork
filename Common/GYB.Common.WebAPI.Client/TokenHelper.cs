using GYB.Common.Polly;
using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GYB.Common.WebAPI.Client
{
    public class TokenHelper
    {
        private static readonly SemaphoreSlim limitLock = new SemaphoreSlim(1);

        public static async Task<string> getTokenAsync(string strURL, string strContent, string mediaType, int RetryCount = 3)
        {
            string strToken = string.Empty;
            await limitLock.WaitAsync().ConfigureAwait(false);
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new StringContent(strContent, Encoding.UTF8, mediaType))
                    {
                        await PolicyHelper.RetryExecAsync<TokenException>(async () =>
                        {
                            try
                            {
                                var response = await client.PostAsync(strURL, content);
                                response.EnsureSuccessStatusCode();
                                var responseStr = await response.Content.ReadAsStringAsync();
                                if (responseStr.Contains("access_token"))
                                {
                                    var temp = JsonConvert.DeserializeObject<dynamic>(responseStr);
                                    strToken = temp.access_token;
                                }
                                else
                                {
                                    throw new TokenException();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new TokenException(ex.Message,ex);
                            }
                        });
                    }
                }
            }
            catch (TokenException ex)
            {
                Console.WriteLine($"Excepiton:{ex.Message}");
            }
            finally
            {
                limitLock.Release();
            }
            return strToken;
        }

        public static async Task<string> getClientTokenAsync(string strURL, string client_id, string client_secret, string mediaType = "application/x-www-form-urlencoded")
        {
            var strContent = $"client_id={client_id}&client_secret={client_secret}&grant_type=client_credentials";
            return await getTokenAsync(strURL, strContent, mediaType);
        }

        public static async Task<string> getRefreshTokenAsync(string strURL, string client_id, string client_secret, string oldToken, string mediaType = "application/x-www-form-urlencoded")
        {
            var strContent = $"client_id={client_id}&client_secret={client_secret}&grant_type=refresh_token&refresh_token={oldToken}";
            return await getTokenAsync(strURL, strContent, mediaType);
        }
    }
}
