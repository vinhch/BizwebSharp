﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    [Trait("Category", "Asset")]
    public class Asset_Tests : IClassFixture<Asset_Tests_Fixture>
    {
        private Asset_Tests_Fixture Fixture { get; }

        public Asset_Tests(Asset_Tests_Fixture fixture)
        {
            Fixture = fixture;
        }

        [Fact(DisplayName = "Create Assets")]
        public async Task Creates_Assets()
        {
            const string key = "snippets/test.bwt";
            var created = await Fixture.Create(key);

            Assert.NotNull(created);
            Assert.Equal(key, created.Key);
            Assert.Equal(Fixture.ThemeId, created.ThemeId);

            // Value is not returned when creating or updating. Must get the asset to check it.
            var asset = await Fixture.Service.GetAsync(Fixture.ThemeId, key);

            Assert.Equal(Fixture.AssetValue, asset.Value);
        }

        [Fact(DisplayName = "Update Assets")]
        public async Task Updates_Assets()
        {
            const string key = "snippets/update-test.bwt";
            const string newValue = "<h1>Hello, world! I've been updated!</h1>";
            var created = await Fixture.Create(key);
            created.Value = newValue;

            await Fixture.Service.CreateOrUpdateAsync(Fixture.ThemeId, created);

            // Value is not returned when creating or updating. Must get the asset to check it.
            var updated = await Fixture.Service.GetAsync(Fixture.ThemeId, key);

            Assert.Equal(newValue, updated.Value);
        }

        [Fact(DisplayName = "Get Assets")]
        public async Task Gets_Assets()
        {
            var key = Fixture.Created.First().Key;
            var asset = await Fixture.Service.GetAsync(Fixture.ThemeId, key);

            Assert.NotNull(asset);
            Assert.Equal(asset.Key, key);
            Assert.Equal(asset.ThemeId, Fixture.ThemeId);
        }

        [Fact(DisplayName = "Copie Assets")]
        public async Task Copies_Assets()
        {
            const string key = "snippets/copy-test.bwt";
            var original = await Fixture.Create("snippets/copy-original-test.bwt");
            var asset = await Fixture.Service.CreateOrUpdateAsync(Fixture.ThemeId, new Asset()
            {
                Key = key,
                SourceKey = original.Key,
            });

            Assert.NotNull(asset);
            Assert.Equal(asset.Key, key);
            Assert.Equal(asset.Value, original.Value);
            Assert.Equal(asset.ContentType, original.ContentType);
            Assert.Equal(asset.ThemeId, Fixture.ThemeId);
        }

        [Fact(DisplayName = "List Assets")]
        public async Task Lists_Assets()
        {
            var list = await Fixture.Service.ListAsync(Fixture.ThemeId);

            Assert.True(list.Count() > 0);
        }

        [Fact(DisplayName = "Delete Assets")]
        public async Task Deletes_Assets()
        {
            var threw = false;
            const string key = "snippets/delete-test.bwt";
            var created = await Fixture.Create(key, true);

            try
            {
                await Fixture.Service.DeleteAsync(Fixture.ThemeId, key);
            }
            catch (BizwebSharpException ex)
            {
                Console.WriteLine($"{nameof(Deletes_Assets)} threw exception. {ex.Message}");

                threw = true;
            }

            Assert.False(threw);
        }
    }

    public class Asset_Tests_Fixture : IAsyncLifetime
    {
        public AssetService Service => new AssetService(Utils.AuthState);

        public List<Asset> Created { get; } = new List<Asset>();

        public string AssetValue => "<h1>Hello world!</h1>";

        public long ThemeId { get; set; }

        public async Task InitializeAsync()
        {
            var themeService = new ThemeService(Utils.AuthState);
            var themes = await themeService.ListAsync();

            ThemeId = themes.First().Id.Value;
        }

        public async Task DisposeAsync()
        {
            foreach (var asset in Created)
            {
                try
                {
                    await Service.DeleteAsync(ThemeId, asset.Key);
                }
                catch (BizwebSharpException ex)
                {
                    if (ex.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Failed to delete created Asset with key {asset.Key}. {ex.Message}");
                    }
                }
            }
        }

        public async Task<Asset> Create(string key, bool skipAddToCreatedList = false)
        {
            var asset = await Service.CreateOrUpdateAsync(ThemeId, new Asset
            {
                ContentType = "text/x-liquid",
                Value = AssetValue,
                Key = key
            });

            if (!skipAddToCreatedList)
            {
                Created.Add(asset);
            }

            return asset;
        }
    }
}