using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

// for exception with System.Collections.Immutable https://github.com/aspnet/EntityFrameworkCore/issues/6220

namespace TFSServices.Controllers
{
    [Route("api/TS/workitems/created")]
    public class TeamServicesWICreatedController : ApiController
    {
        // POST: api/TS/workitems/created
        [HttpPost]
        public void Post([FromBody]TFTypesLib.WorkItemEvent.WorkItemEventCreated value)
        {

        }
    }

    [Route("api/TS/workitems/updated")]
    public class TeamServicesWIUpdatedController : ApiController
    {
        // POST: api/TS/workitems/updated
        [HttpPost]
        public void Post([FromBody]TFTypesLib.WorkItemEvent.WorkItemEventUpdated value)
        {
            ScriptsEngineLib.ScriptsEngine.ProcessWokItemUpdatedEvent(value);
        }
    }
}
