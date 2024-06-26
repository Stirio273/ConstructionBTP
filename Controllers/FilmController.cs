using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class FilmController : Controller
    {
        public IActionResult Index(int offset = 0, string sortOrder = "", string searchString = "", string currentFilter = "", int? page = null)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            List<Film> films = new List<Film>();
            switch (sortOrder)
            {
                case "name_desc":
                    films = new Film().Find<Film>(null, $"ORDER BY nom DESC LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
                    break;
                case "Date":
                    films = new Film().Find<Film>(null, $"ORDER BY dateDeSortie LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
                    break;
                case "date_desc":
                    films = new Film().Find<Film>(null, $"ORDER BY dateDeSortie DESC LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
                    break;
                case "Name":
                    films = new Film().Find<Film>(null, $"ORDER BY nom LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
                    break;
                default:
                    films = new Film().Find<Film>(null, $"ORDER BY id LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
                    break;
            }
            // new Film().FromSql
            DataTable table = new Film().ExecuteQuery(null, "SELECT COUNT(*) FROM film");
            ViewBag.filmCount = Convert.ToInt32(table.Rows[0][0]);
            return View(films);
        }

        public IActionResult UpdateForm(int idFilm)
        {
            List<Categorie> categories = new Categorie().Find<Categorie>(null, null);
            Film f = new Film();
            f.Id = idFilm;
            f.Find(null);
            ViewBag.categories = categories;
            return View(f);
        }

        public IActionResult Update(Film film)
        {
            if (ModelState.IsValid)
            {
                film.Update(null, null, new string[] { "nom", "idCategorie" });
            }
            ViewBag.categories = new Categorie().Find<Categorie>(null, null);
            return View("UpdateForm", film);
        }
    }
}