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
using Bonnie.ExpenseManagement.Tags;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Bonnie.ExpenseManagement.EntityFrameworkCore;

public static class ExpenseManagementDbContextModelCreatingExtensions
{
	public static void ConfigureExpenses(
		this ModelBuilder builder)
	{
		Check.NotNull(builder, nameof(builder));

		builder.Entity<Expense>(b =>
		{
			//Configure table & schema name
			b.ToTable(ExpensesDbProperties.DbTablePrefix + "Expenses", ExpensesDbProperties.DbSchema);

			b.ConfigureByConvention();

			//Properties
			b.Property(e => e.Description).IsRequired().HasMaxLength(ExpenseConsts.MaxDescriptionLength);

			//Relations

			//Indexes
			b.HasIndex(e => e.CreationTime);
		});

		builder.Entity<Tag>(b =>
		{
			//Configure table & schema name
			b.ToTable(ExpensesDbProperties.DbTablePrefix + "Tags", ExpensesDbProperties.DbSchema);

			b.ConfigureByConvention();

			//Properties
			b.Property(t => t.Name).IsRequired().HasMaxLength(TagConsts.MaxNameLength);
			b.Property(t => t.Description).IsRequired().HasMaxLength(TagConsts.MaxDescriptionLength);

			//Relations

			//Indexes
			b.HasIndex(e => e.CreationTime);
		});
	}
}
