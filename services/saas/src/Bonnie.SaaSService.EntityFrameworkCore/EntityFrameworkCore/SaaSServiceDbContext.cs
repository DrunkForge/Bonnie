// Bonnie Shared Finances Management
// Copyright (C) 2023 Drunk Forge Software
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Bonnie.SaaSService.EntityFrameworkCore;

[ConnectionStringName(SaaSDbProperties.ConnectionStringName)]
public class SaaSServiceDbContext : AbpDbContext<SaaSServiceDbContext>, ITenantManagementDbContext, ISaaSServiceDbContext
{
	/* Add DbSet for each Aggregate Root here. Example:
	 * public DbSet<Question> Questions { get; set; }
	 */

	public SaaSServiceDbContext(DbContextOptions<SaaSServiceDbContext> options)
		: base(options)
	{
	}

	public DbSet<Tenant> Tenants { get; set; }

	public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ConfigureSaaS();
		builder.ConfigureTenantManagement();
	}
}
