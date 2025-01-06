using System.Text.Json;
using DiscountCodeSystem.Core.Entity;
using Microsoft.Extensions.Caching.Memory;
using DiscountCodeSystem.Core.Utils;
namespace DiscountCodeSystem.Infrastructure.Service
{
    public class DiscountCodeService
    {
        private const string StorageFilePath = "discount_codes.json";
        private readonly IMemoryCache _cache;
        private readonly Random _random = new Random();
        private readonly HashSet<string> _cacheKeys = new();
        public DiscountCodeService(IMemoryCache cache)
        {
            _cache = cache;
            LoadCodesIntoCache();
        }

        public async Task<(GenerateResponse Response, ValidationError? Error)> GenerateCodesAsync(GenerateRequest request)
        {
            var validationError = Validator.ValidateGenerateRequest(request);
            if (validationError != null)
                return (new GenerateResponse(false), validationError);

            var codesToAdd = new HashSet<string>();

            for (int i = 0; i < request.Count; i++)
            {
                var code = GenerateUniqueCode(request.Length, codesToAdd);
                if (code != null)
                {
                    codesToAdd.Add(code);
                    _cacheKeys.Add(code);
                    _cache.Set(code, false);
                }
            }

            await SaveCodesToStorageAsync();
             return (new GenerateResponse(true, [.. codesToAdd]), null);
        }

        public async Task<UseCodeResponse> UseCodeAsync(string code)
        {
            if (_cache.TryGetValue(code, out var isUsed) && isUsed is false)
            {
                _cache.Set(code, true);
                await SaveCodesToStorageAsync();
                return new UseCodeResponse(0);
            }

            return new UseCodeResponse(1);
        }

        private string? GenerateUniqueCode(int length, HashSet<string> existingCodes)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code;
            int attempts = 0;

            do
            {
                code = new string(Enumerable.Range(0, length).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
                attempts++;
            } while ((_cacheKeys.Contains(code) || existingCodes.Contains(code)) && attempts < 100);

            return attempts < 100 ? code : null;
        }

        private async Task SaveCodesToStorageAsync()
        {
            var codes = _cacheKeys
                .Select(key => new KeyValuePair<string, bool>(key, (bool)_cache.Get(key)!))
                .ToList();

            var json = JsonSerializer.Serialize(codes);
            await File.WriteAllTextAsync(StorageFilePath, json);
        }

        private void LoadCodesIntoCache()
        {
            if (!File.Exists(StorageFilePath)) return;

            var content = File.ReadAllText(StorageFilePath);
            var contentList = JsonSerializer.Deserialize<List<KeyValuePair<string, bool>>>(content);
            var codes = contentList?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (codes != null)
            {
                foreach (var (key, value) in codes)
                {
                    _cacheKeys.Add(key);
                    _cache.Set(key, value);
                }
            }
        }
    }
}