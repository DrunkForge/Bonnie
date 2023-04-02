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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Bonnie.DbMigrator;

internal class Program
{
	private async static Task Main(String[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
				.MinimumLevel.Override("Bonnie", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.Async(c => c.Console())
			.CreateLogger();

		await CreateHostBuilder(args).RunConsoleAsync();
	}

	public static IHostBuilder CreateHostBuilder(String[] args)
	{
		return Host.CreateDefaultBuilder(args)
			.AddAppSettingsSecretsJson()
			.ConfigureLogging((context, logging) => logging.ClearProviders())
			.ConfigureServices((hostContext, services) =>
			{
				services.AddHostedService<DbMigratorHostedService>();
			});
	}
}
