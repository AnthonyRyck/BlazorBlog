using Microsoft.JSInterop;
using MudBlazor;
using Toolbelt.Blazor.HotKeys;

namespace BlazorBlog.Composants
{
	partial class MarkdownEditor : IDisposable
	{
		[Parameter]
		public string Content { get; set; }

		[Parameter]
		public EventCallback<string> ContentChanged { get; set; }

		[Inject]
		private HotKeys HotKeysContext { get; set; }

		[Inject]
		private IJSRuntime JSRuntime { get; set; }

		private string urlYoutube;
		private bool IsYoutubeDisplayed;

		public async void Dispose()
		{
			await HotKeysContext.DisposeAsync();
		}


		protected override void OnInitialized()
		{
			HotKeysContext.CreateContext()
					.Add(ModKeys.Ctrl, Keys.B, OnClickBold, "Pour mettre en gras.", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.I, OnClickItalic, "Pour mettre en italique.", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.Q, OnClickQuote, "Pour mettre en quote", Exclude.ContentEditable)
					.Add(ModKeys.Alt | ModKeys.Shift, Keys.K, OnClickBlockCode, "Pour faire un bloc de code", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.K, OnClickCodeInLine, "Pour faire une ligne de code", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.I, OnClickImage, "Pour mettre une image", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.K, OnClickLink, "Pour mettre un lien.", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.L, OnClickList, "Pour mettre une liste non ordonnée.", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.L, OnClickListOrdered, "Pour mettre une liste ordonnée.", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.T, OnClickListOrdered, "Pour mettre un tableau.", Exclude.ContentEditable);
		}

		public MudTextField<string> Text { get; set; }

		private async Task OnTextChanged(string newValue)
		{
			await ContentChanged.InvokeAsync(newValue);
		}
		
		private async void OnClickLink()
		{
			string value = await GetSelection();
			string markdown = string.IsNullOrEmpty(value)
				? ConstantesApp.MARKDOWN_SYNTAX_LINK
				: $"{ConstantesApp.MARKDOWN_SYNTAX_LINK_START}{value}{ConstantesApp.MARKDOWN_SYNTAX_LINK_END}";
			
			await Insert(markdown);
			await Text.FocusAsync();
		}

		private async void OnClickBold()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_BOLD, "PutYourWord");
		}

		private async void OnClickItalic()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_ITALIC, "PutYourWord");
		}

		private async void OnClickQuote()
		{
			string value = await GetSelection();
			await StartWithMd(ConstantesApp.MARKDOWN_SYNTAX_QUOTE, value);
		}

		private async void OnClickBlockCode()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_BLOCK_CODE, Environment.NewLine + "PutYourBlockCode" + Environment.NewLine);
		}

		private async void OnClickCodeInLine()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_CODE_IN_LINE, "code");
		}

		private async void OnClickImage()
		{
			string value = await GetSelection();
			string markdown = string.IsNullOrEmpty(value)
				? ConstantesApp.MARKDOWN_SYNTAX_IMAGE
				: $"{ConstantesApp.MARKDOWN_SYNTAX_IMAGE_START}{value}{ConstantesApp.MARKDOWN_SYNTAX_IMAGE_END}";

			await Insert(markdown);
			await Text.FocusAsync();
		}


		private async void OnClickList()
		{
			await StartWithMd(ConstantesApp.MARKDOWN_SYNTAX_LIST_BULLET, string.Empty);
		}

		private async void OnClickListOrdered()
		{
			await StartWithMd(ConstantesApp.MARKDOWN_SYNTAX_LIST_ORDERED, string.Empty);
		}

		private async void OnClickTableau()
		{
			string MARKDOWN_SYNTAX_TABLEAU = "| Column 1 | Column 2 | Column 3 |" + Environment.NewLine
											+ "| -------- | -------- | -------- |" + Environment.NewLine
											+ "| Cell 1   | Cell 2   | Cell 3   |" + Environment.NewLine;
			await StartWithMd(MARKDOWN_SYNTAX_TABLEAU, string.Empty);
		}

		private async void OnClickVideoYoutube()
		{
			IsYoutubeDisplayed = !IsYoutubeDisplayed;
		}

		private async Task AddYoutubeVideo()
		{
			// Extraire l'id de la vidéo.
			// Exemple : https://www.youtube.com/watch?v=1qOXCpCwmJ4&pp=ugMICgJmchABGAE%3D

			var splited = urlYoutube.Split("?v=");
			string url = string.Empty;
			string idvideo = string.Empty;

			if (splited.Length > 1)
			{
				idvideo = splited[1];
				url = urlYoutube;
			}
			else
			{
				idvideo = "METTRE_ID_VIDEO";
				url = "METTRE_URL_YOUTUBE";
			}

			string MARKDOWN_SYNTAX_YOUTUBE = Environment.NewLine
				 + "  " + Environment.NewLine
				 + "**Clique sur l'image ouvrir la vidéo Youtube**  " + Environment.NewLine
				 + $"[![](https://i.ytimg.com/vi/{idvideo}/hqdefault.jpg)]({url})" + Environment.NewLine
				 + "  " + Environment.NewLine;
			await StartWithMd(MARKDOWN_SYNTAX_YOUTUBE, string.Empty);
			OnClickVideoYoutube();
		}

		private async Task BothSideWithMd(string markdownSymbol, string defaultWord)
		{
			string value = await GetSelection();
			string markdown = string.IsNullOrEmpty(value)
				? $"{markdownSymbol}{defaultWord}{markdownSymbol}"
				: $"{markdownSymbol}{value}{markdownSymbol}";

			await Insert(markdown);
			await Text.FocusAsync();
		}

		private async Task StartWithMd(string markdownSymbol, string value)
		{
			string markdown = string.IsNullOrEmpty(value)
				? markdownSymbol
				: $"{markdownSymbol}{value}";
				
			await Insert(markdown);
			await Text.FocusAsync();
		}

		private async Task Insert(string markdown)
		{
			await JSRuntime.InvokeVoidAsync("insertAtCursor", "texteditor", markdown);
		}

		private async Task<string> GetSelection()
		{
			try
			{
				return await JSRuntime.InvokeAsync<string>("getSelectedText", "texteditor");
			}
			catch (Exception ex)
			{
				var test = ex;
				throw;
			}
		}
	}
}
