using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace TFTypesLib
{
    [DataContract]
    public class WorkItemEvent
    {
        [DataContract]
        public class WorkItemEventCore
        {
            [DataMember]
            public string subscriptionId;
            [DataMember]
            public int notificationId;
            [DataMember]
            public string id;
            [DataMember]
            public string eventType;
            [DataMember]
            public string publisherId;
            [DataMember]
            public MessageClass message;
            [DataMember]
            public DetailedMessageClass detailedMessage;
            [DataMember]
            public string resourceVersion;
            [DataMember]
            public Dictionary<string, ResourceContainerClass> resourceContainers;
            [DataMember]
            public string createdDate;
        }

        [DataContract]
        public class WorkItemEventUpdated : WorkItemEventCore
        {
            [DataMember]
            public ResourceUpdatedClass resource;
        }

        [DataContract]
        public class WorkItemEventCreated : WorkItemEventCore
        {
            [DataMember]
            public ResourceCreatedClass resource;
        }

        [DataContract]
        public class ResourceCreatedClass
        {
            [DataMember]
            public int id;
            [DataMember]
            public int rev;
            [DataMember]
            public Dictionary<string, string> fields;
            [DataMember]
            public List<RelationClass> relations;
            [DataMember]
            public Dictionary<string, LinkClass> _links;
            [DataMember]
            public string url;
        }

        [DataContract]
        public class RelationClass
        {
            [DataMember]
            public string rel;
            [DataMember]
            public string url;
            [DataMember]
            public RelAttributesClass attributes;
        }

        [DataContract]
        public class RelAttributesClass
        {
            [DataMember]
            public bool isLocked;
            [DataMember]
            public string comment;
            [DataMember]
            public string authorizedDate;
            [DataMember]
            public string id;
            [DataMember]
            public string resourceCreatedDate;
            [DataMember]
            public string resourceModifiedDate;
            [DataMember]
            public string revisedDate;
        }

        [DataContract]
        public class ResourceContainerClass
        {
            [DataMember]
            public string id;
        }

        [DataContract]
        public class MessageClass
        {
            [DataMember]
            public string text;
            [DataMember]
            public string html;
            [DataMember]
            public string markdown;
        }

        [DataContract]
        public class DetailedMessageClass
        {
            [DataMember]
            public string text;
            [DataMember]
            public string html;
            [DataMember]
            public string markdown;
        }

        [DataContract]
        public class ResourceUpdatedClass
        {
            [DataMember]
            public int id;
            [DataMember]
            public int workItemId;
            [DataMember]
            public int rev;
            [DataMember]
            public RevisedByClass revisedBy;
            [DataMember]
            public string revisedDate;
            [DataMember]
            public Dictionary<string, FieldClass> fields;
            [DataMember]
            public Dictionary<string, List<RelationClass>> relations;
            [DataMember]
            public Dictionary<string, LinkClass> _links;
            [DataMember]
            public string url;
            [DataMember]
            public RevisionClass revision;
        }

        [DataContract]
        public class RevisedByClass
        {
            [DataMember]
            public string id;
            [DataMember]
            public string name;
            [DataMember]
            public string displayName;
            [DataMember]
            public string uniqueName;
            [DataMember]
            public string url;
            [DataMember]
            public string imageUrl;
        }

        [DataContract]
        public class RevisionClass
        {
            [DataMember]
            public int id;
            [DataMember]
            public int rev;
            [DataMember]
            public Dictionary<string, string> fields;
            [DataMember]
            public List<RelationClass> relations;
            [DataMember]
            public string url;
        }

        [DataContract]
        public class FieldClass
        {
            [DataMember]
            public string oldValue;
            [DataMember]
            public string newValue;
        }

        [DataContract]
        public class LinkClass
        {
            [DataMember]
            public string href;
        }
    }
}
