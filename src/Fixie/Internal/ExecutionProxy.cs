﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Fixie.Execution;

namespace Fixie.Internal
{
    public class ExecutionProxy : LongLivedMarshalByRefObject
    {
        public IReadOnlyList<MethodGroup> DiscoverTestMethodGroups(string assemblyFullPath, Options options)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return new Discoverer(options).DiscoverTestMethodGroups(assembly);
        }

        public AssemblyResult RunAssembly(string assemblyFullPath, string listenerFactoryAssemblyFullPath, string listenerFactoryType, Options options)
        {
            var runner = CreateRunner(options, listenerFactoryAssemblyFullPath, listenerFactoryType);

            var assembly = LoadAssembly(assemblyFullPath);

            return runner.RunAssembly(assembly);
        }

        public AssemblyResult RunMethods(string assemblyFullPath, string listenerFactoryAssemblyFullPath, string listenerFactoryType, Options options, MethodGroup[] methodGroups)
        {
            var runner = CreateRunner(options, listenerFactoryAssemblyFullPath, listenerFactoryType);

            var assembly = LoadAssembly(assemblyFullPath);

            return runner.RunMethods(assembly, methodGroups);
        }

        static Runner CreateRunner(Options options, string listenerFactoryAssemblyFullPath, string listenerFactoryType)
        {
            var listener = CreateListener(listenerFactoryAssemblyFullPath, listenerFactoryType, options);

            return new Runner(listener, options);
        }

        static Listener CreateListener(string listenerFactoryAssemblyFullPath, string listenerFactoryType, Options options)
        {
            var type = Assembly.LoadFrom(listenerFactoryAssemblyFullPath).GetType(listenerFactoryType);

            var factory = (IListenerFactory)Activator.CreateInstance(type);

            return factory.Create(options);
        }

        [Obsolete]
        public AssemblyResult RunAssembly(string assemblyFullPath, Options options, Listener listener)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return Runner(options, listener).RunAssembly(assembly);
        }

        [Obsolete]
        public AssemblyResult RunMethods(string assemblyFullPath, Options options, Listener listener, MethodGroup[] methodGroups)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return Runner(options, listener).RunMethods(assembly, methodGroups);
        }

        static Assembly LoadAssembly(string assemblyFullPath)
        {
            return Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));
        }

        static Runner Runner(Options options, Listener listener)
        {
            return new Runner(listener, options);
        }
    }
}