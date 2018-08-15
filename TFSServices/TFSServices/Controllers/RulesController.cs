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
    public class RulesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: Rules
        public ActionResult Index(int? page)
        {
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(db.RulesSet.Where(r => r.IsDeleted == false).OrderBy(o => o.Id).ToPagedList(pageNumber, pageSize));
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
            ViewBag.ScheduleTypeList = new SelectList(db.ScheduleTypeSet, "Id", "Name");

            return View();
        }

        // POST: Rules/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,IsActive,Title,Description,TriggerScript,ProcessScript,RuleTypeId,ScheduleTypeId")] Rules rules)
        {

            if (ModelState.IsValid)
            {
                DBHelper _dBHelper = new DBHelper(db);

                int _curwaterm = _dBHelper.GetCurrentWatermark();

                rules.Revision = 1;
                rules.Watermark = _curwaterm + 1;
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
            _rev.Revision = pRule.Revision;
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
            ViewBag.ScheduleTypeList = new SelectList(db.ScheduleTypeSet, "Id", "Name");

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
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Revision,IsActive,Title,Description,TriggerScript,ProcessScript,RuleTypeId,ScheduleTypeId")] Rules rules)
        {
            if (ModelState.IsValid)
            {
                DBHelper _dBHelper = new DBHelper(db);
                int _curwaterm = _dBHelper.GetCurrentWatermark();

                rules.Watermark = _curwaterm + 1;
                rules.Revision = rules.Revision + 1;
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
            DBHelper _dBHelper = new DBHelper(db);
            int _curwaterm = _dBHelper.GetCurrentWatermark();
            
            Rules rules = db.RulesSet.Find(id);
            rules.IsDeleted = true;
            rules.IsActive = false;
            rules.Watermark = _curwaterm + 1;
            //db.RulesSet.Remove(rules);
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

        // GET: Rules/Delete/5
        public ActionResult Run(int? id)
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

        // POST: Rules/Run/5
        [HttpPost, ActionName("Run")]
        [ValidateAntiForgeryToken]
        public ActionResult RunConfirmed(int id)
        {
            var _srcs = new ScriptsEngineLib.ScriptsEngine(Properties.Settings.Default.ServiceUrl, Properties.Settings.Default.PAT);
            _srcs.Debug = Properties.Settings.Default.Debug;

            _srcs.RunTaskScript(id);

            return RedirectToAction("Index", "RunHistories");
        }
    }
}
