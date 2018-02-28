using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TFSServices.Controllers
{
    [Produces("application/json")]
    [Route("api/TS/workitems/created")]
    public class TeamServicesWICreatedController : Controller
    {
        // POST: api/TS/workitems
        [HttpPost]
        public void Post([FromBody]TSEvents.WorkItemEvent.WorkItemEventCreated value)
        {

        }
    }

    [Produces("application/json")]
    [Route("api/TS/workitems/updated")]
    public class TeamServicesWIUpdatedController : Controller
    {
        // POST: api/TS/workitems
        [HttpPost]
        public void Post([FromBody]TSEvents.WorkItemEvent.WorkItemEventUpdated value)
        {

        }
    }
}
