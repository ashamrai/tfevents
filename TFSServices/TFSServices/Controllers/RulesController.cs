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
    public class RulesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: Rules
        public ActionResult Index()
        {
            return View(db.RulesSet.Where(r => r.IsDeleted == false).ToList());
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

            return View();
        }

        // POST: Rules/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsActive,Title,Description,TriggerScript,ProcessScript,RuleTypeId")] Rules rules)
        {

            if (ModelState.IsValid)
            {
                db.RulesSet.Add(rules);
                db.SaveChanges();
                AddRevision(rules, "Create");
                return RedirectToAction("Index");
            }

            return View(rules);
        }

        private void AddRevision(Rules pRule, string pOperation)
        {
            Revisions _rev = new Revisions();
            _rev.Date = DateTime.Now;
            _rev.Revision = _rev.Revision;
            _rev.TriggerScript = pRule.TriggerScript;
            _rev.ProcessScript = pRule.ProcessScript;
            _rev.Rules = pRule;
            _rev.UserName = this.User.Identity.Name;
            _rev.Operation = pOperation;
            db.RevisionsSet.Add(_rev);
            db.SaveChanges();
        }

        // GET: Rules/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.RuleTypeList = new SelectList(db.RuleTypeSet, "Id", "Name");

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
        public ActionResult Edit([Bind(Include = "Id,IsActive,Title,Description,TriggerScript,ProcessScript,RuleTypeId")] Rules rules)
        {
            if (ModelState.IsValid)
            {
                rules.Revision++;
                db.Entry(rules).State = EntityState.Modified;
                db.SaveChanges();
                AddRevision(rules, "Edit");
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
            rules.IsDeleted = true;
            rules.IsActive = false;
            db.RulesSet.Remove(rules);
            db.Entry(rules).State = EntityState.Modified;
            db.SaveChanges();
            Revisions _rev = new Revisions();
            AddRevision(rules, "Delete");
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
