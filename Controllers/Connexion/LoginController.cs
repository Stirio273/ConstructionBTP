using System.Security.Claims;
using Evaluation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Evaluation.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult LoginUser()
        {
            return View();
        }

        public IActionResult LoginAdmin(string? message)
        {
            if (message == null)
            {
                message = "";
            }
            return View((object)message);
        }

        [HttpPost]
        public IActionResult SignInUser(Client utilisateur)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    utilisateur = utilisateur.GetClientByPhoneNumber(null);
                    SignInIdentityUser(utilisateur);
                    return RedirectToAction("Index", "Home");
                }
                return View("LoginUser", utilisateur);
            }
            catch (System.Exception e)
            {
                return RedirectToAction("Index", "Login", new { message = e.Message });
            }
        }

        public IActionResult SignUpView()
        {
            return View("Register");
        }

        public IActionResult SignInAdmin(string email, string password)
        {
            try
            {
                var hasher = new PasswordHasher<string>(null);
                Administrateur admin = new Administrateur(0, email, password, 1);
                admin = admin.GetAdministrateurByEmail(null);
                PasswordVerificationResult result = hasher.VerifyHashedPassword(null, admin.Password, password);
                if (result == PasswordVerificationResult.Failed)
                {
                    throw new Exception("Email ou mot de passe invalide");
                }
                SignInIdentityAdmin(admin);
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception e)
            {
                return RedirectToAction("LoginAdmin", "Login", new { message = e.Message });
            }
        }

        [Authorize]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginUser");
        }

        public void SignInIdentityAdmin(Administrateur administrateur)
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, administrateur.Email), new Claim("UserID", administrateur.Id.ToString()),
                new Claim(ClaimTypes.Role, ProfilList.GetProfilName(administrateur.IdProfil)) }, CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.User = new ClaimsPrincipal(identity);
            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        public void SignInIdentityUser(Client utilisateur)
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.MobilePhone, utilisateur.Numero), new Claim("UserID", utilisateur.Id.ToString()),
                new Claim(ClaimTypes.Role, ProfilList.GetProfilName(utilisateur.IdProfil)) }, CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.User = new ClaimsPrincipal(identity);
            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}
