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

using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Bonnie.AdminService.EntityFrameworkCore;

[DependsOn(typeof(AdminDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpFeatureManagementEntityFrameworkCoreModule))]
public class AdminServiceEntityFrameworkCoreModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpDbContextOptions>(options =>
		{
			options.UseNpgsql();
		});
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		context.Services.AddAbpDbContext<AdminServiceDbContext>(options =>
		{
			/* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */

			options.AddDefaultRepositories(true);
		});

		context.Services.AddAbpDbContext<AdminServiceDbContext>(options =>
		{
			options.ReplaceDbContext<IPermissionManagementDbContext>();
			options.ReplaceDbContext<ISettingManagementDbContext>();
			options.ReplaceDbContext<IFeatureManagementDbContext>();
			options.ReplaceDbContext<IAuditLoggingDbContext>();

			options.AddDefaultRepositories(true);
		});
	}
}
