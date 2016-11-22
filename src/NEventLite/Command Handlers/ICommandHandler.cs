﻿using System.Threading.Tasks;
using NEventLite.Commands;

namespace NEventLite.Command_Handlers
{
    public interface ICommandHandler<T> where T:ICommand
    {
        Task HandleCommandAsync(T command);
    }
}
