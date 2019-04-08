﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Command;
using NEventLite.Command_Bus;
using NEventLite.Command_Handler;
using NEventLite_Example.Command_Handler;

namespace NEventLite_Example.Command_Bus
{
    public class MyCommandBus : ICommandBus
    {
        private readonly NoteCommandHandler _noteCommandHandler;

        public MyCommandBus(NoteCommandHandler noteCommandHandler)
        {
            _noteCommandHandler = noteCommandHandler;
        }

        public async Task<ICommandPublishResult> ExecuteAsync<T>(T command) where T : ICommand
        {
            var handler = _noteCommandHandler as ICommandHandler<T>;

            if (handler != null)
            {

                //You can publish this to a Message Bus here
                //We will just call our command handler directly here to demonstrate async command publish via a bus

                //Important: I am not awaiting on the task to simulate a message bus publish
                Task.Run(() => handler.HandleCommandAsync(command)).ConfigureAwait(false);

                return new CommandPublishResult(true, "", null);
            }
            else
            {
                return new CommandPublishResult(false, "No command handler", 
                    new NullReferenceException($"Command handler not found for {typeof(T)}"));
            }
        }
    }
}
