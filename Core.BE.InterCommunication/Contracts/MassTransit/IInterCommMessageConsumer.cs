using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.InterCommunication.Contracts
{
    public interface IInterCommMessageConsumer<T> : IConsumer<T> where T : class
    {
    }
}
