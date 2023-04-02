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

using Bonnie.AdminService.EntityFrameworkCore;
using Bonnie.AuthService.EntityFrameworkCore;
using Bonnie.SaaSService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;

namespace Bonnie.DbMigrator;

public class BonnieDbMigrationService : ITransientDependency
{
	private readonly ICurrentTenant _currentTenant;
	private readonly IDataSeeder _dataSeeder;
	private readonly ILogger<BonnieDbMigrationService> _logger;
	private readonly ITenantRepository _tenantRepository;
	private readonly IUnitOfWorkManager _unitOfWorkManager;

	public BonnieDbMigrationService(
		ILogger<BonnieDbMigrationService> logger,
		ITenantRepository tenantRepository,
		IDataSeeder dataSeeder,
		ICurrentTenant currentTenant,
		IUnitOfWorkManager unitOfWorkManager)
	{
		_logger = logger;
		_tenantRepository = tenantRepository;
		_dataSeeder = dataSeeder;
		_currentTenant = currentTenant;
		_unitOfWorkManager = unitOfWorkManager;
	}

	public async Task MigrateAsync(CancellationToken cancellationToken)
	{
		await MigrateHostAsync(cancellationToken);
		await MigrateTenantsAsync(cancellationToken);
		_logger.LogInformation("Migration completed!");
	}

	private async Task MigrateHostAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Migrating Host side...");
		await MigrateAllDatabasesAsync(null, cancellationToken);
		await SeedDataAsync();
	}

	private async Task MigrateTenantsAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Migrating tenants...");

		var tenants =
			await _tenantRepository.GetListAsync(includeDetails: true, cancellationToken: cancellationToken);
		var migratedDatabaseSchemas = new HashSet<String>();
		foreach (var tenant in tenants)
		{
			using (_currentTenant.Change(tenant.Id))
			{
				// Database schema migration
				var connectionString = tenant.FindDefaultConnectionString();
				if (!connectionString.IsNullOrWhiteSpace() && //tenant has a separate database
					!migratedDatabaseSchemas.Contains(connectionString)) //the database was not migrated yet
				{
					_logger.LogInformation($"Migrating tenant database: {tenant.Name} ({tenant.Id})");
					await MigrateAllDatabasesAsync(tenant.Id, cancellationToken);
					migratedDatabaseSchemas.AddIfNotContains(connectionString);
				}

				//Seed data
				_logger.LogInformation($"Seeding tenant data: {tenant.Name} ({tenant.Id})");
				await SeedDataAsync();
			}
		}
	}

	private async Task MigrateAllDatabasesAsync(
		Guid? tenantId,
		CancellationToken cancellationToken)
	{
		using (var uow = _unitOfWorkManager.Begin(true))
		{
			if (tenantId == null)
			{
				/* SaaSService schema should only be available in the host side */
				await MigrateDatabaseAsync<SaaSServiceDbContext>(cancellationToken);
			}

			await MigrateDatabaseAsync<AdminServiceDbContext>(cancellationToken);
			await MigrateDatabaseAsync<AuthServiceDbContext>(cancellationToken);

			await uow.CompleteAsync(cancellationToken);
		}

		_logger.LogInformation(
			$"All databases have been successfully migrated ({(tenantId.HasValue ? $"tenantId: {tenantId}" : "HOST")}).");
	}

	private async Task MigrateDatabaseAsync<TDbContext>(
		CancellationToken cancellationToken)
		where TDbContext : DbContext, IEfCoreDbContext
	{
		_logger.LogInformation($"Migrating {typeof(TDbContext).Name.RemovePostFix("DbContext")} database...");

		var dbContext = await _unitOfWorkManager.Current.ServiceProvider
			.GetRequiredService<IDbContextProvider<TDbContext>>()
			.GetDbContextAsync();

		await dbContext
			.Database
			.MigrateAsync(cancellationToken);
	}

	private async Task SeedDataAsync()
	{
		await _dataSeeder.SeedAsync(
			new DataSeedContext(_currentTenant.Id)
				.WithProperty(IdentityDataSeedContributor.AdminEmailPropertyName, "admin@abp.io")
				.WithProperty(IdentityDataSeedContributor.AdminPasswordPropertyName, "1q2w3E*")
		);
	}
}
