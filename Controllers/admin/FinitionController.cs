using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class FinitionController : AdminController
    {
        public IActionResult Liste(int offset = 0)
        {
            List<TypeFinition> finitions = new TypeFinition().Find<TypeFinition>(null, @$"ORDER BY id 
            LIMIT {Constants.NUMBER_OF_ITEMS_PER_PAGE} OFFSET {offset}");
            DataTable table = new Devis().ExecuteQuery(null, @$"SELECT COUNT(*) FROM typeFinition");
            ViewBag.finitionCount = Convert.ToInt32(table.Rows[0][0]);
            return View(finitions);
        }

        public IActionResult UpdateForm(int idTypeFinition)
        {
            TypeFinition tf = new TypeFinition();
            tf.Id = idTypeFinition;
            tf.Find(null);
            return View(tf);
        }

        [HttpPost]
        public IActionResult Update(TypeFinition typeFinition)
        {
            if (ModelState.IsValid)
            {
                typeFinition.Update(null, null, new string[] { "nom", "pourcentage" });
            }
            return View("UpdateForm", typeFinition);
        }
    }
}