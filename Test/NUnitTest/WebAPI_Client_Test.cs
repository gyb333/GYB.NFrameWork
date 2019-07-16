
using GYB.Common.WebAPI.Client;
using NUnit.Framework;
using System;

namespace Tests
{
    public class WebAPI_Client_Test
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestToken()
        {
            string strTokenURL = "http://uat-wilmarpoc.windms.com:8001/open/api/v1/auth/oauth2/access_token";
            string client_id = "4947B72E1DA1A8E482605797EFE75AC9";
            string client_secret = "5A9FBE05CAA5CFEE3CD2A79833A1384AE873BDD1";
            var client= RemotingClientFactory.GetInstance(strTokenURL, client_id, client_secret).Result;
            Assert.Pass();
        }
    }
}