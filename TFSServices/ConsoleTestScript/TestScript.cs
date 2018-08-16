using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFHelper;

namespace ConsoleTestScript
{
    static class TestScript
    {
        static bool ScriptResult = true;
        static string ScriptMessage = "";
        static string ScriptDetailedMessage = "";

        public static void UpdateSuccessors(TFClientHelper TFClient)
        {
            //"System.LinkTypes.Dependency-Reverse"
            string TFProject = "ITService";
            string Wiql = @"SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = '" + TFProject +
                @"'  AND  [Source].[System.WorkItemType] = 'Task'  AND  [Source].[Microsoft.VSTS.Common.Triage] <> 'Info Received') And ([System.Links.LinkType] = 'System.LinkTypes.Dependency-Reverse') And ([Target].[System.WorkItemType] = 'Task'  AND  [Target].[System.State] = 'Closed') ORDER BY [System.Id] mode(MustContain)";

            var WiqlResult = TFClient.GetWorkItemListWithWIQL(Wiql, TFProject);
            var TopLevelIds = TFClient.GetTopLevelWorkItemIds(WiqlResult);

            foreach (int Id in TopLevelIds)
            {
                Dictionary<string, string> Fields = new Dictionary<string, string>();
                Fields.Add("Triage", "Info Received");
                var workItem = TFClient.UpdateWorkItem(Id, Fields);
                ScriptDetailedMessage += Id + ";";
            }

            if (ScriptDetailedMessage != "")
            {
                ScriptDetailedMessage = "Updated work items: " + ScriptDetailedMessage;
                ScriptMessage = "Work items was updated";
            }
            else
                ScriptMessage = "Without changes";

        }

        public static void UpdateParentProposed(TFClientHelper TFClient)
        {
            string TFProject = "ITService";
            string Wiql = @"SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = '" + TFProject +
                @"' AND  [Source].[System.State] = 'Proposed') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.WorkItemType] = 'Task'  AND  [Target].[System.State] = 'Active') ORDER BY [System.Id] mode(MustContain)";

            var WiqlResult = TFClient.GetWorkItemListWithWIQL(Wiql, TFProject);
            var TopLevelIds = TFClient.GetTopLevelWorkItemIds(WiqlResult);

            foreach (int Id in TopLevelIds)
            {
                Dictionary<string, string> Fields = new Dictionary<string, string>();
                Fields.Add("State", "Active");
                var workItem = TFClient.UpdateWorkItem(Id, Fields);
                ScriptDetailedMessage += Id + ";";
            }

            if (ScriptDetailedMessage != "")
            {
                ScriptDetailedMessage = "Updated work items: " + ScriptDetailedMessage;
                ScriptMessage = "Work items was updated";
            }
            else
                ScriptMessage = "Without changes";
        }

        public static void UpdateTaskCompletedFromActivity(TFClientHelper TFClient)
        {
            try
            {
                string TFProject = "ITService";
                DateTime ChahgedDate = DateTime.UtcNow.AddDays(-7);
                string Wiql = @"SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.TeamProject] = '" + TFProject +
                    @"' AND  [Source].[System.WorkItemType] = 'Task') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Target].[System.WorkItemType] = 'Activity'  AND  [Target].[System.ChangedDate] >'" + ChahgedDate.ToShortDateString() +
                    @"') ORDER BY [System.Id] mode(MustContain)";

                var WiqlResult = TFClient.GetWorkItemListWithWIQL(Wiql, TFProject);
                var TopLevelIds = TFClient.GetTopLevelWorkItemIds(WiqlResult);

                foreach (int Id in TopLevelIds)
                {
                    var TaskWorkItem = TFClient.GetWorkItem(Id);
                    bool HaveToUpdate = false;

                    double TasHours = (TaskWorkItem.Fields.Keys.Contains("Microsoft.VSTS.Scheduling.CompletedWork")) ?
                        (double)TaskWorkItem.Fields["Microsoft.VSTS.Scheduling.CompletedWork"] : 0;

                    var ChildLevelIds = TFClient.GetLinkedlWorkItemIds(TaskWorkItem, "System.LinkTypes.Hierarchy-Forward");

                    double NewHours = 0;

                    foreach (int ChildId in ChildLevelIds)
                    {
                        if (ChildId > 0)
                        {
                            var ChildWorkItem = TFClient.GetWorkItem(ChildId);

                            if ("Activity" == (string)ChildWorkItem.Fields["System.WorkItemType"])
                                if (ChildWorkItem.Fields.Keys.Contains("Microsoft.VSTS.Scheduling.CompletedWork"))
                                {
                                    NewHours += (double)ChildWorkItem.Fields["Microsoft.VSTS.Scheduling.CompletedWork"];
                                    HaveToUpdate = true;
                                }
                        }
                    }

                    if (TasHours != NewHours && HaveToUpdate)
                    {
                        Dictionary<string, string> Fields = new Dictionary<string, string>();
                        Fields.Add("Microsoft.VSTS.Scheduling.CompletedWork", NewHours.ToString());
                        var workitem = TFClient.UpdateWorkItem(Id, Fields);
                        ScriptDetailedMessage += Id + ";";
                    }
                }

                if (ScriptDetailedMessage != "")
                {
                    ScriptDetailedMessage = "Updated work items: " + ScriptDetailedMessage;
                    ScriptMessage = "Work items was updated";
                }
                else
                    ScriptMessage = "Without changes";
            }
            catch(Exception ex)
            {
                ScriptMessage = "Exception";
                ScriptDetailedMessage = ex.Message + "\n" + ex.StackTrace;
            }
        }
    }
}
