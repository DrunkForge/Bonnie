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

using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;

namespace Bonnie.Hosting.Shared;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpDataModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpAspNetCoreMultiTenancyModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(AbpEventBusRabbitMqModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpEntityFrameworkCorePostgreSqlModule))]
public class BonnieHostingModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpDbContextOptions>(options =>
		{
			options.UseNpgsql();
		});

		Configure<AbpMultiTenancyOptions>(options =>
		{
			options.IsEnabled = true;
		});

		Configure<AbpDbConnectionOptions>(options =>
		{
			options.Databases.Configure("SaaSService", database =>
			{
				database.MappedConnections.Add("AbpTenantManagement");
				database.IsUsedByTenants = false;
			});

			options.Databases.Configure("AdminService", database =>
			{
				database.MappedConnections.Add("AbpAuditLogging");
				database.MappedConnections.Add("AbpPermissionManagement");
				database.MappedConnections.Add("AbpSettingManagement");
				database.MappedConnections.Add("AbpFeatureManagement");
			});

			options.Databases.Configure("AuthService", database =>
			{
				database.MappedConnections.Add("AbpIdentity");
				database.MappedConnections.Add("AbpOpenIddict");
			});
		});

		Configure<AbpLocalizationOptions>(options =>
		{
			options.Languages.Add(new LanguageInfo("en", "en", "English"));
			options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
		});
	}
}
