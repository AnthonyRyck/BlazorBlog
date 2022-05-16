namespace BlazorBlog.ViewModels
{
	public interface ICounterViewModel
	{
		bool IsLoading { get; }

		/// <summary>
		/// Compteur sur l'année
		/// </summary>
		Counter YearCount { get; }

		/// <summary>
		/// Compteur sur le mois
		/// </summary>
		Counter MonthCount { get; }

		/// <summary>
		/// Compteur sur le jour
		/// </summary>
		Counter DayCount { get; }

		/// <summary>
		/// Compteur sur la semaine
		/// </summary>
		Counter WeekCount { get; }

		/// <summary>
		/// Post le plus visité du jour
		/// </summary>
		CounterPost TodayPostCounter { get; }

		/// <summary>
		/// Post le plus visité du mois
		/// </summary>
		CounterPost MonthPostCounter { get; }

		/// <summary>
		/// Fait les requêtes pour avoir les compteurs
		/// </summary>
		/// <returns></returns>
		Task LoadCounter();
	}
}
