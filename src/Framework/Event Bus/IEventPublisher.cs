﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Event;

namespace NEventLite.Event_Bus
{
    public interface IEventPublisher
    {
        Task PublishAsync(IEvent @event);
    }
}
