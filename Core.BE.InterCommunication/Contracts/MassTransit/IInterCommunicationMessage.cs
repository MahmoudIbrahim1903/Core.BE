using System;

namespace Emeint.Core.BE.InterCommunication.Contracts
{
    public interface IInterCommunicationMessage
    {
        string Id { get; set; }
        string Tag { get; set; }
        string Body { get; set; }
    }
}
