using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreCMS3.Models;
using System.Data.Entity;
using TheatreCMS3.Areas.Prod.Models;
using System.Net;
using System.IO;

namespace TheatreCMS3.Areas.Prod.Controllers
{
    [CustomAuthorize(Roles = "Production Photographer")]
    public class ProductionPhotosController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Prod/ProductionPhotos
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.ProductionList = db.Productions;

            return View(db.ProductionPhotos.ToList());
        }

        // GET: Prod/ProductionPhotos/Upload
        public ActionResult Upload()
        {
            ViewBag.ProductionList = GetProductionList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(ProductionPhoto photo)
        {
            // Convert uploaded file to byte[]
            photo.Image = FileToBytes(photo.File);

            // Set production from chosen Id
            photo.Production = db.Productions.Find(photo.ProductionId);

            // If form is filled out correctly, add new photo to database and redirect to index
            if (ModelState.IsValid)
            {
                db.ProductionPhotos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductionList = GetProductionList();
            return View(photo);
        }

        // GET: Prod/ProductionPhotos/Edit/5
        public ActionResult Edit(int? id)
        {
            // If no ID provided, return bad request code
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find photo in database by ID
            ProductionPhoto photo = db.ProductionPhotos.Find(id);

            // If photo is null, return HttpNotFound
            if (photo == null)
                return HttpNotFound();

            ViewBag.ProductionList = GetProductionList();

            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductionPhoto photo)
        {
            // Update production with production that was selected
            photo.Production = db.Productions.Find(photo.ProductionId);

            // Convert uploaded file to byte[] if new photo is uploaded
            if (photo.File != null)
                photo.Image = FileToBytes(photo.File);

            // If form is filled out correctly, save changes to database and redirect to Index
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductionList = GetProductionList();
            return View(photo);
        }

        // GET: Prod/ProductionPhotos/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProductionPhoto photo = db.ProductionPhotos.Find(id);

            if (photo == null)
                return HttpNotFound();

            return View(photo);
        }

        // GET: Prod/ProductionPhotos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProductionPhoto photo = db.ProductionPhotos.Find(id);

            if (photo == null)
                return HttpNotFound();

            return View(photo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductionPhoto photo = db.ProductionPhotos.Find(id);
            db.ProductionPhotos.Remove(photo);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Takes an HttpPostedFileBase and converts it to a byte array
        [AllowAnonymous]
        public static byte[] FileToBytes(HttpPostedFileBase file)
        {
            byte[] bytes;

            using (BinaryReader br = new BinaryReader(file.InputStream))
                bytes = br.ReadBytes(file.ContentLength);

            return bytes;
        }

        // Takes an ID of a ProductionPhoto and returns an image from its Photo byte[]
        //
<img src="/ProductionPhotos/GetImage/5" />
        [AllowAnonymous]
        public ActionResult GetImage(int id)
        {
            byte[] bytes = db.ProductionPhotos.Find(id).Image;

            if (bytes == null)
                return null;

            return File(bytes, "image/jpeg");
        }

        // Return a list of productions to populate dropdownlist form inputs
        [AllowAnonymous]
        public List<SelectListItem>
  GetProductionList()
  {
  var productions = new List<SelectListItem>
    ();

    foreach (Production production in db.Productions)
    {
    var item = new SelectListItem
    {
    Text = production.Title,
    Value = production.ProductionId.ToString()
    };
    productions.Add(item);
    }

    return productions;
    }
    }
    }
