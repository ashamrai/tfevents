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
    public class RuleTypesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: RuleTypes
        public ActionResult Index()
        {
            return View(db.RuleTypeSet.ToList());
        }

        // GET: RuleTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RuleType ruleType = db.RuleTypeSet.Find(id);
            if (ruleType == null)
            {
                return HttpNotFound();
            }
            return View(ruleType);
        }

        // GET: RuleTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RuleTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IsEvent,HasSchedule")] RuleType ruleType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RuleTypeSet.Add(ruleType);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch(Exception ex)
            { }

            return View(ruleType);
        }

        // GET: RuleTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RuleType ruleType = db.RuleTypeSet.Find(id);
            if (ruleType == null)
            {
                return HttpNotFound();
            }
            return View(ruleType);
        }

        // POST: RuleTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IsEvent,HasSchedule")] RuleType ruleType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ruleType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ruleType);
        }

        // GET: RuleTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RuleType ruleType = db.RuleTypeSet.Find(id);
            if (ruleType == null)
            {
                return HttpNotFound();
            }
            return View(ruleType);
        }

        // POST: RuleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RuleType ruleType = db.RuleTypeSet.Find(id);
            db.RuleTypeSet.Remove(ruleType);
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
