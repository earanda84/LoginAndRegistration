using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;

// 1.- Definir uso de Entity Framework
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// 2.- Librería para poder hashear el password
using Microsoft.AspNetCore.Identity;

// 3.- Paquete manejador para Session's
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoginAndRegistration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    // 1.-Llamar a la ruta post de Register
    [HttpPost("users/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> hasherPassword = new PasswordHasher<User>();

            newUser.Password = hasherPassword.HashPassword(newUser, newUser.Password);

            _context.Add(newUser);
            _context.SaveChanges();

            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == newUser.Email);

            if (userInDb != null)
            {
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            }

            Console.WriteLine($"EL ide del usuario es => {HttpContext.Session.GetInt32("UserId")}");

            return RedirectToAction("Success");
        }

        return View("Index");
    }

    // 2.-Validar el formulario y que este rediriga a la vista Index cuando se registre el usuario satisfactoriamente
    [SessionCheck]
    [HttpGet("success")]
    public IActionResult Success()
    {
        int? sessionId = HttpContext.Session.GetInt32("UserId");

        if (sessionId != null)
        {
            User? userInDb = _context.Users.FirstOrDefault(u => u.UserId == sessionId);

            return View(userInDb);
        }

        return View("Index");
    }


    // 3.- Llamar a la ruta Post de login del usuario
    [HttpPost("users/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (loginUser == null)
        {
            return View("Index");
        }

        if (ModelState.IsValid)
        {
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == loginUser.LEmail);

            if (userInDb == null)
            {
                ModelState.AddModelError("LEmail", "Email/Password inválido");
            }
            else
            {
                PasswordHasher<LoginUser> hasherLPassword = new PasswordHasher<LoginUser>();

                if (loginUser.LPassword != null)
                {

                    var result = hasherLPassword.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LPassword);

                    if (result == 0)
                    {
                        ModelState.AddModelError("LPassword", "Email/Password inválido");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);

                        return RedirectToAction("Success");
                    }
                }
                else
                {
                    ModelState.AddModelError("LPassword", "Invalid Email/Password");
                }
            }
        }
        return View("Index");
    }

    // 4.-Logout Session
    [HttpPost("logout")]
    public IActionResult Logout(User user)
    {
        if (user != null)
        {
            HttpContext.Session.Clear();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


// 5.- Manejador de Session's
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}