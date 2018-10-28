using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFHelper;
using Topshelf;

namespace TFTaskService
{
    class Program
    {       

        static void Main(string[] args)
        {
            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");

            var rc = HostFactory.Run(x =>                                   
            {
                x.Service<TFTasksManager>(s =>                                   
                {
                    s.ConstructUsing(name => new TFTasksManager());                
                    s.WhenStarted(tc => tc.Start());                         
                    s.WhenStopped(tc => tc.Stop());                          
                });

                x.StartAutomatically(); 

                x.SetDescription("Team Foundaio Services Automation Tasks Host");                   
                x.SetDisplayName("TFTaskService");                                  
                x.SetServiceName("TFTaskService");                                  
            });           
        }        
    }
}
