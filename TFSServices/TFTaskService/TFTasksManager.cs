using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFHelper;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace TFTaskService
{
    class TFTasksManager
    {
        public class TaskTimer
        {
            public TFSServicesTypes.Rule Rule;
            public Timer Timer;
        }

        bool UpdatingRules = false;
        bool RuningRule = false;
        int WaterMark = 0;

        Timer _tmupdate, _tmhistory;

        List<TaskTimer> TaskTimers = new List<TaskTimer>();

        public void Start()
        {
            _tmupdate = new Timer(UpdateRules, null, 0, 60000);
            _tmhistory = new Timer(ClearHistory, null, 0, 86400000);
        }

        public void Stop()
        {
            _tmupdate.Dispose();
            _tmhistory.Dispose();
        }

        void ClearHistory(object pParam)
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
            catch (Exception ex)
            {
                WriteLog(ex.Message, ex.StackTrace);
            }
            finally
            {
                UpdatingRules = false;
                _client.Dispose();
            }
        }

        void WriteLog(string Message, string DetailedMessage = null)
        {
            string filename = String.Format("Logs\\{0}-{1:d2}-{2:d2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string time = String.Format("{0:d2}:{1:d2}:{2:d2} ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            File.AppendAllText(filename, time + Message + "\r\n");
            if (DetailedMessage != null) File.AppendAllText(filename, DetailedMessage + "\r\n");
        }

        void UpdateRules(object pParam)
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
                                var RuleToUpdate = (from item in TaskTimers where item.Rule.id == _rule.id && item.Rule.rev != _rule.rev select item).FirstOrDefault();

                                if (RuleToUpdate != null)
                                {
                                    RuleToUpdate.Timer.Dispose();
                                    TaskTimers.Remove(RuleToUpdate);
                                    AddTaskTimer(_rule, "updated");
                                }
                                else
                                {
                                    AddTaskTimer(_rule, "added");
                                }
                            }

                            RemoveInActiveTasks(_rules);

                        }
                    }

                    WaterMark = _watermark;

                    UpdatingRules = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, ex.StackTrace);
            }
            finally
            {
                UpdatingRules = false;
                _client.Dispose();
            }
        }

        private void RemoveInActiveTasks(List<TFSServicesTypes.Rule> rules)
        {
            List<TaskTimer> ToDelete = new List<TaskTimer>();

            foreach (var Task in TaskTimers)
            {
                var FindEctiveRule = (from item in rules where item.id == Task.Rule.id && item.rev == Task.Rule.rev select item).FirstOrDefault();

                if (FindEctiveRule == null) ToDelete.Add(Task);
            }

            if (ToDelete.Count > 0)
                foreach (var Task in ToDelete)
                {
                    WriteLog(String.Format("Rule {0}: id {1}; rev {2}", "Removed", Task.Rule.id, Task.Rule.rev));
                    Task.Timer.Dispose();
                    TaskTimers.Remove(Task);
                }
        }

        private void AddTaskTimer(TFSServicesTypes.Rule pRule, string pOperation)
        {
            TaskTimers.Add(new TaskTimer { Rule = pRule, Timer = new Timer(StartTask, pRule.id, GetShiftRun(pRule), GetStepRun(pRule)) });
            WriteLog(String.Format("Rule {0}: id {1}; rev {2}", pOperation, pRule.id, pRule.rev));
        }

        int GetShiftRun(TFSServicesTypes.Rule pRule)
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

        int GetStepRun(TFSServicesTypes.Rule pRule)
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

        void StartTask(object pTaskId)
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

                    WriteLog("Start tasks id:" + _id);

                    bool _result = StartRemoteTask(_client, String.Format(Properties.Settings.Default.PathRunTask, _id)).Result;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, ex.StackTrace);
            }
            finally
            {
                _client.Dispose();
                RuningRule = false;
            }
        }

        async Task<int> GetWaterMark(HttpClient pClient, string pPath)
        {
            int _watermark = 0;
            HttpResponseMessage response = await pClient.GetAsync(pPath);
            if (response.IsSuccessStatusCode)
            {
                _watermark = await response.Content.ReadAsAsync<int>();
            }

            return _watermark;
        }

        async Task<List<TFSServicesTypes.Rule>> GetActiveTasks(HttpClient pClient, string pPath)
        {
            List<TFSServicesTypes.Rule> _rules = new List<TFSServicesTypes.Rule>();

            HttpResponseMessage response = await pClient.GetAsync(pPath);
            if (response.IsSuccessStatusCode)
            {
                _rules = await response.Content.ReadAsAsync<List<TFSServicesTypes.Rule>>();
            }

            return _rules;
        }

        async Task<bool> StartRemoteTask(HttpClient pClient, string pPath)
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
