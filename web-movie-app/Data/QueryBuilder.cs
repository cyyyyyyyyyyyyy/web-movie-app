using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web_movie_app.Data
{
	internal class QueryBuilder
	{
		internal List<Movie> GetTop10ByRating()
		{
			List<Movie> res = new();
			using (var context = new AppContext())
			{
				res = context.Movies
					.Where(m => m.rating >= 9)
					.OrderByDescending(m => m.rating)
					//.Include(m => m.tags)
					//.Where(m => m.tags.Count > 0)
					.Take(10)
					.Include(m => m.titles)
					.Where(m => m.titles.Count > 0)
					.Include(m => m.top10)
						.ThenInclude(top => top.titles)
					.Include(m => m.categories)
						.ThenInclude(c => c.person)
					.Include(m => m.tags)
					//.AsSplitQuery()
					.ToList();
			}

			return res;
		}
		internal List<Movie> MovieQuery(string movName)
		{
			List<Movie> res = new();
			using (var context = new AppContext())
			{
				res = context.Movies
					.Include(m => m.titles)					
					.Where(m => m.titles.Any(t => t.title.ToLower().Contains(movName.ToLower())))
					.Where(m => m.titles.Count > 0) //???????????????????
					.OrderByDescending(m => m.rating)
					.Take(20)
					.Include(m => m.top10)
						.ThenInclude(top => top.titles)
					.Include(m => m.tags)
					.Include(m => m.categories)
						.ThenInclude(c => c.person)
					//.AsSplitQuery()
					.ToList();
			}			

			return res;
		}

		internal Dictionary<Person, List<Category>> PersonQuery(string persName)
		{
			List<Person> persList;
			using (var context = new AppContext())
			{
				persList = (from p in context.Persons
							where p.name.ToLower().Contains(persName.ToLower())
							select p).Take(20).ToList();
			}

			Dictionary<Person, List<Category>> res = new();
			using (var context = new AppContext())
			{
				foreach (var pers in persList)
				{
					List<Category> catList = (from c in context.Categories.Where(c => c.person.personId == pers.personId)
								.Include(c => c.movie)
								.ThenInclude(m => m.titles)
								.Include(c => c.person)							 
								.AsSplitQuery()
							   select c).ToList();

					res.Add(pers, catList);
				}
			}

			return res;
		}

		internal List<Tag> TagQuery(string tagName)
		{
			List<Tag> tags = new();
			using (var context = new AppContext())
			{
				tags = (from t in context.Tags.Where(t => t.name.ToLower().Contains(tagName.ToLower()))
							.Include(t => t.movies)
							 .ThenInclude(m => m.titles)
							//where t.name == tagName
							.AsSplitQuery()
						select t).Take(20).ToList();
			}
			return tags;
		}
	}

}
