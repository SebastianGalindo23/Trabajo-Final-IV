using Microsoft.AspNetCore.Mvc;
using POS.Data;
using POS.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace POS.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public EmpleadosController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // Acción para listar todos los usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();  // Obtiene todos los usuarios
            return View(users);
        }

        // Acción para ver detalles de un usuario
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // Acción para crear un nuevo usuario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmpleadoRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Usuario,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Rol = model.Rol,
                    DPI = model.DPI // Asumiendo que tienes el campo DPI en el modelo
                };

                var result = await _userManager.CreateAsync(user, model.Contrasena);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Rol);
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Verificar que el id no sea nulo
            if (id == null)
            {
                return NotFound();
            }

            // Buscar el usuario en la base de datos
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Mapear los datos del usuario a la vista
            var model = new EmpleadoRegisterModel
            {
                Id = user.Id,
                Usuario = user.UserName,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol, // Si tienes un campo Rol en el modelo de usuario
                DPI = user.DPI
            };

            return View(model);
        }

        // Acción POST para actualizar un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EmpleadoRegisterModel model)
        {
            // Verificar que el ID coincida con el que se recibe en el modelo
            if (id != model.Id)
            {
                return NotFound();
            }

            // Comprobar que el modelo es válido
            if (ModelState.IsValid)
            {
                // Buscar el usuario en la base de datos
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Actualizar las propiedades del usuario
                user.UserName = model.Usuario;
                user.Email = model.Email;
                user.Nombre = model.Nombre;
                user.DPI = model.DPI;

                // Si tienes un campo de rol, actualízalo aquí también
                if (user.Rol != model.Rol)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

                    if (!string.IsNullOrEmpty(model.Rol))
                    {
                        await _userManager.AddToRoleAsync(user, model.Rol);
                    }
                }

                try
                {
                    // Guardar los cambios en la base de datos
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al actualizar el usuario: {ex.Message}");
                }
            }

            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }
    }
}
