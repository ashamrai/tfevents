using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace TFServices
{
    public enum WIEventType {WICREATED, WIUPDATED};

    public class EventService : IEventService
    {
        public void WorkItemChangedEvent(Stream pEventData)
        {
            StreamReader _reader = new StreamReader(pEventData, Encoding.UTF8);
            string _eventStr = _reader.ReadToEnd();

            WorkItemEventCore _wieventcorre = JsonConvert.DeserializeObject<WorkItemEventCore>(_eventStr);
            WorkItemEvent _wievent = new WorkItemEvent();
            _wievent.Core = _wieventcorre;

            if (_wieventcorre.EventType == WIEventType.WIUPDATED)
            {
                WorkItemEventUpdated _wieventUpdated = JsonConvert.DeserializeObject<WorkItemEventUpdated>(_eventStr);
                _wievent.UpdatedFields = _wieventUpdated.resource.fields;
                _wievent.UpdatedRelations = _wieventUpdated.resource.relations;
                _wievent.RevisedBy = _wieventUpdated.resource.revisedBy;
                _wievent.Fields = _wieventUpdated.resource.revision.fields;
                _wievent.Relations = _wieventUpdated.resource.revision.relations;
                _wievent.id = _wieventUpdated.resource.revision.id;
                _wievent.rev = _wieventUpdated.resource.revision.rev;
                _wievent._links = _wieventUpdated.resource._links;
            }
            else if (_wieventcorre.EventType == WIEventType.WICREATED)
            {
                WorkItemEventCreated _wieventCreated = JsonConvert.DeserializeObject<WorkItemEventCreated>(_eventStr);
                _wievent.Fields = _wieventCreated.resource.fields;
                _wievent.Relations = _wieventCreated.resource.relations;
                _wievent.id = _wieventCreated.resource.id;
                _wievent.rev = _wieventCreated.resource.rev;
                _wievent._links = _wieventCreated.resource._links;
            }

            if (_wievent.Fields.Count > 0)
                ProgressEvent(_wievent);
        }

        private void ProgressEvent(WorkItemEvent wievent)
        {
            
        }
    }
}
