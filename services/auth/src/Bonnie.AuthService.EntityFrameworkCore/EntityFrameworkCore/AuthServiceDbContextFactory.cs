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

namespace Bonnie.AuthService.EntityFrameworkCore;

public class AuthServiceDbContextFactory : IDesignTimeDbContextFactory<AuthServiceDbContext>
{
	public AuthServiceDbContext CreateDbContext(String[] args)
	{
		var configuration = BuildConfiguration();

		var builder = new DbContextOptionsBuilder<AuthServiceDbContext>()
			.UseNpgsql(GetConnectionStringFromConfiguration());

		return new AuthServiceDbContext(builder.Options);
	}

	private static String GetConnectionStringFromConfiguration()
	{
		return BuildConfiguration().GetConnectionString(AuthServiceDbProperties.ConnectionStringName)
			?? BuildConfiguration().GetConnectionString("Default")
			?? throw new InvalidOperationException($"Missing connection string named: {AuthServiceDbProperties.ConnectionStringName}");
	}

	private static IConfigurationRoot BuildConfiguration()
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false);

		return builder.Build();
	}
}
