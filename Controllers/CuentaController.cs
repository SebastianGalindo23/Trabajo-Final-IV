using System.Linq;
using Microsoft.AspNetCore.Mvc;
using POS.Data;
using POS.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;

namespace POS.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public CuentaController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(EmpleadoRegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Usuario,
                Email = model.Email,
                Nombre = model.Nombre,
                Rol = model.Rol,
            };

            var result = await _userManager.CreateAsync(user, model.Contrasena);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Rol);
                return RedirectToAction("Login", "Cuenta");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Ventas");
            }
            else
            {
                ViewData["Layout"] = "_LoginLayout";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(EmpleadoLoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Usuario, model.Contrasena, model.Recordar, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Ventas");
            }

            ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
