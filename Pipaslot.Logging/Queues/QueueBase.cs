﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pipaslot.Logging.States;
using Pipaslot.Logging.Groups;

namespace Pipaslot.Logging.Queues
{
    public abstract class QueueBase : IQueue
    {
        protected readonly LogGroupCollection LogGroups = new LogGroupCollection();
        private readonly LogGroupFormatter _formatter = new LogGroupFormatter();

        public void Write<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state)
        {//TODO Check log level
            var canCreate = CanCreateNewQueue(traceIdentifier, categoryName, severity, message, state);
            var queue = LogGroups.GetQueue(traceIdentifier, canCreate);
            if (queue == null)
            {
                // Log should be ommited
                return;
            }
            // Get current scope method name
            var lastMemberName = GetLastMethodInCurrentScope(queue, categoryName, state);
            // Check if can be written for current scope and method
            if (!CanWrite(traceIdentifier, categoryName, lastMemberName, severity, message, state))
            {
                return;
            }
            // Update depth
            var depth = queue.Depth;
            var stateType = typeof(TState);
            if (stateType == typeof(IncreaseScopeState))
            {
                depth++;
            }
            else if (stateType == typeof(DecreaseScopeState))
            {
                depth--;
            }
            // Log or finish
            if (depth <= 0)
            {
                var log = _formatter.FormatRequest(queue, traceIdentifier, LogLevel);
                Writer.WriteLog(log);

                // Remove request history from memory
                LogGroups.Remove(traceIdentifier);
            }
            else
            {
                queue.Logs.Add(new LogGroup.Log(categoryName, severity, message, state, depth));
            }
        }
        protected abstract ILogWriter Writer { get; }
        protected abstract LogLevel LogLevel { get; }
        protected abstract bool CanWrite<TState>(string traceIdentifier, string categoryName, string memberName, LogLevel severity, string message, TState state);
        protected abstract bool CanCreateNewQueue<TState>(string traceIdentifier, string categoryName, LogLevel severity, string message, TState state);

        private string GetLastMethodInCurrentScope<TState>(LogGroup logGroup, string categoryName, TState state)
        {
            var scopeState = state as IncreaseScopeState;
            if (scopeState != null)
            {
                var name = scopeState.CallerMemberName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
            }
            var lastIncreaseScopeLog = logGroup.Logs.LastOrDefault(l => l.State is IncreaseScopeState);

            if (lastIncreaseScopeLog != null && lastIncreaseScopeLog.CategoryName == categoryName)
            {
                var name = (lastIncreaseScopeLog.State as IncreaseScopeState)?.CallerMemberName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
            }

            return "";
        }
       
        public virtual void Dispose()
        {
            //write all remaining logs
            foreach (var pair in LogGroups.GetAllQueues())
            {
                var log = _formatter.FormatRequest(pair.Value, pair.Key, LogLevel);
                Writer.WriteLog(log);
            }

            LogGroups.Dispose();
        }
    }
}