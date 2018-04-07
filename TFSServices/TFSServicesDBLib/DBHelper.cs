using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSServicesDBLib
{
    public class DBHelper
    {
        public TFSServicesDBContainer DB = new TFSServicesDBContainer();

        public List<Rules> GetRulesByType(string pRuleType)
        {
            var _query = from rls in DB.RulesSet where rls.RuleType.Name == pRuleType && rls.IsActive == true select rls;

            if (_query.Count() == 0) return null;

            return _query.ToList();
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
                _runh.Rules = pRule;
                _runh.RuleRevision = pRule.Revision;
                DB.RunHistorySet.Add(_runh);
                DB.SaveChanges();
            }
            catch(Exception ex)
            {
                _retMessage += ex.Message + "\n\n" + ex.StackTrace;
            }

            return _retMessage;
        }
    }
}
