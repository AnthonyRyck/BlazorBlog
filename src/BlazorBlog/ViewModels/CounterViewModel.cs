namespace BlazorBlog.ViewModels
{
	public class CounterViewModel : ICounterViewModel
	{
		private readonly string UserName;
		private readonly BlogContext Context;


		public CounterViewModel(IHttpContextAccessor httpContextAccessor, BlogContext blogContext)
		{
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			Context = blogContext;
		}

		#region ICounterViewModel

		public bool IsLoading { get; private set; }

		public Counter YearCount { get; private set; }

		public Counter MonthCount { get; private set; }

		public Counter DayCount { get; private set; }

		public Counter WeekCount { get; private set; }

		public CounterPost TodayPostCounter { get; private set; }

		public CounterPost MonthPostCounter { get; private set; }


		public string[] XAxisLabels { get; private set; }

		public List<ChartSeries> Series { get; private set; }

		public async Task LoadCounter()
		{
			IsLoading = true;

			try
			{

				DayCount = await Context.GetCounterDay(UserName);
				YearCount = await Context.GetCounterYear(UserName);
				MonthCount = await Context.GetCounterMonth(UserName);
				WeekCount = await Context.GetCounterWeek(UserName);

				TodayPostCounter = await Context.GetCounterPostDay(UserName);
				MonthPostCounter = await Context.GetCounterPostMonth(UserName);

				var tempGraphData = await Context.GetCounter(UserName, 20);
				XAxisLabels = tempGraphData.Select(x => x.Key).ToArray();
				Series = new List<ChartSeries>()
				{
					new ChartSeries() { Name = "Visites", Data = tempGraphData.Select(x => x.Value).ToArray() }
				};
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ERROR - LOAD COUNTER");
			}

			IsLoading = false;
		}

		#endregion
	}
}
