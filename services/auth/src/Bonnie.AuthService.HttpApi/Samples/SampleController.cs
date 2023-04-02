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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Bonnie.AuthService.Samples;

[Area(AuthServiceRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AuthServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/AuthService/sample")]
public class SampleController : AuthServiceController, ISampleAppService
{
	private readonly ISampleAppService _sampleAppService;

	public SampleController(ISampleAppService sampleAppService)
	{
		_sampleAppService = sampleAppService;
	}

	[HttpGet]
	public async Task<SampleDto> GetAsync()
	{
		return await _sampleAppService.GetAsync();
	}

	[HttpGet]
	[Route("authorized")]
	[Authorize]
	public async Task<SampleDto> GetAuthorizedAsync()
	{
		return await _sampleAppService.GetAsync();
	}
}
