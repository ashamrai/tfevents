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
            TFClientHelper TFClient = new TFClientHelper(@"", @"");
            TestScript.UpdateParentProposed(TFClient);
        }        
    }
}
