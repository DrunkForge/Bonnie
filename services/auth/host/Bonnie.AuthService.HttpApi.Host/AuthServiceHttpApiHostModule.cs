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
using Bonnie.Hosting.Shared;
using Bonnie.SaaSService.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bonnie.AuthService;

[DependsOn(typeof(BonnieHostingModule))]
[DependsOn(typeof(AuthServiceApplicationModule))]
[DependsOn(typeof(AuthServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(AuthServiceHttpApiModule))]
[DependsOn(typeof(AdminServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSEntityFrameworkCoreModule))]
public class AuthServiceHttpApiHostModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		if (hostingEnvironment.IsDevelopment())
		{
			Configure<AbpVirtualFileSystemOptions>(options =>
			{
				options.FileSets.ReplaceEmbeddedByPhysical<AuthServiceDomainSharedModule>(
					Path.Combine(hostingEnvironment.ContentRootPath,
						String.Format("..{0}..{0}src{0}Bonnie.AuthService.Domain.Shared",
							Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<AuthServiceDomainModule>(
					Path.Combine(hostingEnvironment.ContentRootPath,
						String.Format("..{0}..{0}src{0}Bonnie.AuthService.Domain", Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<AuthServiceApplicationContractsModule>(
					Path.Combine(hostingEnvironment.ContentRootPath,
						String.Format("..{0}..{0}src{0}Bonnie.AuthService.Application.Contracts",
							Path.DirectorySeparatorChar)));
				options.FileSets.ReplaceEmbeddedByPhysical<AuthServiceApplicationModule>(
					Path.Combine(hostingEnvironment.ContentRootPath,
						String.Format("..{0}..{0}src{0}Bonnie.AuthService.Application",
							Path.DirectorySeparatorChar)));
			});
		}

		context.Services.AddAbpSwaggerGenWithOAuth(
			configuration["AuthServer:Authority"],
			new Dictionary<String, String> {
				{"AuthService", "AuthService API"}
			},
			options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.CustomSchemaIds(type => type.FullName);
			});


		context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = configuration["AuthServer:Authority"];
				options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
				options.Audience = "AuthService";
			});

		Configure<AbpDistributedCacheOptions>(options =>
		{
			options.KeyPrefix = "AuthService:";
		});

		var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("AuthService");
		if (!hostingEnvironment.IsDevelopment())
		{
			var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
			dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "AuthService-Protection-Keys");
		}

		context.Services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder
					.WithOrigins(
						configuration["App:CorsOrigins"]
							.Split(",", StringSplitOptions.RemoveEmptyEntries)
							.Select(o => o.RemovePostFix("/"))
							.ToArray()
					)
					.WithAbpExposedHeaders()
					.SetIsOriginAllowedToAllowWildcardSubdomains()
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
			});
		});
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		IdentityModelEventSource.ShowPII = true;
		var app = context.GetApplicationBuilder();
		var env = context.GetEnvironment();

		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseCorrelationId();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseCors();
		app.UseAuthentication();

		app.UseMultiTenancy();

		app.UseAbpRequestLocalization();
		app.UseAuthorization();
		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");

			var configuration = context.GetConfiguration();
			options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
			options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
			options.OAuthScopes("AuthService");
		});
		app.UseAuditing();
		app.UseAbpSerilogEnrichers();
		app.UseConfiguredEndpoints();
	}
}
