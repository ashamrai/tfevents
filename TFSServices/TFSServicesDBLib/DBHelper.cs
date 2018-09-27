using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSServicesDBLib
{
    public class DBHelper
    {
        public TFSServicesDBContainer DB;

        public DBHelper()
        {
            DB = new TFSServicesDBContainer();
        }

        public DBHelper(TFSServicesDBContainer pDB)
        {
            DB = pDB;
        }

        public Rules GetRuleById(int pId)
        {
            return DB.RulesSet.Where(rls => rls.Id == pId && rls.IsActive == true).FirstOrDefault();
        }

        public List<Rules> GetRulesByType(string pRuleType)
        {
            var _query = from rls in DB.RulesSet where rls.RuleType.Name == pRuleType && rls.IsActive == true select rls;

            if (_query.Count() == 0) return null;

            return _query.ToList();
        }

        public List<Rules> GetScheduledRules()
        {
            var _query = from rls in DB.RulesSet where rls.RuleType.HasSchedule == true && rls.IsActive == true select rls;

            if (_query.Count() == 0) return null;

            return _query.ToList();
        }

        public int GetCurrentWatermark()
        {
            if (DB.RulesSet.Count() > 0) return DB.RulesSet.Max(rls => rls.Watermark);

            return 0;
        }

        public string AddRunHistory(Rules pRule, string pResult, string pMessage)
        {
            string _retMessage = "";

            try
            {
                RunHistory _runh = new RunHistory();
                _runh.Date = DateTime.Now;
                _runh.Result = pResult;
                _runh.Message = pMessage;
                if (pRule != null)
                {
                    _runh.Rules = pRule;
                    _runh.RuleRevision = pRule.Revision;
                }
                else
                    _runh.RuleRevision = 0;

                DB.RunHistorySet.Add(_runh);
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                _retMessage += ex.Message + "\n\n" + ex.StackTrace;
            }

            return _retMessage;
        }

        public string CleanHistory(int pDays)
        {
            string _retMessage = "";

            try
            {
                DateTime _daysLeft = DateTime.Now.AddDays((-1) * pDays);
                var _historyToRemove = DB.RunHistorySet.Where(h => h.Date < _daysLeft);
                DB.RunHistorySet.RemoveRange(_historyToRemove);
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                _retMessage += ex.Message + "\n\n" + ex.StackTrace;
            }

            return _retMessage;
        }

        ~DBHelper()
        {
            DB.Dispose();
        }
    }
}
