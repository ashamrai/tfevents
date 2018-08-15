using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TFSServicesDBLib;

namespace TFSServices.Controllers
{
    public class RunHistoriesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: RunHistories
        public ActionResult Index(int? page)
        {
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            var runHistorySet = db.RunHistorySet.Include(r => r.Rules).OrderByDescending(h => h.Date);
            return View(runHistorySet.ToPagedList(pageNumber, pageSize));
        }

        // GET: RunHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RunHistory runHistory = db.RunHistorySet.Find(id);
            if (runHistory == null)
            {
                return HttpNotFound();
            }
            return View(runHistory);
        }

        // GET: RunHistories/Create
        public ActionResult Create()
        {
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title");
            return View();
        }

        // POST: RunHistories/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Result,RulesId,RuleRevision,Message")] RunHistory runHistory)
        {
            if (ModelState.IsValid)
            {
                db.RunHistorySet.Add(runHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", runHistory.RulesId);
            return View(runHistory);
        }

        // GET: RunHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RunHistory runHistory = db.RunHistorySet.Find(id);
            if (runHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", runHistory.RulesId);
            return View(runHistory);
        }

        // POST: RunHistories/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Result,RulesId,RuleRevision,Message")] RunHistory runHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(runHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RulesId = new SelectList(db.RulesSet, "Id", "Title", runHistory.RulesId);
            return View(runHistory);
        }

        // GET: RunHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RunHistory runHistory = db.RunHistorySet.Find(id);
            if (runHistory == null)
            {
                return HttpNotFound();
            }
            return View(runHistory);
        }

        // POST: RunHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RunHistory runHistory = db.RunHistorySet.Find(id);
            db.RunHistorySet.Remove(runHistory);
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
