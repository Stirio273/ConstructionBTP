using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class DashboardController : AdminController
    {
        public IActionResult Index()
        {
            Dashboard d = new Dashboard();
            d.Initialize();
            ViewBag.montantTotalPaiementEffectue = Formatter.FormatDouble(d.MontantTotalPaiementEffectue);
            ViewBag.montantTotalDevis = Formatter.FormatDouble(d.MontantTotalDevis);
            return View();
        }

        [HttpGet]
        public JsonResult TotalDevisMois(int annee)
        {
            List<MoisTotalDevis> moisTotalDevis = Dashboard.GetMontantTotalDevisParMois(null, annee);
            return Json(moisTotalDevis);
        }
    }
}