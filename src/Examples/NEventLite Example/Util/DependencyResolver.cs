﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using NEventLite.Command_Bus;
using NEventLite.Event_Bus;
using NEventLite.Logger;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite_Example.Command_Bus;
using NEventLite_Example.Command_Handler;
using NEventLite_Example.Domain;
using NEventLite_Example.Event_Bus;
using NEventLite_Example.Logging;
using NEventLite_Example.Read_Model;
using NEventLite_Example.Repository;
using NEventLite_Example.Storage;
using NEventLite_Storage_Providers.InMemory;
using ServiceStack.Text.Controller;

namespace NEventLite_Example.Util
{
    public class DependencyResolver : IDisposable
    {
        private IContainer Container { get; }

        private readonly string _inMemoryEventStorePath;
        private readonly string _inMemorySnapshotStorePath;
        private readonly string _inMemoryReadModelStorePath;

        public DependencyResolver()
        {
            // Create your builder.
            var builder = new ContainerBuilder();

            //This path is used to save in memory storage
            string strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory.Create();

            _inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            _inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";
            _inMemoryReadModelStorePath = $@"{strTempDataFolderPath}events.readmodel.dump";


            //-------- Event Stores ------------

            //Event store connection settings are in EventstoreEventStorageProvider class
            //If you don't have eventstore installed comment our the line below
            //builder.RegisterType<MyEventstoreEventStorageProvider>().As<IEventStorageProvider>().InstancePerLifetimeScope();

            builder.Register(o => new InMemoryEventStorageProvider(_inMemoryEventStorePath))
                .As<IEventStorageProvider>().PreserveExistingDefaults().InstancePerLifetimeScope();
            //----------------------------------

            //-------- Snapshot Stores ----------

            var snapshotFrequency = 10;

            //Event store connection settings are in EventstoreConnection class
            //If you don't have eventstore installed comment out the line below
            //builder.Register(o => new MyEventstoreSnapshotStorageProvider(snapshotFrequency)).As<ISnapshotStorageProvider>().InstancePerLifetimeScope();

            //Redis connection settings are in RedisConnection class
            //builder.Register(o => new MyRedisSnapshotStorageProvider(snapshotFrequency)).As<ISnapshotStorageProvider>().InstancePerLifetimeScope();

            builder.Register(o => new InMemorySnapshotStorageProvider(snapshotFrequency, _inMemorySnapshotStorePath))
                .As<ISnapshotStorageProvider>().PreserveExistingDefaults().InstancePerLifetimeScope();
            //----------------------------------

            //Event Bus
            builder.RegisterType<MyEventPublisher>().As<IEventPublisher>().InstancePerLifetimeScope();

            //Logging
            builder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();

            //This will resolve and bind storage types to a concrete repository of <T> as needed
            builder.RegisterType<NEventLite.Repository.Repository>().Named("Repository", typeof(IRepository)).InstancePerDependency();

            //This will bind the decorator
            //This way you can link multiple decorators for cross cutting concerns
            builder.RegisterDecorator<IRepository>((c, inner) => new MyRepositoryDecorator(inner), fromKey: "Repository");

            //Register NoteRepository
            builder.RegisterType<NoteRepository>().InstancePerLifetimeScope();

            //Register command bus
            builder.RegisterType<NoteCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<MyCommandBus>().As<ICommandBus>().InstancePerLifetimeScope();

            //Read model
            builder.Register(o => new MyInMemoryReadModelStorage(_inMemoryReadModelStorePath)).InstancePerLifetimeScope();
            builder.RegisterType<MyReadRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MyEventSubscriber>().InstancePerLifetimeScope();

            Container = builder.Build();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public void ClearInMemoryCache()
        {
            string[] files = new string[] {_inMemoryEventStorePath, _inMemorySnapshotStorePath, _inMemoryReadModelStorePath};

            foreach (var o in files)
            {
                if (File.Exists(o))
                {
                    File.Delete(o);
                }
            }
        }
    }
}
