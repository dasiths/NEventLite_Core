﻿/****************************** Class Header ******************************\
Module Name:    <IEventHandler.cs>
Project:        <NEventLite> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

This simply has an Apply method to do state changes in the implemented AggregateRoot
\***************************************************************************/

using NEventLite.Events;

namespace NEventLite.Event_Handlers
{
    /// <summary>
    /// Interface to expose the Apply() of event T in an AggregateRoot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<T> where T: IEvent
    {
        /// <summary>
        /// Apply the event
        /// </summary>
        /// <param name="event">Event</param>
        void Apply(T @event);
    }
}
