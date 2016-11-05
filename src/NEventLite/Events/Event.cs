﻿/****************************** Class Header ******************************\
Module Name:    <Event.cs>
Project:        <NEventLite> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

This is the base class for all events. We implement the required properties to make it work with Aggregates. 
This has be Serializable to make it persist to storage and reload. 
Important: Events are immutable by design. I've left the property set methods public because of potential deserialization issues.
\***************************************************************************/

using System;

namespace NEventLite.Events
{

    /// <summary>
    /// The base class for all events applies to an AggregateRoot
    /// </summary>
    public class Event : IEvent
    {
        public int TargetVersion { get; set; }
        public Guid AggregateId { get;  set; }
        public Guid Id { get; }

        public Event()
        {
        }

        /// <summary>
        /// This method makes it easier to construct a new Event
        /// </summary>
        /// <param name="AggregateID">Aggregate ID</param>
        /// <param name="TargetVersion">Target Version</param>
        public Event(Guid AggregateID, int TargetVersion):base()
        {
            this.AggregateId = AggregateID;
            this.TargetVersion = TargetVersion;
            this.Id = Guid.NewGuid();
        }
    }

}
