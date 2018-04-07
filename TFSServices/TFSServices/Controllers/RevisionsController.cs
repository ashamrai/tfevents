using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TFSServicesDBLib;

namespace TFSServices.Controllers
{
    public class RevisionsController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: Revisions
        public ActionResult Index()
        {
            var revisionsSet = db.RevisionsSet.Include(r => r.Rules);
            return View(revisionsSet.ToList());
        }

        // GET: Revisions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revisions revisions = db.RevisionsSet.Find(id);
            if (revisions == null)
            {
                return HttpNotFound();
            }
            return View(revisions);
        }

        // GET: Revisions/Create
        public ActionResult Create()
        {
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title");
            return View();
        }

        // POST: Revisions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,UserName,TriggerScript,ProcessScript,RulesId,Revision,Operation")] Revisions revisions)
        {
            if (ModelState.IsValid)
            {
                db.RevisionsSet.Add(revisions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", revisions.RulesId);
            return View(revisions);
        }

        // GET: Revisions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revisions revisions = db.RevisionsSet.Find(id);
            if (revisions == null)
            {
                return HttpNotFound();
            }
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", revisions.RulesId);
            return View(revisions);
        }

        // POST: Revisions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,UserName,TriggerScript,ProcessScript,RulesId,Revision,Operation")] Revisions revisions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revisions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", revisions.RulesId);
            return View(revisions);
        }

        // GET: Revisions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revisions revisions = db.RevisionsSet.Find(id);
            if (revisions == null)
            {
                return HttpNotFound();
            }
            return View(revisions);
        }

        // POST: Revisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Revisions revisions = db.RevisionsSet.Find(id);
            db.RevisionsSet.Remove(revisions);
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
