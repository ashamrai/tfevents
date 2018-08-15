using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using ScriptsEngineLib;
using TFHelper;

namespace ConsoleTestScript
{
    class Program
    {
        static void Main(string[] args)
        {
            TFClientHelper TFClient = new TFClientHelper(@"", @"");
            TestPredecessorScript(TFClient);
        }

        static bool TestPredecessorScript(TFClientHelper TFClient)
        {
            //"System.LinkTypes.Dependency-Reverse"
            string TFProject = "ITService";
            string Wiql = @"SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = '" + TFProject + 
                @"'  AND  [Source].[System.WorkItemType] = 'Task'  AND  [Source].[Microsoft.VSTS.Common.Triage] <> 'Info Received') And ([System.Links.LinkType] = 'System.LinkTypes.Dependency-Reverse') And ([Target].[System.WorkItemType] = 'Task'  AND  [Target].[System.State] = 'Closed') ORDER BY [System.Id] mode(MustContain)";

            var WiqlResult = TFClient.GetWorkItemListWithWIQL(Wiql, TFProject);
            var TopLevelIds = TFClient.GetTopLevelWorkItemIds(WiqlResult);

            foreach(int Id in TopLevelIds)
            {
                Dictionary<string, string> Fields = new Dictionary<string, string>();
                Fields.Add("Triage", "Info Received");
                Fields.Add("System.History", "From service");
                var workItem = TFClient.UpdateWorkItem(Id, Fields);
            }

            return true;
        }
    }
}
