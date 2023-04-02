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
using Bonnie.AuthService;
using Bonnie.AuthService.EntityFrameworkCore;
using Bonnie.Hosting.Shared;
using Bonnie.SaaSService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Bonnie.AdminService;

[DependsOn(typeof(BonnieHostingModule))]
[DependsOn(typeof(AdminApplicationModule))]
[DependsOn(typeof(AdminServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(AdminHttpApiModule))]
[DependsOn(typeof(AuthServiceApplicationContractsModule))]
[DependsOn(typeof(AuthServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpIdentityDomainModule))]
[DependsOn(typeof(SaaSApplicationContractsModule))]
public class AdminHttpApiHostModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		var hostingEnvironment = context.Services.GetHostingEnvironment();
		var configuration = context.Services.GetConfiguration();

		context.Services.AddAbpSwaggerGenWithOAuth(
			configuration["AuthServer:Authority"],
			new Dictionary<String, String> {
				{"AdminService", "AdminService API"}
			},
			options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "AdminService API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.CustomSchemaIds(type => type.FullName);
			});


		context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = configuration["AuthServer:Authority"];
				options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
				options.Audience = "AdminService";
			});

		Configure<AbpDistributedCacheOptions>(options =>
		{
			options.KeyPrefix = "AdminService:";
		});

		var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("AdminService");
		if (!hostingEnvironment.IsDevelopment())
		{
			var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
			dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "AdminService-Protection-Keys");
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
		});
		app.UseAuditing();
		app.UseAbpSerilogEnrichers();
		app.UseConfiguredEndpoints();
	}
}
