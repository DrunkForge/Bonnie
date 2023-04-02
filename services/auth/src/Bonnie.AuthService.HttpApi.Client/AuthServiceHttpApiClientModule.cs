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
using Volo.Abp.Account;
using Volo.Abp.Http.Client;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bonnie.AuthService;

[DependsOn(typeof(AuthServiceApplicationContractsModule))]
[DependsOn(typeof(AbpHttpClientModule))]
[DependsOn(typeof(AbpIdentityHttpApiClientModule))]
[DependsOn(typeof(AbpAccountHttpApiClientModule))]
public class AuthServiceHttpApiClientModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddHttpClientProxies(
			typeof(AuthServiceApplicationContractsModule).Assembly,
			AuthServiceRemoteServiceConsts.RemoteServiceName
		);

		Configure<AbpVirtualFileSystemOptions>(options =>
		{
			options.FileSets.AddEmbedded<AuthServiceHttpApiClientModule>();
		});
	}
}
