using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MvcClient.Models;
using Repository;

namespace MvcClient.Controllers
{
    public class AccountController : Controller
    {
        private IMongoDatabase _db;

        public AccountController()
        {
            _db = new MongoClient().GetDatabase("NotesDB");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                string error = string.Empty;

                foreach (var modelState in ModelState.Values)                
                    foreach (var err in modelState.Errors)                   
                        error += string.Format("{0} ", err.ErrorMessage);                   
                
                ViewBag.error = error;
                return View(model);
            }

            var hashResult = Helpers.EncryptPassword(model.Password);

            var user = new UserModel
            {
                Email = model.Email,
                Name = model.Name,
                HashedPassword = hashResult.Hash,
                SecuryStamp = hashResult.SecurityStamp
            };

            try
            { 
                await Repository.MongoCRUD.Insert("Users", user, _db);
            }
            catch
            {
                ViewBag.error = "user taken";
                return View(model);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}