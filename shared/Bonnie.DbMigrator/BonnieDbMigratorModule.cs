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

using Bonnie.AdminService;
using Bonnie.AdminService.EntityFrameworkCore;
using Bonnie.AuthService;
using Bonnie.AuthService.EntityFrameworkCore;
using Bonnie.ExpenseManagement;
using Bonnie.ExpenseManagement.EntityFrameworkCore;
using Bonnie.SaaSService;
using Bonnie.SaaSService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Bonnie.DbMigrator;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AdminServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(AdminApplicationContractsModule))]
[DependsOn(typeof(AuthServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(AuthServiceApplicationContractsModule))]
[DependsOn(typeof(ExpensesEntityFrameworkCoreModule))]
[DependsOn(typeof(ExpensesApplicationContractsModule))]
[DependsOn(typeof(SaaSEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSApplicationContractsModule))]
public class BonnieDbMigratorModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		//Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
	}
}
