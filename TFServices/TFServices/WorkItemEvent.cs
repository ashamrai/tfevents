using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace TFServices
{
    public class WorkItemEvent
    {
        public WorkItemEventCore Core;

        public Dictionary<string, FieldClass> UpdatedFields;

        public RevisedByClass RevisedBy;

        public Dictionary<string, string> Fields;

        public int id;
        public int rev;
        public Dictionary<string, LinkClass> _links;
        public string url;
    }

    public class WorkItemEventCore
    {
        [JsonProperty("subscriptionId")]
        public string subscriptionId;

        [JsonProperty("notificationId")]
        public int notificationId;

        [JsonProperty("id")]
        public string id;

        [JsonProperty("eventType")]
        public string eventType;

        [JsonProperty("publisherId")]
        public string publisherId;

        [JsonProperty("message")]
        public MessageClass message;

        [JsonProperty("detailedMessage")]
        public DetailedMessageClass detailedMessage;

        [JsonProperty("resourceVersion")]
        public string resourceVersion;

        [JsonProperty("resourceContainers")]
        public Dictionary<string, ResourceContainerClass> resourceContainers;

        [JsonProperty("createdDate")]
        public string createdDate;

        public WIEventType EventType
        {
            get
            {
                if (eventType.ToLower() == "workitem.created") return WIEventType.WICREATED;
                else return WIEventType.WIUPDATED;
            }
        }
    }

    public class WorkItemEventUpdated : WorkItemEventCore
    {
        [JsonProperty("resource")]
        public ResourceUpdatedClass resource;
    }

    public class WorkItemEventCreated : WorkItemEventCore
    {
        [JsonProperty("resource")]
        public ResourceCreatedClass resource;
    }

    public class ResourceCreatedClass
    {
        [JsonProperty("id")]
        public int id;

        [JsonProperty("rev")]
        public int rev;

        [JsonProperty("fields")]
        public Dictionary<string, string> fields;

        [JsonProperty("_links")]
        public Dictionary<string, LinkClass> _links;

        [JsonProperty("url")]
        public string url;
    }

    public class ResourceContainerClass
    {
        [JsonProperty("id")]
        public string id;
    }

    public class MessageClass
    {
        [JsonProperty("text")]
        public string text;

        [JsonProperty("html")]
        public string html;

        [JsonProperty("markdown")]
        public string markdown;
    }


    public class DetailedMessageClass
    {
        [JsonProperty("text")]
        public string text;

        [JsonProperty("html")]
        public string html;

        [JsonProperty("markdown")]
        public string markdown;
    }


    public class ResourceUpdatedClass
    {
        [JsonProperty("id")]
        public int id;

        [JsonProperty("workItemId")]
        public int workItemId;

        [JsonProperty("rev")]
        public int rev;

        [JsonProperty("revisedBy")]
        public RevisedByClass revisedBy;

        [JsonProperty("revisedDate")]
        public string revisedDate;

        [JsonProperty("fields")]
        public Dictionary<string, FieldClass> fields;

        [JsonProperty("_links")]
        public Dictionary<string, LinkClass> _links;

        [JsonProperty("url")]
        public string url;

        [JsonProperty("revision")]
        public RevisionClass revision;
    }

    public class RevisedByClass
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("name")]
        public string name;

        [JsonProperty("displayName")]
        public string displayName;

        [JsonProperty("uniqueName")]
        public string uniqueName;

        [JsonProperty("url")]
        public string url;

        [JsonProperty("imageUrl")]
        public string imageUrl;
    }

    public class RevisionClass
    {
        [JsonProperty("id")]
        public int id;

        [JsonProperty("rev")]
        public int rev;

        [JsonProperty("fields")]
        public Dictionary<string, string> fields;

        [JsonProperty("url")]
        public string url;
    }

    public class FieldClass
    {
        [JsonProperty("oldValue")]
        public string oldValue;

        [JsonProperty("newValue")]
        public string newValue;
    }

    public class LinkClass
    {
        [JsonProperty("href")]
        public string href;
    }

}