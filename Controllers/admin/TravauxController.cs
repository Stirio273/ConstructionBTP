using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class TravauxController : AdminController
    {
        public IActionResult Liste(int offset = 0)
        {
            List<TypeTravaux> travaux = new TypeTravaux().GetAllTypeTravaux(null, @$"ORDER BY numero 
            LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
            DataTable table = new Devis().ExecuteQuery(null, @$"SELECT COUNT(*) FROM typeTravaux");
            ViewBag.devisCount = Convert.ToInt32(table.Rows[0][0]);
            return View(travaux);
        }

        public IActionResult UpdateForm(int idTypeTravaux)
        {
            List<Unite> unites = new Unite().Find<Unite>(null, null);
            TypeTravaux tp = new TypeTravaux();
            tp.Id = idTypeTravaux;
            tp.Find(null);
            ViewBag.unites = unites;
            return View(tp);
        }

        [HttpPost]
        public IActionResult Update(TypeTravaux typeTravaux)
        {
            if (ModelState.IsValid)
            {
                typeTravaux.Update(null, null, new string[] { "numero", "designation", "idunite", "prixUnitaire" });
            }
            ViewBag.unites = new Unite().Find<Unite>(null, null);
            return View("UpdateForm", typeTravaux);
        }
    }
}