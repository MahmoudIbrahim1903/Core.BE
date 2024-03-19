using System;

namespace Emeint.Core.BE.Infrastructure.Idempotency
{
    public class ClientCommand
    {
        public Guid RequestClientReferenceNumber { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public DateTime Time { get; set; }
    }
}
