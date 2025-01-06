using DiscountCodeSystem.Core.Entity;
using DiscountCodeSystem.Infrastructure.Service;
using Microsoft.Extensions.Caching.Memory;

namespace DiscountCodeSystem.Tests
{
    public class DiscountCodeServiceTests
    {
        private readonly DiscountCodeService _service;
        private readonly IMemoryCache _memoryCache;
        public DiscountCodeServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new DiscountCodeService(_memoryCache);
        }

        [Fact]
        public async Task GenerateCodes_ShouldCreateCorrectNumberOfCodes()
        {
            const ushort count = 10;
            const byte length = 8;

            var (response, error) = await _service.GenerateCodesAsync(new GenerateRequest(count, length));

            Assert.Null(error);
            Assert.True(response.Result);
            Assert.NotNull(response.Codes);
            Assert.Equal(count, response.Codes.Count);
        }

        [Fact]
        public async Task GenerateCodes_ShouldFailForInvalidLength()
        {
            const ushort count = 10;
            const byte length = 6;

            var (response, error) = await _service.GenerateCodesAsync(new GenerateRequest(count, length));

            Assert.NotNull(error);
            Assert.Equal("Length", error.Field);
            Assert.False(response.Result);
        }

        [Fact]
        public async Task GenerateCodes_ShouldFailForExcessiveCount()
        {
            const ushort count = 2500;
            const byte length = 8;

            var (response, error) = await _service.GenerateCodesAsync(new GenerateRequest(count, length));

            Assert.NotNull(error);
            Assert.Equal("Count", error.Field);
            Assert.False(response.Result);
        }


        [Fact]
        public void UseCode_ShouldFailForAlreadyUsedCode()
        {
            const string code = "TESTCODE";
            _service.GenerateCodesAsync(new GenerateRequest(1, (byte)code.Length)).Wait();
           _service.UseCodeAsync(code).Wait();

            var response = _service.UseCodeAsync(code).Result;

            Assert.Equal(1, response.Result);
        }

        [Fact]
        public void UseCode_ShouldFailForNonexistentCode()
        {
            const string code = "NONEXISTENT";

          var response = _service.UseCodeAsync(code).Result;

            Assert.Equal(1, response.Result);
        }

        [Fact]
        public async Task GenerateCodes_ShouldProduceUniqueCodes()
        {
            const ushort count = 100;
            const byte length = 7;

            var (response, error) = await _service.GenerateCodesAsync(new GenerateRequest(count, length));

            Assert.Null(error);
            Assert.True(response.Result);
            Assert.NotNull(response.Codes);
            Assert.Equal(count, response.Codes.Count);
            Assert.Equal(count, response.Codes.Distinct().Count());
        }

        [Fact]
        public async Task GenerateCodes_ShouldSupportParallelRequests()
        {
            const ushort countPerRequest = 500;
            const byte length = 7;

            var tasks = Enumerable.Range(0, 4).Select(_ => _service.GenerateCodesAsync(new GenerateRequest(countPerRequest, length))).ToArray();
            var results = await Task.WhenAll(tasks);

            Assert.All(results, result =>
            {
                Assert.Null(result.Error);
                Assert.True(result.Response.Result);
                Assert.NotNull(result.Response.Codes);
                Assert.Equal(countPerRequest, result.Response.Codes.Count);
            });
        }
    }
}