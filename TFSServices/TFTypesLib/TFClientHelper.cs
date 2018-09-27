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
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;

namespace TFHelper
{
    public static class TFFields
    {
        public static TFField TargetDate = new TFField { Name = "Target Date", RefName = "Microsoft.VSTS.Scheduling.TargetDate" };
        public static TFField CompletedWork = new TFField { Name = "Completed Work", RefName = "Microsoft.VSTS.Scheduling.CompletedWork" };
        public static TFField RemainingWork = new TFField { Name = "Remaining Work", RefName = "Microsoft.VSTS.Scheduling.RemainingWork" };
        public static TFField Priority = new TFField { Name = "Priority", RefName = "Microsoft.VSTS.Common.Priority" };
        public static TFField Blocked = new TFField { Name = "Blocked", RefName = "Microsoft.VSTS.CMMI.Blocked" };
        public static TFField WorkItemType = new TFField { Name = "Work Item Type", RefName = "System.WorkItemType" };
        public static TFField Triage = new TFField { Name = "Triage", RefName = "Triage" };
        public static TFField State = new TFField { Name = "State", RefName = "System.State" };
        public static TFField Title = new TFField { Name = "Title", RefName = "System.Title" };
        public static TFField AssignedTo = new TFField { Name = "Assigned To", RefName = "System.AssignedTo" };
        public static TFField IterationPath = new TFField { Name = "Iteration Path", RefName = "System.IterationPath" };
        public static TFField AreaPath = new TFField { Name = "Area Path", RefName = "System.AreaPath" };
        public static TFField LinkParent = new TFField { Name = "Parent", RefName = "System.LinkTypes.Hierarchy-Reverse" };
        public static TFField LinkChild = new TFField { Name = "Child", RefName = "System.LinkTypes.Hierarchy-Forward" };
        public static TFField LinkSuccessor = new TFField { Name = "Successor", RefName = "System.LinkTypes.Dependency-Forward" };
        public static TFField LinkPredecessor = new TFField { Name = "Predecessor", RefName = "System.LinkTypes.Dependency-Forward" };

        public class TFField
        {
            public string Name = "";
            public string RefName = "";
        }
    }

    public class TFClientHelper
    {
        private string ServiceUrl{ get; set; }
        private string PAT { get; set; }

        public WorkItemTrackingHttpClient WitClient { get; set; }
        public WorkHttpClient WorkClient { get; set; }
        public TeamHttpClient TeamClient { get; set; }
        public ProjectHttpClient ProjectClient { get; set; }

        public TFClientHelper(string pServiceUrl, string pPAT)
        {
            ServiceUrl = pServiceUrl;
            PAT = pPAT;

            VssConnection connection = new VssConnection(new Uri(ServiceUrl), new VssBasicCredential(string.Empty, PAT));
            WitClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkClient = connection.GetClient<WorkHttpClient>();
            TeamClient = connection.GetClient<TeamHttpClient>();
            ProjectClient = connection.GetClient<ProjectHttpClient>();
        }

        public WorkItem GetWorkItem(int pId)
        {
            return WitClient.GetWorkItemAsync(pId, expand: WorkItemExpand.All).Result;
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

        public WorkItem CreateWorkItem(string pProjectName, string pWorkItemType, Dictionary<String, String> pFields, string pParentUrl)
        {
            JsonPatchDocument _patchDocument = new JsonPatchDocument();

            foreach (var _key in pFields.Keys) _patchDocument.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/fields/" + _key, Value = pFields[_key] });

            _patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = TFFields.LinkParent.RefName,
                        url = pParentUrl,
                        attributes = new { comment = "From service" }
                    }
                }
            );

            return WitClient.CreateWorkItemAsync(_patchDocument, pProjectName, pWorkItemType).Result;
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


        public TeamSettingsIteration GetDefaultTeamCurrentIteration(string pProjectName)
        {
            var project = ProjectClient.GetProject(pProjectName).Result;

            TeamContext _tmcntx = new TeamContext(project.Id, project.DefaultTeam.Id);

            var _iterations = WorkClient.GetTeamIterationsAsync(_tmcntx).Result;
           
            foreach (var _iteration in _iterations)
                if (DateTime.Now.Date >= _iteration.Attributes.StartDate && DateTime.Now.Date <= _iteration.Attributes.FinishDate)
                    return _iteration;

            return null;
        }
    }
}
