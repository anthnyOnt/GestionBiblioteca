using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GestionBiblioteca.Services.PrestamoDraftCache;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace GestionBibliotecaTests
{
    public class PrestamoDraftCacheTest
    {
    private readonly PrestamoDraftCache _service;
    private readonly InMemoryDistributedCache _store;
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public PrestamoDraftCacheTest()
        {
            _store = new InMemoryDistributedCache();
            _service = new PrestamoDraftCache(_store);
        }

        private class InMemoryDistributedCache : IDistributedCache
        {
            private readonly Dictionary<string, byte[]> _data = new();

            public byte[]? Get(string key)
            {
                return _data.TryGetValue(key, out var v) ? v : null;
            }

            public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            {
                return Task.FromResult(Get(key));
            }

            public void Refresh(string key) {}

            public Task RefreshAsync(string key, CancellationToken token = default)
            {
                return Task.CompletedTask;
            }

            public void Remove(string key) => _data.Remove(key);

            public Task RemoveAsync(string key, CancellationToken token = default)
            {
                Remove(key);
                return Task.CompletedTask;
            }

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            {
                _data[key] = value;
            }

            public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
            {
                Set(key, value, options);
                return Task.CompletedTask;
            }
            
            public string? GetStringValue(string key)
            {
                return _data.TryGetValue(key, out var v) ? System.Text.Encoding.UTF8.GetString(v) : null;
            }
        }

        [Fact]
        public async Task GetAllAsync_Empty_ReturnsEmpty()
        {
            var result = await _service.GetAllAsync(1);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_AddsItem()
        {
            await _service.AddAsync(1, 42);

            var all = await _service.GetAllAsync(1);
            Assert.Single(all);
            Assert.Contains(42, all);
            
            var key = "gbv:draft:1";
            var raw = _store.GetStringValue(key);
            var des = JsonSerializer.Deserialize<List<int>>(raw!, JsonOpts);
            Assert.NotNull(des);
            Assert.Contains(42, des);
        }

        [Fact]
        public async Task AddAsync_NoDuplicates()
        {
            await _service.AddAsync(1, 7);
            await _service.AddAsync(1, 7);

            var all = await _service.GetAllAsync(1);
            Assert.Single(all);
            Assert.Contains(7, all);
        }

        [Fact]
        public async Task RemoveAsync_RemovesItem()
        {
            await _service.AddAsync(2, 1);
            await _service.AddAsync(2, 2);

            await _service.RemoveAsync(2, 1);

            var all = await _service.GetAllAsync(2);
            Assert.Single(all);
            Assert.Contains(2, all);
            Assert.DoesNotContain(1, all);
        }

        [Fact]
        public async Task ClearAsync_RemovesKey()
        {
            await _service.AddAsync(3, 99);

            var key = "gbv:draft:3";
            var rawBefore = _store.GetStringValue(key);
            Assert.NotNull(rawBefore);

            await _service.ClearAsync(3);
            
            var all = await _service.GetAllAsync(3);
            Assert.Empty(all);
            var rawAfter = _store.GetStringValue(key);
            Assert.Null(rawAfter);
        }
    }
}
