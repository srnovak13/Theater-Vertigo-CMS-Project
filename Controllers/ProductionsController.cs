using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS3.Areas.Prod.Models;
using TheatreCMS3.Models;
using PagedList;

namespace TheatreCMS3.Areas.Prod.Controllers
{
    public class ProductionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Prod/Productions
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            // Keeps track of search filter on different pages
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            // Get productions and filter by searchString
            var productions = from p in db.Productions
                              select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                productions = productions.Where(p => p.Title.Contains(searchString));
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            return View(productions.OrderBy(p => p.Title).ToPagedList(pageNumber, pageSize));
        }

        // GET: Prod/Productions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // GET: Prod/Productions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Prod/Productions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Production production)
        {
            if (ModelState.IsValid)
            {
                db.Productions.Add(production);
                db.SaveChanges();

                // Create new production photo
                ProductionPhoto defaultPhoto = new ProductionPhoto
                {
                    Title = production.Title,
                    Description = production.Description,
                    Image = ProductionPhotosController.FileToBytes(production.File),
                    Production = production,
                    ProductionId = production.ProductionId
                };

                db.ProductionPhotos.Add(defaultPhoto);
                db.SaveChanges();

                production.DefaultPhoto = defaultPhoto;
                production.ProPhotoID = defaultPhoto.ProPhotoId;

                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(production);
        }

        // GET: Prod/Productions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // POST: Prod/Productions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Production production)
        {
            if (ModelState.IsValid)
            {
                // Can't pass DefaultPhoto as a Hidden form, so this is necessary to reset the photo with
                // the ProPhotoID passed through the form
                production.DefaultPhoto = db.ProductionPhotos.Find(production.ProPhotoID);

                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();

                // Update DefaultPhoto's image if a new file was uploaded
                if (production.File != null)
                {
                    byte[] image = ProductionPhotosController.FileToBytes(production.File);
                    production.DefaultPhoto.Image = image;

                    db.Entry(production.DefaultPhoto).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(production);
        }

        // GET: Prod/Productions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // POST: Prod/Productions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Production production = db.Productions.Find(id);
            db.Productions.Remove(production);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}