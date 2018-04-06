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
    public class RulesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: Rules
        public ActionResult Index()
        {
            return View(db.RulesSet.ToList());
        }

        // GET: Rules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.RulesSet.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // GET: Rules/Create
        public ActionResult Create()
        {
            ViewBag.RuleTypeList = new SelectList(db.RuleTypeSet, "Id", "Name");
            ViewBag.WebMethodsList = new SelectList(db.WebMethodSet, "Id", "Name");

            return View();
        }

        // POST: Rules/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsActive,Title,Description,TriggerScript,ProcessScript,RuleTypeId,WebMethodId")] Rules rules)
        {

            if (ModelState.IsValid)
            {
                db.RulesSet.Add(rules);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rules);
        }

        // GET: Rules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.RulesSet.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // POST: Rules/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IsActive,Title,Description,TriggerScript,ProcessScript")] Rules rules)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rules).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rules);
        }

        // GET: Rules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rules rules = db.RulesSet.Find(id);
            if (rules == null)
            {
                return HttpNotFound();
            }
            return View(rules);
        }

        // POST: Rules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rules rules = db.RulesSet.Find(id);
            db.RulesSet.Remove(rules);
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
