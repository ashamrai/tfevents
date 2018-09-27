using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFHelper;

namespace TFTaskService
{
    class Program
    {
        public class TaskTimer
        {
            public TFSServicesTypes.Rule Rule;
            public Timer Timer;
        }

        static bool UpdatingRules = false;
        static bool RuningRule = false;
        static int WaterMark = 0;

        static List<TaskTimer> TaskTimers = new List<TaskTimer>();

        static void Main(string[] args)
        {
            Timer _tmupdate = new Timer(UpdateRules, null, 0, 60000);
            Timer _tmhistory = new Timer(ClearHistory, null, 0, 86400000);

            while (true) { Thread.Sleep(60000); };
        }

        static void ClearHistory(object pParam)
        {
            HttpClient _client = new HttpClient();

            try
            {
                _client.BaseAddress = new Uri(Properties.Settings.Default.TFSServicesUrl);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                bool _result = StartRemoteTask(_client, String.Format(Properties.Settings.Default.PathHistoryClear, Properties.Settings.Default.HistoryDaysLeft)).Result;

            }
            catch (Exception _ex)
            {

            }
            finally
            {
                UpdatingRules = false;
                _client.Dispose();
            }
        }

        static void UpdateRules(object pParam)
        {
            HttpClient _client = new HttpClient();

            try
            {
                _client.BaseAddress = new Uri(Properties.Settings.Default.TFSServicesUrl);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                int _watermark = GetWaterMark(_client, Properties.Settings.Default.PathGetWatermark).Result;

                if (_watermark > WaterMark)
                {
                    UpdatingRules = true;

                    while (RuningRule) { }

                    List<TFSServicesTypes.Rule> _rules = GetActiveTasks(_client, Properties.Settings.Default.PathGetActiveTasks).Result;

                    if (_rules != null)
                    {
                        if (TaskTimers.Count == 0)
                            foreach (var _rule in _rules)
                                AddTaskTimer(_rule, "added");
                        else
                        {
                            foreach (var _rule in _rules)
                            {
                                var _ruleToDelete = (from item in TaskTimers where item.Rule.id == _rule.id && item.Rule.rev != _rule.rev select item).FirstOrDefault();

                                if (_ruleToDelete != null)
                                {
                                    _ruleToDelete.Timer.Dispose();
                                    TaskTimers.Remove(_ruleToDelete);
                                    AddTaskTimer(_rule, "updated");
                                }
                            }
                        }
                    }

                    WaterMark = _watermark;

                    UpdatingRules = false;
                }
            }
            catch (Exception _ex)
            {
                
            }
            finally
            {
                UpdatingRules = false;
                _client.Dispose();
            }
        }

        private static void AddTaskTimer(TFSServicesTypes.Rule pRule, string pOperation)
        {
            TaskTimers.Add(new TaskTimer { Rule = pRule, Timer = new Timer(StartTask, pRule.id, GetShiftRun(pRule), GetStepRun(pRule)) });
            Console.WriteLine(String.Format("Rule {0}: id {1}; rev {2}", pOperation, pRule.id, pRule.rev));
        }

        static int GetShiftRun(TFSServicesTypes.Rule pRule)
        {
            int _ret_val = 0;
            int _cur_mins = DateTime.Now.Minute;
            int _cur_hours = DateTime.Now.Hour;

            switch (pRule.period)
            {
                case 0:
                    _ret_val = ((60 - _cur_mins) % pRule.step) * 60000; // minutes
                    break;
                case 1:
                    _ret_val = (((24 - _cur_hours) % pRule.step) * 60 + 60 - _cur_mins) * 60000; //hours
                    break;
            }

            return _ret_val;
        }

        static int GetStepRun(TFSServicesTypes.Rule pRule)
        {
            int _ret_val = 0;

            switch (pRule.period)
            {
                case 0:
                    _ret_val = pRule.step * 60000; //minutes
                    break;
                case 1:
                    _ret_val = pRule.step * 60 * 60000; //hours
                    break;
            }

            return _ret_val;
        }

        static void StartTask(object pTaskId)
        {
            while (UpdatingRules || RuningRule) { }

            RuningRule = true;

            HttpClient _client = new HttpClient();

            try
            {
                int _id = (int)pTaskId;

                if (_id > 0)
                {
                    _client.BaseAddress = new Uri(Properties.Settings.Default.TFSServicesUrl);
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    bool _result = StartRemoteTask(_client, String.Format(Properties.Settings.Default.PathRunTask, _id)).Result;
                }
            }
            catch(Exception _ex)
            {

            }
            finally
            {
                _client.Dispose();
                RuningRule = false;
            }
        }

        static bool StartTasks()
        {
            return true;
        }

        static async Task<int> GetWaterMark(HttpClient pClient, string pPath)
        {
            int _watermark = 0;
            HttpResponseMessage response = await pClient.GetAsync(pPath);
            if (response.IsSuccessStatusCode)
            {
                _watermark = await response.Content.ReadAsAsync<int>();
            }

            return _watermark;
        }

        static async Task<List<TFSServicesTypes.Rule>> GetActiveTasks(HttpClient pClient, string pPath)
        {
            List<TFSServicesTypes.Rule> _rules = new List<TFSServicesTypes.Rule>();

            HttpResponseMessage response = await pClient.GetAsync(pPath);
            if (response.IsSuccessStatusCode)
            {
                _rules = await response.Content.ReadAsAsync<List<TFSServicesTypes.Rule>>();
            }

            return _rules;
        }

        static async Task<bool> StartRemoteTask(HttpClient pClient, string pPath)
        {
            bool _result = false;

            HttpResponseMessage response = await pClient.GetAsync(pPath);
            if (response.IsSuccessStatusCode)
            {
                _result = await response.Content.ReadAsAsync<bool>();
            }

            return _result;
        }
    }
}
