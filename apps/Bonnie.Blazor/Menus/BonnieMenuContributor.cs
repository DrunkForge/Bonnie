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

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace Bonnie.Blazor.Menus;

public class BonnieMenuContributor : IMenuContributor
{
	private readonly IConfiguration _configuration;

	public BonnieMenuContributor(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task ConfigureMenuAsync(MenuConfigurationContext context)
	{
		if (context.Menu.Name == StandardMenus.Main)
		{
			await ConfigureMainMenuAsync(context);
		}
		else if (context.Menu.Name == StandardMenus.User)
		{
			await ConfigureUserMenuAsync(context);
		}
	}

	private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
	{

		context.Menu.Items.Insert(
			0,
			new ApplicationMenuItem(
				BonnieMenus.Home,
				"Home",
				"/",
				icon: "fas fa-home"
			)
		);

		var admin = context.Menu.GetAdministration();
		Console.WriteLine(admin);
		admin.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);

		admin.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
		admin.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

		return Task.CompletedTask;
	}

	private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
	{

		var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

		context.Menu.AddItem(new ApplicationMenuItem(
			"Account.Manage",
			"Manage Your Profile",
			$"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={_configuration["App:SelfUrl"]}",
			icon: "fa fa-cog",
			order: 1000,
			null).RequireAuthenticated());

		return Task.CompletedTask;
	}
}
