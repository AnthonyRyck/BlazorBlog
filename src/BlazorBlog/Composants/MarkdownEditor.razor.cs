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

		public async void Dispose()
		{
			await HotKeysContext.DisposeAsync();
		}


		protected override void OnInitialized()
		{
			HotKeysContext.CreateContext()
					.Add(ModKeys.Ctrl, Keys.B, OnClickBold, "Pour mettre en gras.", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.I, OnClickItalic, "Pour mettre en italique.", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.Q, OnClickQuote, "Pour mettre en quote", Exclude.ContentEditable)
					.Add(ModKeys.Ctrl, Keys.C, OnClickBlockCode, "Pour faire un bloc de code", Exclude.ContentEditable)
					.Add(ModKeys.Alt, Keys.C, OnClickCodeInLine, "Pour faire une ligne de code", Exclude.ContentEditable)
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
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_BOLD);
		}

		private async void OnClickItalic()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_ITALIC);
		}

		private async void OnClickQuote()
		{
			string value = await GetSelection();
			await StartWithMd(ConstantesApp.MARKDOWN_SYNTAX_QUOTE, value);
		}

		private async void OnClickBlockCode()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_BLOCK_CODE);
		}

		private async void OnClickCodeInLine()
		{
			await BothSideWithMd(ConstantesApp.MARKDOWN_SYNTAX_CODE_IN_LINE);
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

		private async Task BothSideWithMd(string markdownSymbol)
		{
			string value = await GetSelection();
			string markdown = string.IsNullOrEmpty(value)
				? $"{markdownSymbol} {markdownSymbol}"
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
