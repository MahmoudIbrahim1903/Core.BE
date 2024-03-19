using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.InterCommunication.Contracts;
using System.Collections.Generic;

namespace Emeint.Core.BE.InterCommunication.Concretes.Base
{
    public class EntityChangeQMessageBase : IEntityChangeQMessage
    {
        public EntityActionType ActionType { get; set; }
        public KeyValuePair<string, object> EntityKey { get; set; }
        public List<KeyValuePair<string, object>> ChangedProperties { get; set; }

        public EntityChangeQMessageBase()
        {
            EntityKey = new KeyValuePair<string, object>();
            ChangedProperties = new List<KeyValuePair<string, object>>();
        }
    }
}
