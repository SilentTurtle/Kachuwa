using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Kachuwa.Core.DI;
using Microsoft.Extensions.DependencyModel;
using StackExchange.Redis;

namespace Kachuwa.Messaging
{
  
    public sealed class KachuwaPubSub : IKachuwaPubSub
    {
        private Action<Type, object> _globalHandler;
        private Action<Guid, Exception> _globalErrorHandler;

        public KachuwaPubSub()
        {
            RegisterAllSubscriber();

        }

        ///// <summary>
        ///// Returns a single instance of the <see cref="MessageHub"/>
        ///// </summary>
        //public static MessageHub Instance { get; } = new MessageHub();

        /// <summary>
        /// Registers a callback which is invoked on every message published by the <see cref="MessageHub"/>.
        /// <remarks>Invoking this method with a new <paramref name="onMessage"/>overwrites the previous one.</remarks>
        /// </summary>
        /// <param name="onMessage">
        /// The callback to invoke on every message
        /// <remarks>The callback receives the type of the message and the message as arguments</remarks>
        /// </param>
        public void RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            EnsureNotNull(onMessage);
            _globalHandler = onMessage;
        }

        /// <summary>
        /// Invoked if an error occurs when publishing a message to a subscriber.
        /// <remarks>Invoking this method with a new <paramref name="onError"/>overwrites the previous one.</remarks>
        /// </summary>
        public void RegisterGlobalErrorHandler(Action<Guid, Exception> onError)
        {
            EnsureNotNull(onError);
            _globalErrorHandler = onError;
        }

        /// <summary>
        /// Publishes the <paramref name="message"/> on the <see cref="KachuwaPubSub"/>.
        /// </summary>
        /// <param name="message">The message to published</param>
        public void Publish<T>(T message)
        {
            var localSubscriptions = GetTheLatestSubscribers();

            var msgType = typeof(T);


            var msgTypeInfo = msgType.GetTypeInfo();

            _globalHandler?.Invoke(msgType, message);

            // ReSharper disable once ForCanBeConvertedToForeach | Performance Critical
            for (var idx = 0; idx < localSubscriptions.Length; idx++)
            {
                var subscription = localSubscriptions[idx];


                if (!subscription.Type.GetTypeInfo().IsAssignableFrom(msgTypeInfo)) { continue; }

                //if (!subscription.Type.IsAssignableFrom(msgType)) { continue; }

                try
                {
                    subscription.Handle(message);
                }
                catch (Exception e)
                {
                    _globalErrorHandler?.Invoke(subscription.Token, e);
                }
            }
        }

        /// <summary>
        /// Subscribes a callback against the <see cref="KachuwaPubSub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="KachuwaPubSub"/></param>
        /// <returns>The token representing the subscription</returns>
        public Guid Subscribe<T>(Action<T> action) => Subscribe(action, TimeSpan.Zero);

        /// <summary>
        /// Subscribes a callback against the <see cref="KachuwaPubSub"/> for a specific type of message.
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="action">The callback to be invoked once the message is published on the <see cref="KachuwaPubSub"/></param>
        /// <param name="throttleBy">The <see cref="TimeSpan"/> specifying the rate at which subscription is throttled</param>
        /// <returns>The token representing the subscription</returns>
        public Guid Subscribe<T>(Action<T> action, TimeSpan throttleBy)
        {
            EnsureNotNull(action);
            return RegisterSubscriber(throttleBy, action);
        }

        /// <summary>
        /// Unsubscribes a subscription from the <see cref="KachuwaPubSub"/>.
        /// </summary>
        /// <param name="token">The token representing the subscription</param>
        public void Unsubscribe(Guid token) => UnRegisterSubscriber(token);

        /// <summary>
        /// Checks if a specific subscription is active on the <see cref="KachuwaPubSub"/>.
        /// </summary>
        /// <param name="token">The token representing the subscription</param>
        /// <returns><c>True</c> if the subscription is active otherwise <c>False</c></returns>
        public bool IsSubscribed(Guid token) => IsRegisteredSubscriber(token);

        /// <summary>
        /// Clears all the subscriptions from the <see cref="KachuwaPubSub"/>.
        /// <remarks>The global handler and the global error handler are not affected</remarks>
        /// </summary>
        public void ClearSubscriptions() => ClearSubscriber();

        /// <summary>
        /// Disposes the <see cref="KachuwaPubSub"/>.
        /// </summary>
        public void Dispose()
        {
            _globalHandler = null;
            ClearSubscriptions();
        }

        [DebuggerStepThrough]
        private void EnsureNotNull(object obj)
        {
            if (obj == null) { throw new NullReferenceException(nameof(obj)); }
        }

        private static readonly List<Subscription> AllSubscriptions = new List<Subscription>();
        private static int _subscriptionsChangeCounter;

        [ThreadStatic]
        private static int _localSubscriptionRevision;

        [ThreadStatic]
        private static Subscription[] _localSubscriptions;

        internal void RegisterAllSubscriber()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);
            var instances = runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(ISubscriber)) &&
                            t.GetConstructor(new[]{typeof(IKachuwaPubSub) }) != null)
                .Select(y => (ISubscriber)Activator.CreateInstance(y,new object[]{this}));

           
        }

        internal static Guid RegisterSubscriber<T>(TimeSpan throttleBy, Action<T> action)
        {
            var type = typeof(T);
            var key = Guid.NewGuid();
            var subscription = new Subscription(type, key, throttleBy, action);

            lock (AllSubscriptions)
            {
                AllSubscriptions.Add(subscription);
                _subscriptionsChangeCounter++;
            }

            return key;
        }

        internal static void UnRegisterSubscriber(Guid token)
        {
            lock (AllSubscriptions)
            {
                var subscription = AllSubscriptions.Find(s => s.Token == token);
                if (subscription == null) { return; }

                var removed = AllSubscriptions.Remove(subscription);
                if (!removed) { return; }

                if (_localSubscriptions != null)
                {
                    var localIdx = Array.IndexOf(_localSubscriptions, subscription);
                    if (localIdx >= 0) { _localSubscriptions = RemoveAt(_localSubscriptions, localIdx); }
                }

                _subscriptionsChangeCounter++;
            }
        }

        internal static void ClearSubscriber()
        {
            lock (AllSubscriptions)
            {
                AllSubscriptions.Clear();
                if (_localSubscriptions != null)
                {
                    Array.Clear(_localSubscriptions, 0, _localSubscriptions.Length);
                }
                _subscriptionsChangeCounter++;
            }
        }

        internal static bool IsRegisteredSubscriber(Guid token)
        {
            lock (AllSubscriptions) { return AllSubscriptions.Any(s => s.Token == token); }
        }

        internal static Subscription[] GetTheLatestSubscribers()
        {
            if (_localSubscriptions == null) { _localSubscriptions = new Subscription[0]; }

            var changeCounterLatestCopy = Interlocked.CompareExchange(ref _subscriptionsChangeCounter, 0, 0);
            if (_localSubscriptionRevision == changeCounterLatestCopy) { return _localSubscriptions; }

            Subscription[] latestSubscriptions;
            lock (AllSubscriptions)
            {
                latestSubscriptions = AllSubscriptions.ToArray();
            }

            _localSubscriptionRevision = changeCounterLatestCopy;
            _localSubscriptions = latestSubscriptions;
            return _localSubscriptions;
        }

        private static T[] RemoveAt<T>(T[] source, int index)
        {
            var dest = new T[source.Length - 1];
            if (index > 0) { Array.Copy(source, 0, dest, 0, index); }

            if (index < source.Length - 1)
            {
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
            }

            return dest;
        }
    }
}
