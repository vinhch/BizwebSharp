using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Helper")]
    public class Helper_Tests
    {
        [Fact(DisplayName = "Utils Upload File")]
        public async Task Upload_Themes()
        {
            var response = await Utils.UploadToFileIoAsync("theme_bizweb.zip");

            Assert.True(response.Success);
            Assert.Equal("1 day", response.Expiry);
            Assert.NotEmpty(response.Key);
            Assert.NotEmpty(response.Link);
        }
    }
}
