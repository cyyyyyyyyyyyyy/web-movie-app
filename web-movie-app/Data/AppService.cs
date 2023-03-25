using Microsoft.AspNetCore.Components;
using IMDbApiLib;
using IMDbApiLib.Models;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Newtonsoft.Json;

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

		internal Dictionary<string, MovieData> cachedMovieData = new();
		internal Dictionary<string, string> cachedPersonData = new();
		internal async Task GetSearchReslults()
		{
			currSearchString = searchString;

			movieList = qb.MovieQuery(searchString);
			personDictionary = qb.PersonQuery(searchString);
			tagList = qb.TagQuery(searchString);

			var t = await GetMovieAndPersonData(movieList, personDictionary.Keys.ToList());

			movieData = t.Item1;
			personData = t.Item2;
		}
		internal async Task<Tuple<Dictionary<string, MovieData>, Dictionary<string, string>> > GetMovieAndPersonData(List<Movie> movieList, List<Person> personList)
		{
			var md = new Dictionary<string, MovieData>();
			var notCachedMovieList = new List<Movie>();
			var pd = new Dictionary<string, string>();
			var notCachedPersonList = new List<Person>();
			string tempDesc = "Description.aidsngasdfngsnadgljflsjdngadf;sj.gad;kngdsngndfnsgndksnnsngkdsfngldfskjnglsdkjfngkjdsfnlgndksfng";
			string tempImg = "https://sun9-17.userapi.com/impf/c854324/v854324828/25c98d/HUwxh2oIzKQ.jpg?size=160x0&quality=90&sign=7f8625cd75cb370d596827e2bd1188a0";

			foreach (Movie m in movieList)
			{
				if (cachedMovieData.ContainsKey(m.imdbId))
					md.Add(m.imdbId, cachedMovieData[m.imdbId]);
				else
					notCachedMovieList.Add(m);
			}
			foreach (Person p in personList)
			{
				if (cachedPersonData.ContainsKey(p.personId))
					pd.Add(p.personId, cachedPersonData[p.personId]);
				else 
					notCachedPersonList.Add(p);
			}

			//var api = new ApiLib("k_6ax952gb");

			foreach (var m in notCachedMovieList)
			{

				//TitleData data = await api.TitleAsync(m.imdbId);
				var tmpData = new MovieData { imageurl = tempImg, plot = tempDesc }/*{ imageurl = data.Image, plot = data.Plot }*/;
				//OmdbApiMovie data = await GetMovieData(m.imdbId);
				//var tmpData = new MovieData { imageurl = data.Poster, plot = data.Plot };
				md.Add(m.imdbId, tmpData);
				cachedMovieData.Add(m.imdbId, tmpData);

			}
			foreach (var p in notCachedPersonList)
			{
				//NameData data = await api.NameAsync(p.personId);
				var tmpData = tempImg /*data.Image*/;
				pd.Add(p.personId, tmpData);
				cachedPersonData.Add(p.personId, tmpData);
			}

			return new Tuple<Dictionary<string, MovieData>, Dictionary<string, string>>(md, pd);
		}
		internal async Task GetTop10()
		{
			topMovieList = qb.GetTop10ByRating();
			var t = await GetMovieAndPersonData(topMovieList, new List<Person>());

			topMovieData = t.Item1;
		}

		internal async Task<OmdbApiMovie> GetMovieData(string movId)
		{
			string omdbKey = "783c5ac4";
			string url = "http://www.omdbapi.com/?i=" + movId + "&plot=full&apikey=" + omdbKey;

			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, url);

			using (var response = await client.SendAsync(request)) 
			{
				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var res = JsonConvert.DeserializeObject<OmdbApiMovie>(json);
					if (res != null)
					{
						return res;
					}
					else
					{
						return new OmdbApiMovie();
					}
				}
			}

			return new OmdbApiMovie();
		}
	}
}
