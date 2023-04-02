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

using Bonnie.ExpenseManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Bonnie.ExpenseManagement;

[DependsOn(typeof(ExpensesApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreMvcModule))]
public class ExpensesHttpApiModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		PreConfigure<IMvcBuilder>(mvcBuilder =>
		{
			mvcBuilder.AddApplicationPartIfNotExists(typeof(ExpensesHttpApiModule).Assembly);
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Configure<AbpLocalizationOptions>(options =>
		{
			options.Resources
				.Get<ExpensesResource>()
				.AddBaseTypes(typeof(AbpUiResource));
		});
	}
}
