using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using TimeCapsule_Backend;

namespace WebRole1.Controllers
{
    public class PeopleController : Controller
    {
        // GET: People
        public string get(string name)
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                var peopleList = db.Persons.Where(l => l.Username == name);
                
                return JsonConvert.SerializeObject(peopleList);
            }

        }
    }
}
