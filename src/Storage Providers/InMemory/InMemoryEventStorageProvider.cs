﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Event;
using NEventLite.Exception;
using NEventLite.Storage;
using NEventLite_Storage_Providers.Util;

namespace NEventLite_Storage_Providers.InMemory
{
    public class InMemoryEventStorageProvider : IEventStorageProvider
    {
        private readonly string _memoryDumpFile;
        private Dictionary<Guid, List<IEvent>> _eventStream = new Dictionary<Guid, List<IEvent>>();

        public InMemoryEventStorageProvider(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _eventStream = SerializerHelper.LoadListFromFile<Dictionary<Guid, List<IEvent>>>(_memoryDumpFile).First();
            }
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(Type aggregateType, Guid aggregateId, int start, int count)
        {
            try
            {
                if (_eventStream.ContainsKey(aggregateId))
                {

                    //this is needed for make sure it doesn't fail when we have int.maxValue for count
                    if (count > int.MaxValue - start)
                    {
                        count = int.MaxValue - start;
                    }

                    return
                        _eventStream[aggregateId].Where(
                            o =>
                                (_eventStream[aggregateId].IndexOf(o) >= start) &&
                                (_eventStream[aggregateId].IndexOf(o) < (start + count)))
                            .ToArray();
                }
                else
                {
                    return new List<IEvent>();
                }

            }
            catch (Exception ex)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found. Details {ex.Message}");
            }

        }

        public async Task<IEvent> GetLastEventAsync(Type aggregateType, Guid aggregateId)
        {
            if (_eventStream.ContainsKey(aggregateId))
            {
                return _eventStream[aggregateId].Last();
            }
            else
            {
                return null;
            }
        }

        public async Task CommitChangesAsync(AggregateRoot aggregate)
        {
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                if (_eventStream.ContainsKey(aggregate.Id) == false)
                {
                    _eventStream.Add(aggregate.Id, events.ToList());
                }
                else
                {
                    _eventStream[aggregate.Id].AddRange(events);
                }
            }

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] {_eventStream});

        }
    }
}
