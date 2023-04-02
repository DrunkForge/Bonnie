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

using Bonnie.ExpenseManagement.Samples;

namespace Bonnie.ExpenseManagement.EntityFrameworkCore.Samples;

public class SampleRepository_Tests : SampleRepository_Tests<ExpensesEntityFrameworkCoreTestModule>
{
	/* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to EF core.
     */
}
