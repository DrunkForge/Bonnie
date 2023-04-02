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

using Bonnie.ExpenseManagement.Expenses;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Bonnie.ExpenseManagement.EntityFrameworkCore;

[ConnectionStringName(ExpensesDbProperties.ConnectionStringName)]
public class ExpenseManagementDbContext : AbpDbContext<ExpenseManagementDbContext>, IExpenseManagementDbContext
{
	public DbSet<Expense> Expenses { get; set; } = default!;

	public ExpenseManagementDbContext(DbContextOptions<ExpenseManagementDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ConfigureExpenses();
	}
}
