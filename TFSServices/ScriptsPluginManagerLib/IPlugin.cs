using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptsPluginManagerLib
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        bool ConnectToService(string URI, string Username, string Password, Dictionary<string, object> CustomArgs = null);
        bool DisconectFromService();
        bool GetItem(string Id, out Dictionary<string, object> Values);
        bool GetItems(string QueryType, string QueryText, out List<Dictionary<string, object>> Items);

        bool UpdateItem(Dictionary<string, object> Values);
        bool UpdateItems(List<Dictionary<string, object>> Items);
        bool CustomAction(string Name, Dictionary<string, object> Params, out Dictionary<string, object> Values);
    }
}
