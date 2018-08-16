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

        public WorkItem GetWorkItem(int pId)
        {
            return WitClient.GetWorkItemAsync(pId, expand: WorkItemExpand.Relations).Result;
        }
        public int GetWIIDFromUrl(string pUrl)
        {
            int _id = 0;

            string[] _url_arr = pUrl.Split(new string[] { "_apis/wit/workItems/" }, StringSplitOptions.RemoveEmptyEntries);

            if (_url_arr.Count() == 2)
            {
                if (int.TryParse(_url_arr[1], out _id)) return _id;
                else return 0;
            }

            return 0;
        }

        public WorkItem GetWorkItem(string pUrl)
        {
            int _id = GetWIIDFromUrl(pUrl);

            if (_id > 0)
                return WitClient.GetWorkItemAsync(_id, expand: WorkItemExpand.Relations).Result;
            else
                return null;
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

        public List<int> GetLinkedlWorkItemIds(WorkItem pWorkItem, string pReferenceName)
        {
            return pWorkItem.Relations.Where(p => p.Rel == pReferenceName).Select(i => GetWIIDFromUrl(i.Url)).ToList();
        }
    }
}
