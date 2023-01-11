using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Module4Lab7.Models;
using System.Diagnostics;
using System.Net;

namespace Module4Lab7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController>? _logger;
        MyAppContext? context;

        public HomeController(ILogger<HomeController> logger,MyAppContext db)
        {
            _logger = logger;
            context = db;
        }

        private void StartInitial()
        {
            var users = context?.Users.AsNoTracking().ToList();

            if (users?.Count==0)
            {
                context?.Users.Add(new User() { Name = "Tom", Email = "tom@gmail.com", Password = "12345" });
                context?.Users.Add(new User() { Name = "Ben", Email = "ben@gmail.com", Password = "12345" });
                context?.Users.Add(new User() { Name = "Alex", Email = "alex@gmail.com", Password = "12345" });

                context?.SaveChangesAsync();
            }
            

            
        }

        public IActionResult LogIn(User user)
        {
            var existUser = context?.Users.FirstOrDefault(u => u.Email.Equals(user.Email) && u.Password.Equals(user.Password));

            if (existUser != null)
            {
                ViewBag.message = $"{existUser.Name} {existUser.Email}";
                Response.Cookies.Append("AuthorizedUser", existUser.Email);
                return View("Index");
            }
            else
            {
                ViewBag.message = "Invalid Login or Password";
            }

            return View("Index");
        }
        

        public IActionResult Index()
        {

            StartInitial();
            
            if(Request.Cookies.TryGetValue("AuthorizedUser",out var Email))
            {
                var authorizedUser = context?.Users.FirstOrDefault(u => u.Email.Equals(Email));
                if (authorizedUser != null)
                {
                    ViewBag.message = $"{authorizedUser.Name} {authorizedUser.Email}";

                    return View();
                }
            }

            return View();
            

            
        }

        
        public IActionResult About()
        {
            return View();
        }


        public IActionResult Registration()
        {
            

            return View();
        }


        public IActionResult LogOut(int id)
        {
            var user = context?.Users.FirstOrDefault(u => u.Id.Equals(id));
            if (user != null)
            {
                if(Request.Cookies.TryGetValue("AuthorizedUser",out var Email))
                {
                    context?.Users.Remove(user);
                    context?.SaveChangesAsync();

                    Response.Cookies.Delete("AuthorizedUser");
                }

                

                RedirectToAction("Iindex");
            }

            return View("Index");
        }


        public IActionResult Welcome([FromForm]User user)
        {
            var existUser = context?.Users.FirstOrDefault(u => u.Email!.Equals(user.Email));

            if (existUser != null)
            {
                ViewBag.message = $"User {user.Email} already exists!";
                return View("Registration");
            }
            context?.Users.Add(user);
            context?.SaveChanges();
            Response.Cookies.Append("AuthorizedUser", user.Email!);

            return View(user);
        }

        public IActionResult Conference()
        {
            if(Request.Cookies.TryGetValue("AuthorizedUser",out var Email))
            {
                Console.WriteLine(Email);
                var user = context?.Users.FirstOrDefault(u => u.Email!.Equals(Email));

                if (user != null)
                {
                    ViewBag.users = context?.Users.AsNoTracking().ToList();
                    return View("Conference", user);
                }
            }

            ViewBag.message = "For watching a conference you need for registration!";
            return View("Registration");
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
}