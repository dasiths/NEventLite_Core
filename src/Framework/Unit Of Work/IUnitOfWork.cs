﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;

namespace NEventLite.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        Task<T> GetAsync<T>(Guid id, int? expectedVersion = null) where T:AggregateRoot;

        void Add<T>(T aggregate) where T : AggregateRoot;

        Task CommitAsync();
    }
}
