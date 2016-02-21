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
using WebGrease.Css.Extensions;

namespace WebRole1.Controllers
{
    public class LocationsController : Controller
    {
        public string addLocation(string name, string longitude, string latitude)
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                if ((name==null)||(db.Locations.Count(i => i.Name == name && i.Longitude == longitude && i.Latitude == latitude) != 0))
                {
                    Response.StatusCode = 400;
                    return JsonConvert.SerializeObject(false);
                }
                db.Locations.InsertOnSubmit(new Location(name,longitude,latitude));
                db.SubmitChanges();
                return JsonConvert.SerializeObject(true);
            }
        }
        // GET: Location
        public string get(string name)
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                var locList = db.Locations.Where(l => l.Name == name);

                return JsonConvert.SerializeObject(locList);
            }
            
        }

        public string getAllLocationsWithImages()
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                var usedLocations = db.Images.GroupBy(t=>t.Location).Select(s => s.First().Location).ToList();
                var locList = db.Locations.Where(l => usedLocations.Contains(l.id));

                return JsonConvert.SerializeObject(locList);
            }
        }

        public string getUserLocations(string username)
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                var userId = db.Persons.First(p => p.Username == username).id;
                var userImagesLocations = db.Images.Where(i => i.OwnerId == userId).GroupBy(i=>i.Location).Select(i=>i.First().Location).ToList();
                var userCities = db.Locations.Where(l => userImagesLocations.Contains(l.id)).Select(l=>l.Name);

                return JsonConvert.SerializeObject(userCities);
            }
        }

        public string getAllLocationsWithPhotoCount()
        {
            using (TimeCapsuleDBDataContext db = new TimeCapsuleDBDataContext())
            {
                Dictionary<string,int> cityCounts = new Dictionary<string, int>();
                var locals = db.Locations.ToList();
                db.Images.GroupBy(t => t.Location).ForEach(ig=>cityCounts.Add(locals.First(p=>p.id==ig.First().Location).Name,ig.Count()));
                List<cityInfo> info = new List<cityInfo>();
                foreach (var local in locals)
                {
                    if (!cityCounts.ContainsKey(local.Name))
                    {
                        info.Add(new cityInfo(local.Name, 0));
                    }
                    else
                    {
                        info.Add(new cityInfo(local.Name,cityCounts[local.Name]));
                    }
                }
                return JsonConvert.SerializeObject(info);
            }
        }
    }

    class cityInfo
    {
        public cityInfo(string name, int count)
        {
            this.Name = name;
            this.Count = count;
        }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}