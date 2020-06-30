using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NAP.Signin.AspNetCore
{
    public class ErrController : Controller
    {
        [AllowAnonymous]
        public new ActionResult NotFound()
        {
            return View("Fanex.NAP.Signin.AspNetCore.Views.NotFound");
        }
    }
}
