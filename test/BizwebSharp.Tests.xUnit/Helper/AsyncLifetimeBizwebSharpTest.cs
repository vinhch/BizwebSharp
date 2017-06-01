using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Services;
using Xunit;

namespace BizwebSharp.Tests.xUnit
{
    public abstract class AsyncLifetimeBizwebSharpTest<TService, TEntity> : IAsyncLifetime
        where TService : BaseService
        where TEntity : BaseEntity, new()
    {
        public TService Service => (TService) Activator.CreateInstance(typeof(TService), Utils.AuthState);

        public List<TEntity> Created { get; } = new List<TEntity>();

        public async Task InitializeAsync()
        {
            // Create one for count, list, get, etc. orders.
            await Create();
        }

        public abstract Task DisposeAsync();

        public abstract Task<TEntity> Create(bool skipAddToCreatedList = false);
    }
}
