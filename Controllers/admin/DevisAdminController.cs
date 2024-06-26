using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class DevisAdminController : AdminController
    {

        [HttpGet]
        public IActionResult Details(Devis devis)
        {
            List<TravauxDevis> travaux = devis.GetAllTravaux(null, "ORDER BY numero");
            ViewBag.devis = devis;
            return View(travaux);
        }
        public IActionResult Liste(int offset = 0)
        {
            List<Devis> devis = new Administrateur().GetAllDevis(null, @$"ORDER BY numero ASC 
            LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
            DataTable table = new Devis().ExecuteQuery(null, @$"SELECT COUNT(*) FROM devis");
            ViewBag.devisCount = Convert.ToInt32(table.Rows[0][0]);
            return View(devis);
        }
    }
}