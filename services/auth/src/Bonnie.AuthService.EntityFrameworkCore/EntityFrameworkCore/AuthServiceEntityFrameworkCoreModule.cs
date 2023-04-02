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
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace Bonnie.AuthService.EntityFrameworkCore;

[DependsOn(typeof(AuthServiceDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpIdentityEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpOpenIddictEntityFrameworkCoreModule))]
public class AuthServiceEntityFrameworkCoreModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpDbContextOptions>(options =>
		{
			options.UseNpgsql();
		});
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		context.Services.AddAbpDbContext<AuthServiceDbContext>(options =>
		{
			options.ReplaceDbContext<IIdentityDbContext>();
			options.ReplaceDbContext<IOpenIddictDbContext>();

			options.AddDefaultRepositories(true);
		});
	}
}
