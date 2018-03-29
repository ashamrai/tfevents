using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using TFTypesLib;

namespace ScriptsEngineLib
{
    public static class ScriptsEngine
    {
        public class ScriptWiUpdatedHost
        {
            public WorkItemEvent.WorkItemEventUpdated InputWorkItem { get; set; }
        }

        public class ScriptIntHost
        {
            public int InputWorkItem { get; set; }
        }

        private const string Xml_Marker = "$xml_string$";
        private const string WorkItemEventType_Marker = "$workitem_event_type$";

        public static void TestMethod(WorkItemEvent.WorkItemEventUpdated pInputWorkItem)
        {
            try
            {
                RunScript(pInputWorkItem);
            }
            catch(Exception ex)
            {

            }
        }

        public static bool CheckRule(WorkItemEvent.WorkItemEventUpdated pInputWorkItem)
        {
            bool _result = false;

            try
            {
                var ScrOpt = ScriptOptions.Default.AddReferences(
                    Assembly.GetAssembly(typeof(WorkItemEvent.WorkItemEventUpdated))
                    ).AddImports(
                    "System",
                    "TFTypesLib"
                    );


                string _src = @"bool CheckConditionUpdated(WorkItemEvent.WorkItemEventUpdated InputWorkItem)
                            {
	                            if (InputWorkItem == null) return false;

                                if (InputWorkItem.resource.fields.ContainsKey(""System.History"") &&
                                    InputWorkItem.resource.fields[""System.History""].newValue == ""Changes from script"") return false;

                                if (InputWorkItem.resource.revision.fields[""System.WorkItemType""] == ""Bug"") return true;

                                return false;
                            }

                            CheckConditionUpdated(InputWorkItem)";

                _result = CSharpScript.RunAsync<bool>(_src, ScrOpt, new ScriptWiUpdatedHost { InputWorkItem = pInputWorkItem }).Result.ReturnValue;
            }
            catch (Exception ex) { }

            return _result;
        }

        public static bool RunScript(WorkItemEvent.WorkItemEventUpdated pInputWorkItem)
        {
            bool _result = false;

            try
            {
                var ScrOpt = ScriptOptions.Default.AddReferences(
                    Assembly.GetAssembly(typeof(WorkItemTrackingHttpClient)),
                    Assembly.GetAssembly(typeof(JsonPatchDocument)),
                    Assembly.GetAssembly(typeof(VssCredentials)),
                    Assembly.GetAssembly(typeof(WorkItemEvent.WorkItemEventUpdated)),
                    Assembly.GetAssembly(typeof(Uri))
                    ).AddImports(
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi",
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models",
                    "Microsoft.VisualStudio.Services.WebApi.Patch.Json",
                    "Microsoft.VisualStudio.Services.WebApi.Patch",
                    "Microsoft.VisualStudio.Services.Common",
                    "System",
                    "TFTypesLib"
                    );


                string str = @"bool ProcessEvent(WorkItemEvent.WorkItemEventUpdated InputWorkItem)
                {
                    if (InputWorkItem == null) return false;

                    JsonPatchDocument PatchDocument = new JsonPatchDocument();

                    PatchDocument.Add(
                        new JsonPatchOperation()
                        {
                            Operation = Operation.Add,
                            Path = ""/fields/System.History"",
                            Value = ""Changes from script""
                        }
                    );

                    VssCredentials Cred = new VssCredentials(true);
                    WorkItemTrackingHttpClient WIClient = new WorkItemTrackingHttpClient(new Uri(""http://tfs-srv:8080/tfs/DefaultCollection""), Cred);
                    WorkItem result = WIClient.UpdateWorkItemAsync(PatchDocument, InputWorkItem.resource.revision.id).Result; 

                    return true;
                }

                ProcessEvent(InputWorkItem);";

                _result = CSharpScript.RunAsync<bool>(str, ScrOpt, new ScriptWiUpdatedHost { InputWorkItem = pInputWorkItem }).Result.ReturnValue;
            }
            catch (Exception ex)
            { }

            return _result;
        }

    }
}
