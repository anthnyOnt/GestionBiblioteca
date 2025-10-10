using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GestionBiblioteca.Services.PrestamoDraftCache
{
    public class PrestamoDraftCache : IPrestamoDraftCache
    {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public PrestamoDraftCache(IDistributedCache cache) { _cache = cache; }

        private static string Key(int usuarioId) => $"gbv:draft:{usuarioId}";

        public async Task AddAsync(int usuarioId, int ejemplarId)
        {
            var list = (await GetAllAsync(usuarioId)).ToList();
            if (!list.Contains(ejemplarId)) list.Add(ejemplarId);
            await Save(usuarioId, list);
        }

        public async Task RemoveAsync(int usuarioId, int ejemplarId)
        {
            var list = (await GetAllAsync(usuarioId)).Where(x => x != ejemplarId).ToList();
            await Save(usuarioId, list);
        }

        public async Task<IReadOnlyList<int>> GetAllAsync(int usuarioId)
        {
            var raw = await _cache.GetStringAsync(Key(usuarioId));
            if (string.IsNullOrEmpty(raw)) return Array.Empty<int>();
            return JsonSerializer.Deserialize<List<int>>(raw, JsonOpts) ?? new List<int>();
        }

        public Task ClearAsync(int usuarioId) => _cache.RemoveAsync(Key(usuarioId));

        private Task Save(int usuarioId, List<int> list)
        {
            var raw = JsonSerializer.Serialize(list, JsonOpts);
            var opt = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            return _cache.SetStringAsync(Key(usuarioId), raw, opt);
        }
    }
}