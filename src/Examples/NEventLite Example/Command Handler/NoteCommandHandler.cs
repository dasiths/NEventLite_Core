﻿using System.Threading.Tasks;
using NEventLite.Command_Handler;
using NEventLite.Unit_Of_Work;
using NEventLite_Example.Command;
using NEventLite_Example.Domain;
using NEventLite_Example.Repository;

namespace NEventLite_Example.Command_Handler
{
    public class NoteCommandHandler : ICommandHandler<CreateNoteCommand>,
                                      ICommandHandler<EditNoteCommand>
    {
        private readonly NoteRepository _repository;
        public NoteCommandHandler(NoteRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleCommandAsync(CreateNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var newNote = new Note(command.AggregateId, command.Title, command.Desc, command.Cat);
            work.Add(newNote);

            await work.CommitAsync();
        }

        public async Task HandleCommandAsync(EditNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var loadedNote = await work.GetAsync<Note>(command.AggregateId, command.TargetVersion);

            loadedNote.ChangeTitle(command.Title);
            loadedNote.ChangeDescription(command.Description);
            loadedNote.ChangeCategory(command.Cat);

            await work.CommitAsync();
        }

    }
}
