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
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Bonnie.DbMigrator;

public class OpenIddictDataSeeder : ITransientDependency
{
	private readonly IConfiguration _configuration;
	private readonly ICurrentTenant _currentTenant;
	private readonly IGuidGenerator _guidGenerator;
	private readonly IAbpApplicationManager _applicationManager;
	private readonly IOpenIddictScopeManager _scopeManager;
	private readonly IPermissionDataSeeder _permissionDataSeeder;
	private readonly IStringLocalizer<OpenIddictResponse> L;

	public OpenIddictDataSeeder(
		IAbpApplicationManager applicationManager,
		IOpenIddictScopeManager scopeManager,
		IPermissionDataSeeder permissionDataSeeder,
		IStringLocalizer<OpenIddictResponse> l,
		IGuidGenerator guidGenerator,
		IConfiguration configuration,
		ICurrentTenant currentTenant)
	{
		_configuration = configuration;
		_applicationManager = applicationManager;
		_scopeManager = scopeManager;
		_permissionDataSeeder = permissionDataSeeder;
		_guidGenerator = guidGenerator;
		_currentTenant = currentTenant;
		L = l;
	}

	[UnitOfWork]
	public async virtual Task SeedAsync()
	{
		using (_currentTenant.Change(null))
		{
			await CreateApiResourcesAsync();
			await CreateClientsAsync();
		}
	}

	private async Task CreateClientsAsync()
	{
		var clients = _configuration.GetSection("Clients").Get<List<ServiceClient>>();
		var commonScopes = new[] {
			OpenIddictConstants.Permissions.Scopes.Address,
			OpenIddictConstants.Permissions.Scopes.Email,
			OpenIddictConstants.Permissions.Scopes.Phone,
			OpenIddictConstants.Permissions.Scopes.Profile,
			OpenIddictConstants.Permissions.Scopes.Roles,
			"offline_access"
		};

		foreach (var client in clients)
		{
			//await CreateClientAsync(
			//    client.ClientId,
			//    commonScopes.Union(client.Scopes),
			//    client.GrantTypes,
			//    client.ClientSecret.ToSha256(),
			//    redirectUris: client.RedirectUris,
			//    postLogoutRedirectUris: client.PostLogoutRedirectUris
			//);

			var isClientSecretAvailable = !String.IsNullOrEmpty(client.ClientSecret);

			await CreateClientAsync(
					client.ClientId,
					displayName: client.ClientId,
					secret: isClientSecretAvailable ? client.ClientSecret : null,
					type: isClientSecretAvailable ? OpenIddictConstants.ClientTypes.Confidential : OpenIddictConstants.ClientTypes.Public,
					scopes: commonScopes.Union(client.Scopes).ToList(),
					grantTypes: client.GrantTypes.ToList(),
					redirectUris: client.RedirectUris,
					postLogoutRedirectUris: client.PostLogoutRedirectUris,
					consentType: OpenIddictConstants.ConsentTypes.Implicit
				);
		}
	}


	private async Task CreateApiResourcesAsync()
	{
		var apiResources = _configuration.GetSection("ApiResource").Get<String[]>();
		if (apiResources == null)
		{
			return;
		}

		foreach (var item in apiResources)
		{
			await CreateApiResourceAsync(item);
		}
	}

	private async Task CreateApiResourceAsync(String name)
	{
		if (await _scopeManager.FindByNameAsync(name) == null)
		{
			await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
			{
				Name = name,
				DisplayName = name + " API",
				Resources =
				{
					name
				}
			});
		}
	}

	private async Task CreateClientAsync(
		[NotNull] String name,
		[NotNull] String type,
		[NotNull] String consentType,
		String displayName,
		String? secret,
		List<String> grantTypes,
		List<String> scopes,
		String[]? redirectUris = null,
		String[]? postLogoutRedirectUris = null,
		List<String>? permissions = null)
	{
		if (!String.IsNullOrEmpty(secret) && String.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
		{
			throw new BusinessException(L["NoClientSecretCanBeSetForPublicApplications"]);
		}

		if (String.IsNullOrEmpty(secret) && String.Equals(type, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
		{
			throw new BusinessException(L["TheClientSecretIsRequiredForConfidentialApplications"]);
		}

		if (!String.IsNullOrEmpty(name) && await _applicationManager.FindByClientIdAsync(name) != null)
		{
			return;
			//throw new BusinessException(L["TheClientIdentifierIsAlreadyTakenByAnotherApplication"]);
		}

		var client = await _applicationManager.FindByClientIdAsync(name);
		if (client == null)
		{
			var application = new OpenIddictApplicationDescriptor
			{
				ClientId = name,
				Type = type,
				ClientSecret = secret,
				ConsentType = consentType,
				DisplayName = displayName
			};

			Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
			Check.NotNullOrEmpty(scopes, nameof(scopes));

			if (new[] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(grantTypes.Contains))
			{
				application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);
				if (String.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
					application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
				}
			}
			application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);


			foreach (var grantType in grantTypes)
			{
				if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
					application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
				}

				if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode || grantType == OpenIddictConstants.GrantTypes.Implicit)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
				}

				if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
					grantType == OpenIddictConstants.GrantTypes.ClientCredentials ||
					grantType == OpenIddictConstants.GrantTypes.Password ||
					grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
					grantType == OpenIddictConstants.GrantTypes.DeviceCode)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
					application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
					application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
				}

				if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
				}

				if (grantType == OpenIddictConstants.GrantTypes.Implicit)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
				}

				if (grantType == OpenIddictConstants.GrantTypes.Password)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
				}

				if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
				}

				if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
					application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
				}

				if (grantType == OpenIddictConstants.GrantTypes.Implicit)
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
					if (String.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
					{
						application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
						application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
					}
				}
			}

			var buildInScopes = new[]
			{
				OpenIddictConstants.Permissions.Scopes.Address,
				OpenIddictConstants.Permissions.Scopes.Email,
				OpenIddictConstants.Permissions.Scopes.Phone,
				OpenIddictConstants.Permissions.Scopes.Profile,
				OpenIddictConstants.Permissions.Scopes.Roles,
				"offline_access"
			};

			foreach (var scope in scopes)
			{
				if (buildInScopes.Contains(scope))
				{
					application.Permissions.Add(scope);
				}
				else
				{
					application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
				}
			}

			if (redirectUris != null)
			{
				foreach (var redirectUri in redirectUris)
				{
					if (!redirectUri.IsNullOrEmpty())
					{
						if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
						{
							throw new BusinessException(L["InvalidRedirectUri", redirectUri]);
						}

						if (application.RedirectUris.All(x => x != uri))
						{
							application.RedirectUris.Add(uri);
						}
					}
				}
			}

			if (postLogoutRedirectUris != null)
			{
				foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
				{
					if (!postLogoutRedirectUri.IsNullOrEmpty())
					{
						if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
						{
							throw new BusinessException(L["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]);
						}

						if (application.PostLogoutRedirectUris.All(x => x != uri))
						{
							application.PostLogoutRedirectUris.Add(uri);
						}
					}
				}
			}

			if (permissions != null)
			{
				await _permissionDataSeeder.SeedAsync(
					ClientPermissionValueProvider.ProviderName,
					name,
					permissions,
					null
				);
			}

			await _applicationManager.CreateAsync(application);
		}
	}
}
