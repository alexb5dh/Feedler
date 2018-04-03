using System.Net;
using System.Threading.Tasks;
using Feedler.Tests.Extensions;
using Xunit;

namespace Feedler.Tests
{
    /// <summary>
    /// Tests to ensure Swagger schema is not broken and UI is working as intended.
    /// </summary>
    public class SwaggerTests: FeedlerTestHost
    {
        [TestFor("Swagger")]
        public async Task Swagger_ShouldHaveUI()
        {
            var response = await Client.GetAsync("swagger/");
            Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        }

        [TestFor("Swagger")]
        public async Task Swagger_ShouldHaveSchema()
        {
            var response = await Client.GetAsync("swagger/v1/swagger.json");
            Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        }
    }
}
