using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace TFTypesLib
{
    public class WorkItemEvent
    {
        public class WorkItemEventCore
        {
            public string subscriptionId;
            public int notificationId;
            public string id;
            public string eventType;
            public string publisherId;
            public MessageClass message;
            public DetailedMessageClass detailedMessage;
            public string resourceVersion;
            public Dictionary<string, ResourceContainerClass> resourceContainers;
            public string createdDate;
        }

        public class WorkItemEventUpdated : WorkItemEventCore
        {
            public ResourceUpdatedClass resource;
        }

        public class WorkItemEventCreated : WorkItemEventCore
        {
            public ResourceCreatedClass resource;
        }

        public class ResourceCreatedClass
        {
            public int id;
            public int rev;
            public Dictionary<string, string> fields;
            public List<RelationClass> relations;
            public Dictionary<string, LinkClass> _links;
            public string url;
        }

        public class RelationClass
        {
            public string rel;
            public string url;
            public RelAttributesClass attributes;
        }

        [DataContract()]
        public class RelAttributesClass
        {
            public bool isLocked;
            public string comment;
            public string authorizedDate;
            public string id;
            public string resourceCreatedDate;
            public string resourceModifiedDate;
            public string revisedDate;
        }

        public class ResourceContainerClass
        {
            public string id;
        }

        public class MessageClass
        {
            public string text;
            public string html;
            public string markdown;
        }

        public class DetailedMessageClass
        {
            public string text;
            public string html;
            public string markdown;
        }

        public class ResourceUpdatedClass
        {
            public int id;
            public int workItemId;
            public int rev;
            public RevisedByClass revisedBy;
            public string revisedDate;
            public Dictionary<string, FieldClass> fields;
            public Dictionary<string, List<RelationClass>> relations;
            public Dictionary<string, LinkClass> _links;
            public string url;
            public RevisionClass revision;
        }

        public class RevisedByClass
        {
            public string id;
            public string name;
            public string displayName;
            public string uniqueName;
            public string url;
            public string imageUrl;
        }

        public class RevisionClass
        {
            public int id;
            public int rev;
            public Dictionary<string, string> fields;
            public List<RelationClass> relations;
            public string url;
        }

        public class FieldClass
        {
            public string oldValue;
            public string newValue;
        }

        public class LinkClass
        {
            public string href;
        }
    }
}
