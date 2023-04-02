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

namespace Bonnie.DbMigrator;

public class ServiceClient
{
	public String ClientId { get; set; } = String.Empty;
	public String ClientSecret { get; set; } = String.Empty;
	public String[] RootUrls { get; set; } = Array.Empty<String>();
	public String[] Scopes { get; set; } = Array.Empty<String>();
	public String[] GrantTypes { get; set; } = Array.Empty<String>();
	public String[] RedirectUris { get; set; } = Array.Empty<String>();
	public String[] PostLogoutRedirectUris { get; set; } = Array.Empty<String>();
	public String[] AllowedCorsOrigins { get; set; } = Array.Empty<String>();
}
