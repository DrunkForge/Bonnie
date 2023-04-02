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

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Bonnie.AuthService.EventHandler;

public class TenantCreatedEventHandler : IDistributedEventHandler<TenantCreatedEto>, ITransientDependency
{
	private readonly ICurrentTenant _currentTenant;
	private readonly ILogger<TenantCreatedEventHandler> _logger;
	private readonly IIdentityDataSeeder _authDataSeeder;
	public TenantCreatedEventHandler(
		ICurrentTenant currentTenant,
		IIdentityDataSeeder authDataSeeder,
		ILogger<TenantCreatedEventHandler> logger)
	{
		_currentTenant = currentTenant;
		_authDataSeeder = authDataSeeder;
		_logger = logger;
	}

	public async Task HandleEventAsync(TenantCreatedEto eventData)
	{
		try
		{
			using (_currentTenant.Change(eventData.Id))
			{

				_logger.LogInformation($"Creating admin user for tenant {eventData.Id}...");
				await _authDataSeeder.SeedAsync(
					eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminEmailPropertyName) ?? "admin@antosubash.com",
					eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminPasswordPropertyName) ?? "1q2w3E*",
					eventData.Id
				);
			}
		}
		catch (Exception ex)
		{
			await HandleErrorTenantCreatedAsync(eventData, ex);
		}
	}

	private Task HandleErrorTenantCreatedAsync(TenantCreatedEto eventData, Exception ex)
	{
		throw new NotImplementedException();
	}
}
