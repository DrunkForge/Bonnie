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

using Bonnie.ExpenseManagement.Tags;
using Volo.Abp.Domain.Entities.Auditing;

namespace Bonnie.ExpenseManagement.Expenses;

public class Expense : FullAuditedEntity<Guid>
{
	public String Description { get; set; } = String.Empty;
	public Decimal Amount { get; set; }
	public DateTime Date { get; set; }
	public ExpenseType Type { get; set; } = ExpenseType.Shared;
}
