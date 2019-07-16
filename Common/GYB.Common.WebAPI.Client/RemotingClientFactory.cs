using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GYB.Common.WebAPI.Client
{
    public static class RemotingClientFactory
    {
        private static readonly ConcurrentDictionary<string, RemotingClient> cDict = new ConcurrentDictionary<string, RemotingClient>();
        private static readonly SemaphoreSlim limitLock = new SemaphoreSlim(1);


        public async static Task<RemotingClient> GetInstance(string tokenURL, string client_id, string client_secret)
        {
            RemotingClient rc;
            cDict.TryGetValue(tokenURL, out rc);
            if (rc == null)
            {
                await limitLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    cDict.TryGetValue(tokenURL, out rc);
                    if (rc == null)
                    {
                        rc = new RemotingClient(tokenURL, client_id, client_secret);
                        cDict[tokenURL] = rc;
                    }
                }
                finally
                {
                    limitLock.Release();
                }
            }
            return rc;
        }

    }
}
