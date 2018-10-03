using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using ScriptsEngineLib;
using TFHelper;

namespace ConsoleTestScript
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 2)
            {
                Console.WriteLine("Use: ConsoleTestScript.exe vsts_url vsts_pat");
                return;
            }

            TFClientHelper TFClient = new TFClientHelper(args[0], args[1]);
            TestScript.CreateSprintsForEachWeek(TFClient);
        }        
    }
}
