using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TFHelper;

// for exception with System.Collections.Immutable https://github.com/aspnet/EntityFrameworkCore/issues/6220

namespace TFSServices.Controllers
{
    [Route("api/TS/workitems/created")]
    public class TeamServicesWICreatedController : ApiController
    {
        // POST: api/TS/workitems/created
        [HttpPost]
        public void Post([FromBody]TFHelper.WorkItemEvent.WorkItemEventCreated value)
        {

        }
    }

    [Route("api/TS/Tasks/runtask/{pTaskId}")]
    public class TeamServicesTaskRunController : ApiController
    {
        // POST: api/TS/Tasks/runtask/{id}
        [HttpGet]
        public bool Get([FromUri] int pTaskId)
        {
            var _srcs = new ScriptsEngineLib.ScriptsEngine(Properties.Settings.Default.ServiceUrl, Properties.Settings.Default.PAT);
            _srcs.Debug = Properties.Settings.Default.Debug;
            
            _srcs.RunTaskScript(pTaskId);

            return true;
        }
    }

    [Route("api/TS/Tasks/currentrevision")]
    public class CurrentRevisionController : ApiController
    {
        // POST: api/TS/workitems/created
        [HttpGet]
        public int Get()
        {
            TFSServicesDBLib.DBHelper _dBHelper = new TFSServicesDBLib.DBHelper();
            
            return _dBHelper.GetCurrentWatermark(); 
        }
    }

    [Route("api/TS/Tasks/scheduledrules")]
    public class ScheduledRulesController : ApiController
    {
        // POST: api/TS/workitems/created
        [HttpGet]
        public List<TFSServicesTypes.Rule> Get()
        {
            List<TFSServicesTypes.Rule> _ret_rules = new List<TFSServicesTypes.Rule>();

            TFSServicesDBLib.DBHelper _dBHelper = new TFSServicesDBLib.DBHelper();

            var _rules = _dBHelper.GetScheduledRules();

            if (_rules == null) return null;

            foreach (var _rule in _rules)
                _ret_rules.Add(new TFSServicesTypes.Rule { id = _rule.Id, rev = _rule.Revision, period = _rule.ScheduleType.Period, step = _rule.ScheduleType.Step });

            return _ret_rules;
        }
    }

    [Route("api/TS/workitems/updated")]
    public class TeamServicesWIUpdatedController : ApiController
    {
        // POST: api/TS/workitems/updated
        [HttpPost]
        public void Post([FromBody]TFHelper.WorkItemEvent.WorkItemEventUpdated value)
        {
            var _srcs = new ScriptsEngineLib.ScriptsEngine(Properties.Settings.Default.ServiceUrl, Properties.Settings.Default.PAT);
            _srcs.Debug = Properties.Settings.Default.Debug;
            _srcs.ProcessWokItemUpdatedEvent(value);
        }
    }
}
