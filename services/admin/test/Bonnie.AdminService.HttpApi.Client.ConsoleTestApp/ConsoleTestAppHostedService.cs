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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace Bonnie.AdminService.HttpApi.Client.ConsoleTestApp;

public class ConsoleTestAppHostedService : IHostedService
{
	private readonly IConfiguration _configuration;

	public ConsoleTestAppHostedService(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using (var application = await AbpApplicationFactory.CreateAsync<AdminConsoleApiClientModule>(options =>
		{
			options.Services.ReplaceConfiguration(_configuration);
			options.UseAutofac();
		}))
		{
			await application.InitializeAsync();

			var demo = application.ServiceProvider.GetRequiredService<ClientDemoService>();
			await demo.RunAsync();

			await application.ShutdownAsync();
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
