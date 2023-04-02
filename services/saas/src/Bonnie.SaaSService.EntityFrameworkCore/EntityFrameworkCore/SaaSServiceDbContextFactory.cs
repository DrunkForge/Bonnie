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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bonnie.SaaSService.EntityFrameworkCore;

public class SaaSServiceDbContextFactory : IDesignTimeDbContextFactory<SaaSServiceDbContext>
{
	public SaaSServiceDbContext CreateDbContext(String[] args)
	{
		var configuration = BuildConfiguration();

		var builder = new DbContextOptionsBuilder<SaaSServiceDbContext>()
			.UseNpgsql(GetConnectionStringFromConfiguration());

		return new SaaSServiceDbContext(builder.Options);
	}

	private static String GetConnectionStringFromConfiguration()
	{
		return BuildConfiguration().GetConnectionString(SaaSDbProperties.ConnectionStringName)
			?? BuildConfiguration().GetConnectionString("Default")
			?? throw new InvalidOperationException($"Missing connection string named: {SaaSDbProperties.ConnectionStringName}");
	}

	private static IConfigurationRoot BuildConfiguration()
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false);

		return builder.Build();
	}
}