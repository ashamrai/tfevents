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
    public class ScheduleTypesController : Controller
    {
        private TFSServicesDBContainer db = new TFSServicesDBContainer();

        // GET: ScheduleTypes
        public ActionResult Index()
        {
            return View(db.ScheduleTypeSet.ToList());
        }

        // GET: ScheduleTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleType scheduleType = db.ScheduleTypeSet.Find(id);
            if (scheduleType == null)
            {
                return HttpNotFound();
            }
            return View(scheduleType);
        }

        // GET: ScheduleTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ScheduleTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Period,Step")] ScheduleType scheduleType)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleTypeSet.Add(scheduleType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scheduleType);
        }

        // GET: ScheduleTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleType scheduleType = db.ScheduleTypeSet.Find(id);
            if (scheduleType == null)
            {
                return HttpNotFound();
            }
            return View(scheduleType);
        }

        // POST: ScheduleTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Period,Step")] ScheduleType scheduleType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduleType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scheduleType);
        }

        // GET: ScheduleTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleType scheduleType = db.ScheduleTypeSet.Find(id);
            if (scheduleType == null)
            {
                return HttpNotFound();
            }
            return View(scheduleType);
        }

        // POST: ScheduleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleType scheduleType = db.ScheduleTypeSet.Find(id);
            db.ScheduleTypeSet.Remove(scheduleType);
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
