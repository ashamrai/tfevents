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
        public Dictionary<string, List<RelationClass>> UpdatedRelations;        

        public Dictionary<string, string> Fields;
        public List<RelationClass> Relations;

        public RevisedByClass RevisedBy;

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

        [JsonProperty("relations")]
        public List<RelationClass> relations;

        [JsonProperty("_links")]
        public Dictionary<string, LinkClass> _links;

        [JsonProperty("url")]
        public string url;
    }

    public class RelationClass
    {
        [JsonProperty("rel")]
        public string rel;

        [JsonProperty("url")]
        public string url;

        [JsonProperty("attributes")]
        public RelAttributesClass attributes;
    }

    public class RelAttributesClass
    {
        [JsonProperty("isLocked")]
        public bool isLocked;

        [JsonProperty("comment")]
        public string comment;

        [JsonProperty("authorizedDate")]
        public string authorizedDate;

        [JsonProperty("id")]
        public string id;

        [JsonProperty("resourceCreatedDate")]
        public string resourceCreatedDate;

        [JsonProperty("resourceModifiedDate")]
        public string resourceModifiedDate;

        [JsonProperty("revisedDate")]
        public string revisedDate;
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

        [JsonProperty("relations")]
        public Dictionary<string, List<RelationClass>> relations;

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

        [JsonProperty("relations")]
        public List<RelationClass> relations;

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