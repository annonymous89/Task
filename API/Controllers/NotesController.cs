using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Repository;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private IMongoDatabase _db;

        public NotesController()
        {
            _db = new MongoClient().GetDatabase("NotesDB");                   
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = await GetUserId();

            var notes = await Repository.MongoCRUD.LoadAllForUser<NoteModel>("Notes", _db, userId);
            return new JsonResult(notes);
        }

        [HttpPost]
        public async Task<IActionResult> Add()
        {
            var userId = await GetUserId();
            NoteModel note;

            using (var s = new StreamReader(Request.Body))
            {
                var json = await s.ReadToEndAsync();
                note = Newtonsoft.Json.JsonConvert.DeserializeObject<NoteModel>(json);
                note.UserId = userId;
            }

            if (note == null)
                return null;

            await Repository.MongoCRUD.Insert("Notes", note, _db);
            return new JsonResult(null);
        }

        private async Task<Guid> GetUserId()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var _token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            var userId = _token.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            return new Guid(userId);
        }
    }
}