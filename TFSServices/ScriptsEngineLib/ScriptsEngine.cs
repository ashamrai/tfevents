using System;
using System.Collections.Generic;
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
using TFHelper;

namespace ScriptsEngineLib
{
    public class ScriptsEngine
    {
        TFSServicesDBLib.DBHelper DBConnection = new TFSServicesDBLib.DBHelper();
        public bool Debug = false;
        string ServiceUrl = "";
        string PAT = "";

        public class ScriptWiUpdatedHost
        {
            public WorkItemEvent.WorkItemEventUpdated InputWorkItem { get; set; }
        }

        public class ScriptIntHost
        {
            public int InputWorkItem { get; set; }
        }

        public ScriptsEngine(string pServiceUrl, string pPAT)
        {
            ServiceUrl = pServiceUrl;
            PAT = pPAT;
        }

        public  void TestMethod(WorkItemEvent.WorkItemEventUpdated pInputWorkItem)
        {
            try
            {
            }
            catch (Exception ex)
            {

            }
        }


        public bool ProcessWokItemUpdatedEvent(WorkItemEvent.WorkItemEventUpdated pInputWorkItem)
        {
            var _rules = DBConnection.GetRulesByType("WorkItemUpdated");

            if (_rules == null) return false;

            foreach (var _rule in _rules)
            {
                if (CheckRuleWorkItemUpdated(pInputWorkItem, _rule)) RunScriptWorkItemUpdated(pInputWorkItem, _rule);
            }

            return true;
        }

        public bool CheckRuleWorkItemUpdated(WorkItemEvent.WorkItemEventUpdated pInputWorkItem, TFSServicesDBLib.Rules pRule)
        {
            bool _result = false;

            try
            {
                var _scrOpt = ScriptOptions.Default.AddReferences(
                    Assembly.GetAssembly(typeof(WorkItemEvent.WorkItemEventUpdated)),
                    Assembly.GetAssembly(typeof(System.Collections.Generic.Dictionary<string, string>))
                    ).AddImports(
                    "System",
                    "System.Collections.Generic",
                    "TFHelperLib"
                    );

                string _srcHeader = @"bool CheckConditionUpdated(WorkItemEvent.WorkItemEventUpdated InputWorkItem){
";

                string _srcEnd = @"
}
CheckConditionUpdated(InputWorkItem)";

                string _src = _srcHeader + pRule.TriggerScript + _srcEnd;

                _result = CSharpScript.RunAsync<bool>(_src, _scrOpt, new ScriptWiUpdatedHost { InputWorkItem = pInputWorkItem }).Result.ReturnValue;

                if (Debug) DBConnection.AddRunHistory(pRule, "Rule was validated: " + pRule.Title, TypesHelper.Serialize(pInputWorkItem));
            }
            catch (Exception ex)
            {
                if (Debug) DBConnection.AddRunHistory(pRule, "Validation exception for : " + pRule.Title, ex.Message + "\n\n" + ex.StackTrace);
                _result = false;
            }

            return _result;
        }

        public bool RunScriptWorkItemUpdated(WorkItemEvent.WorkItemEventUpdated pInputWorkItem, TFSServicesDBLib.Rules pRule)
        {
            bool _result = false;

            try
            {
                if (Debug) DBConnection.AddRunHistory(pRule, "Start run: " + pRule.Title, TypesHelper.Serialize(pInputWorkItem));

                var _scrOpt = ScriptOptions.Default.AddReferences(
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
                    "TFHelper"
                    );


                string _srcHeader = @"bool ProcessEvent(WorkItemEvent.WorkItemEventUpdated InputWorkItem){";
                string _srcEnd = @"
}
ProcessEvent(InputWorkItem)";

                string _src = _srcHeader + pRule.ProcessScript + _srcEnd;

                _result = CSharpScript.RunAsync<bool>(_src, _scrOpt, new ScriptWiUpdatedHost { InputWorkItem = pInputWorkItem }).Result.ReturnValue;

                if (Debug) DBConnection.AddRunHistory(pRule, "End run: " + pRule.Title, TypesHelper.Serialize(pInputWorkItem));
            }
            catch (Exception ex)
            {
                if (Debug) DBConnection.AddRunHistory(pRule, "Validation exception for : " + pRule.Title, ex.Message + "\n\n" + ex.StackTrace);
                _result = false;
            }

            return _result;
        }


        public bool RunTaskScript(int pTaskId)
        {
            bool _result = false;

            TFSServicesDBLib.Rules _rule = DBConnection.GetRuleById(pTaskId);

            if (_rule == null)
            {
                DBConnection.AddRunHistory(null, "Can not find the rule Id: " + pTaskId, "");
                return false;
            }

            try
            {
                if (Debug) DBConnection.AddRunHistory(_rule, "Start run", "");

                var _scrOpt = ScriptOptions.Default.AddReferences(
                    Assembly.GetAssembly(typeof(WorkItemTrackingHttpClient)),
                    Assembly.GetAssembly(typeof(JsonPatchDocument)),
                    Assembly.GetAssembly(typeof(VssConnection)),
                    Assembly.GetAssembly(typeof(VssBasicCredential)),
                    Assembly.GetAssembly(typeof(Uri)),
                    Assembly.GetAssembly(typeof(TFClientHelper)),
                     Assembly.GetAssembly(typeof(Dictionary<string, string>))
                    ).AddImports(
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi",
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models",
                    "Microsoft.VisualStudio.Services.WebApi.Patch.Json",
                    "Microsoft.VisualStudio.Services.WebApi.Patch",
                    "Microsoft.VisualStudio.Services.WebApi",
                    "Microsoft.VisualStudio.Services.Common",
                    "TFHelper",
                    "System",
                    "System.Collections.Generic"
                    );


                string _srcHeader = @"bool ProcessEvent(){
";
                string _srcGetClients = @"TFClientHelper TFClient = new TFClientHelper(""" + ServiceUrl + @""", """ + PAT + @""");
";
                string _srcEnd = @"
}
ProcessEvent()";

                string _src = _srcHeader + _srcGetClients + _rule.ProcessScript + _srcEnd;

                _result = CSharpScript.RunAsync<bool>(_src, _scrOpt).Result.ReturnValue;

                if (Debug) DBConnection.AddRunHistory(_rule, "End run", "");
            }
            catch (Exception ex)
            {
                if (Debug) DBConnection.AddRunHistory(_rule, "Script exception for : " + _rule.Title, ex.Message + "\n\n" + ex.StackTrace);
                _result = false;
            }

            return _result;
        }
    }
}
