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
            if (id == null)
            {
                Console.WriteLine("ID no proporcionado.");
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"Usuario no encontrado con ID: {id}");
                return NotFound();
            }
            // Log para confirmar que se ha encontrado el usuario
            Console.WriteLine($"Usuario encontrado: {user.UserName}");

            var model = new EmpleadoRegisterModel
            {
                Id = user.Id,
                Usuario = user.UserName,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                DPI = user.DPI
            };
            // Log para confirmar los datos que se están pasando al modelo
            Console.WriteLine($"Cargando datos para el usuario: {model.Usuario}, {model.Nombre}, {model.Email}, {model.Rol}, {model.DPI}");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EmpleadoRegisterModel model, string oldPassword, string newPassword, string confirmPassword)
        {
            // Log para verificar el id recibido
            Console.WriteLine($"ID recibido en POST: {id}");
            Console.WriteLine($"ID del modelo: {model.Id}");

            if (id != model.Id)
            {
                // Log si los ids no coinciden
                Console.WriteLine($"ID no coincide: {id} != {model.Id}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    // Log si el usuario no se encuentra
                    Console.WriteLine($"Usuario no encontrado al intentar actualizar: {id}");
                    return NotFound();
                }

                // Log para ver los datos recibidos en el modelo
                Console.WriteLine($"Actualizando usuario: {model.Usuario}, {model.Nombre}, {model.Email}");

                // Actualizar datos del usuario
                user.Nombre = model.Nombre;
                user.UserName = model.Usuario;
                user.Email = model.Email;
                user.DPI = model.DPI;

                // Log para confirmar los datos actualizados
                Console.WriteLine($"Datos actualizados: {user.UserName}, {user.Nombre}, {user.Email}, {user.DPI}");

                // Actualizar rol si aplica
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!string.IsNullOrEmpty(model.Rol))
                {
                    // Log para confirmar que se está agregando un rol
                    Console.WriteLine($"Añadiendo rol: {model.Rol}");
                    await _userManager.AddToRoleAsync(user, model.Rol);
                }

                // Cambiar contraseña si se proporciona
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword != confirmPassword)
                    {
                        // Log si las contraseñas no coinciden
                        Console.WriteLine("Las contraseñas no coinciden.");
                        ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                        return View(model);
                    }

                    // Log para intentar cambiar la contraseña
                    Console.WriteLine($"Intentando cambiar la contraseña para el usuario {user.UserName}");
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        // Log de errores al cambiar la contraseña
                        foreach (var error in changePasswordResult.Errors)
                        {
                            Console.WriteLine($"Error al cambiar la contraseña: {error.Description}");
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                // Guardar cambios en el usuario
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Log para confirmar que los cambios fueron guardados
                    Console.WriteLine("Usuario actualizado exitosamente.");
                    return RedirectToAction("Index");
                }

                // Log de errores al guardar los cambios
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error al actualizar el usuario: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log para ver que el modelo no es válido
                Console.WriteLine("Modelo inválido.");
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
