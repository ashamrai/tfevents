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
    public class ScriptsEngine
    {
        TFSServicesDBLib.DBHelper DBConnection = new TFSServicesDBLib.DBHelper();
        public bool Debug = false;

        public class ScriptWiUpdatedHost
        {
            public WorkItemEvent.WorkItemEventUpdated InputWorkItem { get; set; }
        }

        public class ScriptIntHost
        {
            public int InputWorkItem { get; set; }
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
                    "TFTypesLib"
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
                    "TFTypesLib"
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

    }
}
