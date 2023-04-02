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

using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Bonnie.Blazor.Menus;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenIddict.Abstractions;
using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.WebAssembly.BasicTheme;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Blazor.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;
using Volo.Abp.TenantManagement.Blazor.WebAssembly;
using Volo.Abp.UI.Navigation;

namespace Bonnie.Blazor;

[DependsOn(typeof(AbpAutofacWebAssemblyModule))]
[DependsOn(typeof(AbpAspNetCoreComponentsWebAssemblyBasicThemeModule))]
[DependsOn(typeof(AbpIdentityBlazorWebAssemblyModule))]
[DependsOn(typeof(AbpTenantManagementBlazorWebAssemblyModule))]
[DependsOn(typeof(AbpSettingManagementBlazorWebAssemblyModule))]
public class BonnieBlazorModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
		var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

		ConfigureAuthentication(builder);
		ConfigureHttpClient(context, environment);
		ConfigureBlazorise(context);
		ConfigureRouter(context);
		ConfigureUI(builder);
		ConfigureMenu(context);
		ConfigureAutoMapper(context);
	}

	private void ConfigureRouter(ServiceConfigurationContext context)
	{
		Configure<AbpRouterOptions>(options =>
		{
			options.AppAssembly = typeof(BonnieBlazorModule).Assembly;
		});
	}

	private void ConfigureMenu(ServiceConfigurationContext context)
	{
		Configure<AbpNavigationOptions>(options =>
		{
			options.MenuContributors.Add(new BonnieMenuContributor(context.Services.GetConfiguration()));
		});
	}

	private void ConfigureBlazorise(ServiceConfigurationContext context)
	{
		context.Services
			.AddBootstrap5Providers()
			.AddFontAwesomeIcons();
	}

	private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
	{
		builder.Services.AddOidcAuthentication(options =>
		{
			builder.Configuration.Bind("AuthServer", options.ProviderOptions);
			options.UserOptions.NameClaim = OpenIddictConstants.Claims.Name;
			options.UserOptions.RoleClaim = OpenIddictConstants.Claims.Role;
			options.ProviderOptions.DefaultScopes.Add("openid");
			options.ProviderOptions.DefaultScopes.Add("profile");
			options.ProviderOptions.DefaultScopes.Add("roles");
			options.ProviderOptions.DefaultScopes.Add("email");
			options.ProviderOptions.DefaultScopes.Add("phone");
			options.ProviderOptions.DefaultScopes.Add("SaasService");
			options.ProviderOptions.DefaultScopes.Add("AuthService");
			options.ProviderOptions.DefaultScopes.Add("AdminService");
		});
	}

	private static void ConfigureUI(WebAssemblyHostBuilder builder)
	{
		builder.RootComponents.Add<App>("#ApplicationContainer");

	}

	private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
	{
		context.Services.AddTransient(sp => new HttpClient
		{
			BaseAddress = new Uri(environment.BaseAddress)
		});
	}

	private void ConfigureAutoMapper(ServiceConfigurationContext context)
	{
		Configure<AbpAutoMapperOptions>(options =>
		{
			options.AddMaps<BonnieBlazorModule>();
		});
	}
}
