using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using TFHelper;
using ScriptsPluginManagerLib;
using Newtonsoft.Json;

namespace ScriptsEngineLib
{
    public class ScriptsEngine
    {
        public class Globals
        {
            public Dictionary<string, string> CustomSettings;
        }

        public Dictionary<string, string> CustomSettings { get; set; }


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
                var _scrOpt = GetOptions();

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

                var _scrOpt = GetOptions();


                string _srcHeader = @"string ProcessEvent(WorkItemEvent.WorkItemEventUpdated InputWorkItem){";
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
            string _scriptMessage = "";
            string _scriptDetailedMessage = "";

            TFSServicesDBLib.Rules _rule = DBConnection.GetRuleById(pTaskId);

            if (_rule == null)
            {
                DBConnection.AddRunHistory(null, "Can not find the rule Id: " + pTaskId, "");
                return false;
            }

            try
            {
                if (Debug) DBConnection.AddRunHistory(_rule, "Start run", "");

                var _globals = new Globals { /*ScriptResult = true, ScriptMessage = "",*/ CustomSettings = CustomSettings };

                var _scrOpt = GetOptions();


                string _srcHeader = @"bool ScriptResult = true;
string ScriptMessage = """";
string ScriptDetailedMessage = """";

void ProcessEvent(){
try
{
PluginManager PluginManager = new PluginManager();
PluginManager.Initialize();
";
                string _srcGetClients = @"TFClientHelper TFClient = new TFClientHelper(""" + ServiceUrl + @""", """ + PAT + @""");
";
                string _srcEnd = @"
}
catch(Exception ex)
{
    ScriptMessage = ""Exception"";
    ScriptDetailedMessage = ex.Message + ""\n"" + ex.StackTrace;
}
}
ProcessEvent()";

                string _src = _srcHeader + _srcGetClients + _rule.ProcessScript + _srcEnd;

                var _scriptResult = CSharpScript.RunAsync<bool>(_src, _scrOpt, _globals).Result;

                if ( _scriptResult.Variables.Length == 3)
                {
                    _result = (bool)_scriptResult.Variables[0].Value;
                    _scriptMessage = (string)_scriptResult.Variables[1].Value;
                    _scriptDetailedMessage = (string)_scriptResult.Variables[2].Value;

                    if (Debug) DBConnection.AddRunHistory(_rule, "End run: " + _scriptMessage, _scriptDetailedMessage);
                    else DBConnection.AddRunHistory(_rule, (_result)? "Success" : "Fail", _scriptMessage + "\n\n" + _scriptDetailedMessage);
                }
                else
                    DBConnection.AddRunHistory(_rule, "Script did not return all globals", "");

            }
            catch (Exception ex)
            {
                DBConnection.AddRunHistory(_rule, "Script exception", ex.Message + "\n\n" + ex.StackTrace);
                _result = false;
            }

            return _result;
        }

        ScriptOptions GetOptions()
        {
            return ScriptOptions.Default.AddReferences(
                    Assembly.GetAssembly(typeof(TeamSettingsIteration)),
                    Assembly.GetAssembly(typeof(WorkItemTrackingHttpClient)),
                    Assembly.GetAssembly(typeof(Exception)),
                    Assembly.GetAssembly(typeof(WorkItem)),
                    Assembly.GetAssembly(typeof(JsonPatchDocument)),
                    Assembly.GetAssembly(typeof(VssConnection)),
                    Assembly.GetAssembly(typeof(VssBasicCredential)),
                    Assembly.GetAssembly(typeof(Uri)),
                    Assembly.GetAssembly(typeof(TFClientHelper)),
                    Assembly.GetAssembly(typeof(PluginManager))
                    ).AddImports(
                    "Microsoft.TeamFoundation.Work.WebApi",
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi",
                    "Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models",
                    "Microsoft.VisualStudio.Services.WebApi.Patch.Json",
                    "Microsoft.VisualStudio.Services.WebApi.Patch",
                    "Microsoft.VisualStudio.Services.WebApi",
                    "Microsoft.VisualStudio.Services.Common",
                    "TFHelper",
                    "ScriptsPluginManagerLib",
                    "System",
                    "System.Exception",
                    "System.Collections.Generic"
                    );
        }

        public bool SetCustomSettings(string SettingsJson)
        {
            Dictionary<string, string> _settings;
            try
            {
                if (SettingsJson == "") return true;

                _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(SettingsJson);
            }
            catch(Exception ex)
            {
                DBConnection.AddRunHistory(null, "Can not parse custom settings", ex.Message + "\n\n" + ex.StackTrace);
                return false;
            }

            CustomSettings = _settings;

            return true;
        }
    }
}
