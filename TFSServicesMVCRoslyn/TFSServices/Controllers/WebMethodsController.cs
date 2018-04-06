using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TFSServices.Models;

namespace TFSServices.Controllers
{
    public class WebMethodsController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: WebMethods
        public ActionResult Index()
        {
            return View(db.WebMethodSet.ToList());
        }

        // GET: WebMethods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebMethod webMethod = db.WebMethodSet.Find(id);
            if (webMethod == null)
            {
                return HttpNotFound();
            }
            return View(webMethod);
        }

        // GET: WebMethods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebMethods/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] WebMethod webMethod)
        {
            if (ModelState.IsValid)
            {
                db.WebMethodSet.Add(webMethod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(webMethod);
        }

        // GET: WebMethods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebMethod webMethod = db.WebMethodSet.Find(id);
            if (webMethod == null)
            {
                return HttpNotFound();
            }
            return View(webMethod);
        }

        // POST: WebMethods/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] WebMethod webMethod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webMethod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(webMethod);
        }

        // GET: WebMethods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebMethod webMethod = db.WebMethodSet.Find(id);
            if (webMethod == null)
            {
                return HttpNotFound();
            }
            return View(webMethod);
        }

        // POST: WebMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebMethod webMethod = db.WebMethodSet.Find(id);
            db.WebMethodSet.Remove(webMethod);
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
