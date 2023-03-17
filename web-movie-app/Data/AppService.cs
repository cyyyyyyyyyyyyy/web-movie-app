using Microsoft.AspNetCore.Components;
using IMDbApiLib;
using IMDbApiLib.Models;

namespace web_movie_app.Data
{
	public class AppService
	{
		internal string searchString { get; set; } = "";
		internal string currSearchString = "";

		internal QueryBuilder qb = new();
		internal List<Movie> movieList = new();
		internal Dictionary<Person, List<Category>> personDictionary = new();
		internal List<Tag> tagList = new();
		internal List<Movie> topMovieList = new();

		internal Dictionary<string, MovieData> movieData = new();
		internal Dictionary<string, string> personData = new();
		internal Dictionary<string, MovieData> topMovieData = new();

		internal Movie detailedMovie = new();
		internal MovieData detailedMovieData = new();
		internal Person detailedPerson = new();
		internal string detailedPersonData = "";
		internal Tag detailedTag = new();
		internal async Task GetSearchReslults()
		{
			currSearchString = searchString;

			movieList = qb.MovieQuery(searchString);
			personDictionary = qb.PersonQuery(searchString);
			tagList = qb.TagQuery(searchString);

			/*var api = new ApiLib("k_6ax952gb");
			foreach (var m in movieList)
				{
				var data = await api.TitleAsync(m.imdbId);
				movieData.Add(m.imdbId, new MovieData { imageurl = data.Image, plot = data.Plot });
			}
			foreach (var p in personDictionary.Keys)
				{
				var data = await api.NameAsync(p.personId);
				personData.Add(p.personId, data.Image);
			}*/
			var t = await GetMovieAndPersonData(movieList, personDictionary.Keys.ToList());

			movieData = t.Item1;
			personData = t.Item2;
		}
		internal async Task<Tuple<Dictionary<string, MovieData>, Dictionary<string, string>> > GetMovieAndPersonData(List<Movie> movieList, List<Person> personList)
		{
			var md = new Dictionary<string, MovieData>();
			var pd = new Dictionary<string, string>();

			string tempDesc = "Description.aidsngasdfngsnadgljflsjdngadf;sj.gad;kngdsngndfnsgndksnnsngkdsfngldfskjnglsdkjfngkjdsfnlgndksfng";

			//var api = new ApiLib("k_6ax952gb");
			foreach (var m in movieList)
			{
				//TitleData data = await api.TitleAsync(m.imdbId);
				md.Add(m.imdbId, new MovieData { imageurl = "", plot = tempDesc }/*{ imageurl = data.Image, plot = data.Plot }*/);
			}
			foreach (var p in personList)
			{
				//NameData data = await api.NameAsync(p.personId);
				pd.Add(p.personId, "" /*data.Image*/);
			}

			return new Tuple<Dictionary<string, MovieData>, Dictionary<string, string>>(md, pd);
		}
		internal async Task GetTop10()
		{
			topMovieList = qb.GetTop10ByRating();
			var t = await GetMovieAndPersonData(topMovieList, new List<Person>());

			topMovieData = t.Item1;
		}
	}
}
