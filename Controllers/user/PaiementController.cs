using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class PaiementController : UserController
    {
        [HttpGet]
        public IActionResult Index(Devis devis)
        {
            double montantRestant = devis.GetMontantRestant(null);
            ViewBag.montantRestant = montantRestant;
            return View(devis);
        }

        [HttpPost]
        public JsonResult Payer(Paiement paiement)
        {
            try
            {
                Devis d = new Devis();
                d.Id = paiement.IdDevis;
                d.Payer(null, paiement);
                return Json(null);
            }
            catch (System.Exception e)
            {
                return Json(e.Message);
            }
        }
    }
}