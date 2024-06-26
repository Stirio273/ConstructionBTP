using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class DevisUserController : UserController
    {
        [HttpGet]
        public IActionResult Details(Devis devis)
        {
            int idClient = Convert.ToInt32(User.FindFirst("UserID").Value);
            List<TravauxDevis> travaux = devis.GetAllTravaux(null, "ORDER BY numero");
            List<Paiement> paiements = new Paiement().Find<Paiement>(null, $"WHERE iddevis = {devis.Id}");
            devis = devis.GetAllDevis(null, $"WHERE id = {devis.Id}")[0];
            ViewBag.devis = devis;
            ViewBag.paiements = paiements;
            return View(travaux);
        }

        public IActionResult Liste(int offset = 0)
        {
            int idClient = Convert.ToInt32(User.FindFirst("UserID").Value);
            List<Devis> devis = new Client(idClient, "", 2).GetAllDevis(null, @$"ORDER BY numero ASC 
            LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
            DataTable table = new Devis().ExecuteQuery(null, @$"SELECT COUNT(*) FROM devis WHERE 
            idClient = {idClient}");
            ViewBag.devisCount = Convert.ToInt32(table.Rows[0][0]);
            return View(devis);
        }
        public IActionResult Create(Devis devis)
        {
            devis.IdClient = Int32.Parse(User.FindFirst("UserID").Value);
            devis.Create(null);
            return RedirectToAction("CreateForm", "DevisUser");
        }
        public IActionResult CreateForm()
        {
            List<TypeMaison> typeMaisons = new List<TypeMaison>();
            List<TypeFinition> typeFinitions = new List<TypeFinition>();
            Formulaire.GetDataForCreateDevisUser(ref typeMaisons, ref typeFinitions);
            ViewBag.typeMaisons = typeMaisons;
            ViewBag.typeFinitions = typeFinitions;
            return View();
        }
    }
}