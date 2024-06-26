using System.Security.Claims;

namespace Evaluation.Models
{
    public class FrontPage
    {
        public static string ShowUser(ClaimsPrincipal user)
        {
            if (user.Claims.Where(c => c.Type == ClaimTypes.Role).ToList()[0].Value.Equals("BTP"))
                return user.FindFirst(ClaimTypes.Email).Value;
            else
                return user.FindFirst(ClaimTypes.MobilePhone).Value;
        }
    }
}