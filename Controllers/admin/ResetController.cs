using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class ResetController : AdminController
    {
        public IActionResult Reset()
        {
            new Reset().ResetDatabase();
            return RedirectToAction("Logout", "Login");
        }
    }
}