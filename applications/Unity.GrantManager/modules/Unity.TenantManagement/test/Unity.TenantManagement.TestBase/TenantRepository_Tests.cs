﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Xunit;

namespace Unity.TenantManagement;

public abstract class TenantRepository_Tests<TStartupModule> : TenantManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    public ITenantRepository TenantRepository { get; }

    protected TenantRepository_Tests()
    {
        TenantRepository = GetRequiredService<ITenantRepository>();
    }

    [Fact]
    public async Task FindByNameAsync()
    {
        var tenant = await TenantRepository.FindByNameAsync("ACME");
        tenant.ShouldNotBeNull();

        tenant = await TenantRepository.FindByNameAsync("undefined-tenant");
        tenant.ShouldBeNull();

        tenant = await TenantRepository.FindByNameAsync("ACME", includeDetails: true);
        tenant.ShouldNotBeNull();
        tenant.ConnectionStrings.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task FindAsync()
    {
        var tenantId = (await TenantRepository.FindByNameAsync("ACME")).Id;

        var tenant = await TenantRepository.FindAsync(tenantId);
        tenant.ShouldNotBeNull();

        tenant = await TenantRepository.FindAsync(Guid.NewGuid());
        tenant.ShouldBeNull();

        tenant = await TenantRepository.FindAsync(tenantId, includeDetails: true);
        tenant.ShouldNotBeNull();
        tenant.ConnectionStrings.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetListAsync()
    {
        var tenants = await TenantRepository.GetListAsync();
        tenants.ShouldContain(t => t.Name == "acme");
        tenants.ShouldContain(t => t.Name == "volosoft");
    }

    [Fact]
    public async Task Should_Eager_Load_Tenant_Collections()
    {
        var role = await TenantRepository.FindByNameAsync("ACME");
        role.ConnectionStrings.ShouldNotBeNull();
        role.ConnectionStrings.Any().ShouldBeTrue();
    }
}
