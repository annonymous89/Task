using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace MvcClient.Controllers
{
    public class NotesController : Controller
    {
        public async Task<IActionResult> ReadAll()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            string result = string.Empty;
            
            try
            {
                result = await client.GetStringAsync("http://localhost:5001/notes");
            }
            catch
            {
                return SignOut("Cookies", "oidc");
            }
          
            var notes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NoteModel>>(result);

            var noteModels = notes
                .Select(c => new Repository.NoteViewModel { Content = c.Content, Name = c.Name })
                .ToList();
            
            
            return View(new Repository.NotesViewModel { Notes = noteModels });
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new NoteModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(NoteModel model)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json);
            
            try
            {
                var result = await client.PostAsync("http://localhost:5001/notes", stringContent);
            }
            catch
            {
                return SignOut("Cookies", "oidc");
            }

            return RedirectToAction(nameof(NotesController.ReadAll));
        }
    }
}