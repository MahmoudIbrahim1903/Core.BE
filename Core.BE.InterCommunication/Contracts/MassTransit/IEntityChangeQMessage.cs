using Emeint.Core.BE.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Contracts
{
    public interface IEntityChangeQMessage 
    {
        EntityActionType ActionType { get; set; }
        KeyValuePair<string, object> EntityKey { get; set; }
        List<KeyValuePair<string, object>> ChangedProperties { get; set; }
    }
}
