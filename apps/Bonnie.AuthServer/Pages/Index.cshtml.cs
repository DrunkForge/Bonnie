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

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Localization;
using Volo.Abp.OpenIddict.Applications;

namespace Bonnie.Pages;

public class IndexModel : AbpPageModel
{
	public List<OpenIddictApplication> Applications { get; protected set; } = new List<OpenIddictApplication>();

	public IReadOnlyList<LanguageInfo> Languages { get; protected set; } = Array.Empty<LanguageInfo>();

	public String CurrentLanguage { get; protected set; } = String.Empty;

	protected IOpenIddictApplicationRepository OpenIdApplicationRepository { get; }

	protected ILanguageProvider LanguageProvider { get; }

	public IndexModel(IOpenIddictApplicationRepository openIdApplicationRepository, ILanguageProvider languageProvider)
	{
		OpenIdApplicationRepository = openIdApplicationRepository;
		LanguageProvider = languageProvider;
	}

	public async Task OnGetAsync()
	{
		Applications = await OpenIdApplicationRepository.GetListAsync();

		Languages = await LanguageProvider.GetLanguagesAsync();
		CurrentLanguage = CultureInfo.CurrentCulture.DisplayName;
	}
}
