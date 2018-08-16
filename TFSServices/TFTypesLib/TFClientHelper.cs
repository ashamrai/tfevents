using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace TFHelper
{
    public class TFClientHelper
    {
        private string ServiceUrl{ get; set; }
        private string PAT { get; set; }

        public WorkItemTrackingHttpClient WitClient { get; set; }

        public TFClientHelper(string pServiceUrl, string pPAT)
        {
            ServiceUrl = pServiceUrl;
            PAT = pPAT;

            VssConnection connection = new VssConnection(new Uri(ServiceUrl), new VssBasicCredential(string.Empty, PAT));
            WitClient = connection.GetClient<WorkItemTrackingHttpClient>();
        }

        public WorkItem UpdateWorkItem(int pId, Dictionary<string, string> pFields)
        {
            JsonPatchDocument _patchDocument = new JsonPatchDocument();

            foreach (var _key in pFields.Keys) _patchDocument.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/fields/" + _key, Value = pFields[_key] });

            return WitClient.UpdateWorkItemAsync(_patchDocument, pId).Result;
        }

        public WorkItemQueryResult GetWorkItemListWithWIQL(string pWiql, string pProjectName)
        {
            Wiql _wiql = new Wiql();
            _wiql.Query = pWiql;

            return WitClient.QueryByWiqlAsync(_wiql, pProjectName).Result;
        }

        public List<int> GetTopLevelWorkItemIds(WorkItemQueryResult pResult)
        {
            return pResult.WorkItemRelations.Where(p => p.Source == null).Select(i => i.Target.Id).ToList();
        }

        public List<int> GetLinkedlWorkItemIds(WorkItemQueryResult pResult, int pId, string pReferenceName = "")
        {
            return (pReferenceName == "") ?
                pResult.WorkItemRelations.Where(p => p.Source.Id == pId).Select(i => i.Target.Id).ToList() :
                pResult.WorkItemRelations.Where(p => p.Source.Id == pId && p.Rel == pReferenceName).Select(i => i.Target.Id).ToList();
        }
    }
}
