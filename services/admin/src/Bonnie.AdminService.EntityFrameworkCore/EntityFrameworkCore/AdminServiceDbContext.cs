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
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Bonnie.AdminService.EntityFrameworkCore;

[ConnectionStringName(AdminDbProperties.ConnectionStringName)]
public class AdminServiceDbContext : AbpDbContext<AdminServiceDbContext>,
	IPermissionManagementDbContext,
	ISettingManagementDbContext,
	IFeatureManagementDbContext,
	IAuditLoggingDbContext,
	IAdminServiceDbContext
{
	/* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

	public AdminServiceDbContext(DbContextOptions<AdminServiceDbContext> options)
		: base(options)
	{
	}

	public DbSet<AuditLog> AuditLogs { get; set; }
	public DbSet<FeatureValue> FeatureValues { get; set; }

	public DbSet<PermissionGrant> PermissionGrants { get; set; }
	public DbSet<Setting> Settings { get; set; }

	public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }

	public DbSet<PermissionDefinitionRecord> Permissions { get; set; }

	public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }

	public DbSet<FeatureDefinitionRecord> Features { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ConfigureAdmin();
		builder.ConfigurePermissionManagement();
		builder.ConfigureSettingManagement();
		builder.ConfigureAuditLogging();
		builder.ConfigureFeatureManagement();
	}
}
