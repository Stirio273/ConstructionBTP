using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class ImportController : AdminController
    {
        public IActionResult Donnees()
        {
            return View();
        }

        public IActionResult Paiement(string message)
        {
            ViewBag.message = message;
            return View();
        }

        [HttpPost]
        public async Task ImportDonneesCSV(IFormFile maisonTravaux, IFormFile devis)
        {
            try
            {
                if (maisonTravaux != null && maisonTravaux.Length > 0)
                {
                    // To get the file name without the path:
                    // var fileName = Path.GetFileName(file.FileName);

                    // Process the file, for example, save it to the server:
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", maisonTravaux.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await maisonTravaux.CopyToAsync(stream);
                    }
                    Importation.ImportMaisonTravauxIntoDatabase(null, filePath);
                    Console.WriteLine("Success Maison et travaux");
                }
                if (devis != null && devis.Length > 0)
                {
                    // To get the file name without the path:
                    // var fileName = Path.GetFileName(file.FileName);

                    // Process the file, for example, save it to the server:
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", devis.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await devis.CopyToAsync(stream);
                    }
                    Importation.ImportDevisIntoDatabase(null, filePath);
                    Console.WriteLine("Success Devis");
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult ImportPaiementCSV(IFormFile paiement)
        {
            try
            {
                if (paiement != null && paiement.Length > 0)
                {
                    // To get the file name without the path:
                    // var fileName = Path.GetFileName(file.FileName);

                    // Process the file, for example, save it to the server:
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", paiement.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        paiement.CopyToAsync(stream);
                    }
                    Importation.ImportPaiementIntoDatabase(null, filePath);
                    return RedirectToAction("Paiement", "Import", new { message = "Success" });
                }
                return RedirectToAction("Paiement", "Import", new { message = "Error on uploading file" });
            }
            catch (System.Exception e)
            {
                return RedirectToAction("Paiement", "Import", new { message = e.Message });
            }
        }
    }
}