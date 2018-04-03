using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Feedler.Extensions;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace Feedler.Tests.Extensions
{
    public static class HttpAssert
    {
        // Makes assertion but provides HTTP response if it fails.
        private static async Task AssertAsync(HttpResponseMessage response, Func<bool> assertion, string message)
        {
            if (!assertion())
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new XunitException($"{message}\n" +
                                             $"Response: {(int)response.StatusCode} {response.StatusCode} \"{content}\"");
            }
        }

        public static async Task IsErrorResponseAsync(HttpResponseMessage response, HttpStatusCode httpStatusCode, string errorCode = null)
        {
            await AssertAsync(response, () => response.StatusCode == httpStatusCode, $"Response should have status code {httpStatusCode}.");

            var responseJson = await response.Content.ReadAsAsync<JObject>();
            if (errorCode != null)
            {
                var responseError = responseJson.String("Code");
                await AssertAsync(response, () => responseError == errorCode, $"Response should have error code \"{errorCode}\".");
            }
        }

        public static Task IsSuccessResponseAsync(HttpResponseMessage response)
        {
            return AssertAsync(response, () => response.IsSuccessStatusCode, "Response should have success status code.");
        }
    }
}