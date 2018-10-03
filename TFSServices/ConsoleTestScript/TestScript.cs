using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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

        public static void CreateSprintsForEachWeek(TFClientHelper TFClient)
        {
            try
            {
                Dictionary<string, List<string>> TeamProjects = new Dictionary<string, List<string>>();
                DateTime FromDate = new DateTime(2018, 10, 3);
                DateTime ToDate = new DateTime(2018, 10, 31);
                bool CreateNewRoot = true;
                string RootName = "2018";
                int IterationStep = 7;

                TeamProjects.Add("ITService", new List<string>());
                TeamProjects["ITService"].Add("ITService Team");
                TeamProjects["ITService"].Add("Команда 1");
                TeamProjects["ITService"].Add("Команда 2");
                TeamProjects["ITService"].Add("Команда 3");

                if (CreateNewRoot)
                    foreach (string TeamProjectName in TeamProjects.Keys)
                    {
                        TFClient.CreateIteration(TeamProjectName, RootName, DateTime.MinValue, DateTime.MinValue);
                    }

                while (FromDate.DayOfWeek != DayOfWeek.Monday) FromDate = FromDate.AddDays(1);

                while (FromDate < ToDate)
                {
                    DateTime EndIteration = (FromDate.AddDays(IterationStep - 1) > ToDate) ? ToDate : FromDate.AddDays(IterationStep - 1);
                    string IterationName = String.Format("wk. {0:00}.{1:00}-{2:00}.{3:00}", FromDate.Day, FromDate.Month, EndIteration.Day, EndIteration.Month);

                    foreach (string TeamProjectName in TeamProjects.Keys)
                    {
                        var Iteration = TFClient.CreateIteration(TeamProjectName, IterationName, FromDate, EndIteration, RootName);

                        if (TeamProjects[TeamProjectName].Count > 0)
                        {
                            foreach (string TeamName in TeamProjects[TeamProjectName]) TFClient.AddIterationToTeam(TeamProjectName, TeamName, Iteration);
                        }
                    }

                    FromDate = FromDate.AddDays(IterationStep);
                }
            }
            catch (Exception ex)
            {
                ScriptMessage = "Exception";
                ScriptDetailedMessage = ex.Message + "\n" + ex.StackTrace;
            }
        }

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

        public static void Custom(TFClientHelper TFClient)
        {
            try
            {
                string TFProject = "ITService";
                string[] ItemTypes = new string[] { "Requirement", "Task", "Stage" };
                string WiqlTypes = "";
                foreach (string ItemType in ItemTypes)  WiqlTypes += ((WiqlTypes == "") ? "" : ", ") + "'" + ItemType + "'";

                TeamSettingsIteration CurrentIteration = TFClient.GetDefaultTeamCurrentIteration(TFProject);

                if (CurrentIteration == null)
                {
                    ScriptMessage = "Can not get the current iteration";
                    return;
                }

                
                string Wiql = @"SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] ='" + TFProject +
                    @"'  AND  [System.WorkItemType] IN (" + WiqlTypes +
                    ")  AND  [System.IterationPath] <> '" + CurrentIteration.Path +
                    "' AND  [System.State] <> 'Closed'  AND  [Microsoft.VSTS.Scheduling.StartDate] <= '" + CurrentIteration.Attributes.FinishDate.Value.ToShortDateString() +
                    @"' ORDER BY [System.Id]";


                var WiqlResult = TFClient.GetWorkItemListWithWIQL(Wiql, TFProject);

                foreach(WorkItemReference WorkItemRef in WiqlResult.WorkItems)
                {                    
                    Dictionary<string, string> Fields = new Dictionary<string, string>();
                    Fields.Add(TFFields.IterationPath.RefName, CurrentIteration.Path);

                    //WorkItem WI = TFClient.UpdateWorkItem(WorkItemRef.Id, Fields);
                    ScriptDetailedMessage += WorkItemRef.Id + ";";
                }

                if (ScriptDetailedMessage != "")
                {
                    ScriptDetailedMessage = "Updated work items: " + ScriptDetailedMessage;
                    ScriptMessage = "Work items was updated";
                }
                else
                    ScriptMessage = "Without changes";

            }
            catch (Exception ex)
            {
                ScriptMessage = "Exception";
                ScriptDetailedMessage = ex.Message + "\n" + ex.StackTrace;
            }
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

                            if ((string)ChildWorkItem.Fields["System.WorkItemType"] == "Activity")
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
            catch (Exception ex)
            {
                ScriptMessage = "Exception";
                ScriptDetailedMessage = ex.Message + "\n" + ex.StackTrace;
            }
        }
    }
}
