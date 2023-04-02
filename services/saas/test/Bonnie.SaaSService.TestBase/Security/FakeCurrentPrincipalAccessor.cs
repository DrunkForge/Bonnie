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

using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Bonnie.SaaSService.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
	protected override ClaimsPrincipal GetClaimsPrincipal()
	{
		return GetPrincipal();
	}

	private ClaimsPrincipal? _principal;

	private ClaimsPrincipal GetPrincipal()
	{
		if (_principal == null)
		{
			lock (this)
			{
				if (_principal == null)
				{
					_principal = new ClaimsPrincipal(
						new ClaimsIdentity(
							new List<Claim>
							{
									new Claim(AbpClaimTypes.UserId,"2e701e62-0953-4dd3-910b-dc6cc93ccb0d"),
									new Claim(AbpClaimTypes.UserName,"admin"),
									new Claim(AbpClaimTypes.Email,"admin@abp.io")
							}
						)
					);
				}
			}
		}

		return _principal;
	}
}
