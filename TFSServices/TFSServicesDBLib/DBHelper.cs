using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSServicesDBLib
{
    public static class DBHelper
    {
        public static List<Rules> GetRulesByType(string pRuleType)
        {
            var _context = new TFSServicesDBContainer();

            var _query = from rls in _context.RulesSet where rls.RuleType.Name == pRuleType && rls.IsActive == true select rls;

            if (_query.Count() == 0) return null;

            return _query.ToList();
        }
    }
}
