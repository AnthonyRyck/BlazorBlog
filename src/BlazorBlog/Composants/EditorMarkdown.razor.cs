using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorBlog.Composants
{
	partial class EditorMarkdown
	{
		[Parameter]
		public string Content { get; set; }

		[Parameter]
		public EventCallback<string> ContentChanged { get; set; }


		public MudTextField<string> Text { get; set; }

		private async Task OnTextChanged(string newValue)
		{
			await ContentChanged.InvokeAsync(newValue);
		}

		private async void OnClickLink()
		{
			string value = await GetSelection();
			string markdown = string.Empty;

			if (string.IsNullOrEmpty(value))
			{
				markdown = "[]()";
			}
			else
			{
				markdown = $"[{value}]()";
			}

			await Insert(markdown);
			await Text.FocusAsync();
		}

		private async void OnClickBold()
		{
			await BothSideWithMd("**");
		}

		private async void OnClickItalic()
		{
			await BothSideWithMd("*");
		}

		private async void OnClickQuote()
		{
			await StartWithMd(">");
		}

		private async void OnClickBlockCode()
		{
			await BothSideWithMd("```");
		}

		private async void OnClickImage()
		{
			string value = await GetSelection();
			string markdown = string.Empty;

			if (string.IsNullOrEmpty(value))
			{
				markdown = "![]()";
			}
			else
			{
				markdown = $"![{value}]()";
			}

			await Insert(markdown);
			await Text.FocusAsync();


			await Insert("![]()");
			await Text.FocusAsync();
		}

		private async Task BothSideWithMd(string markdownSymbol)
		{
			string value = await GetSelection();
			string markdown = string.Empty;

			if (string.IsNullOrEmpty(value))
			{
				markdown = $"{markdownSymbol} {markdownSymbol}";
			}
			else
			{
				markdown = markdownSymbol + value + markdownSymbol;
			}

			await Insert(markdown);
			await Text.FocusAsync();
		}

		private async Task StartWithMd(string markdownSymbol)
		{
			string value = await GetSelection();
			string markdown = string.Empty;

			if (string.IsNullOrEmpty(value))
			{
				markdown = markdownSymbol;
			}
			else
			{
				markdown = $"{markdownSymbol} {value}";
			}

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
