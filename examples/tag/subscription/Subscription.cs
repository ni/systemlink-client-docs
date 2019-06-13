using System;
using System.Threading;
using NationalInstruments.SystemLink.Clients.Tag;
using NationalInstruments.SystemLink.Clients.Tag.Values;

namespace NationalInstruments.SystemLink.Clients.Examples.Tag
{
    /// <summary>
    /// Example for the SystemLink Tag client API that uses a subscription to
    /// receive tag value updates.
    /// </summary>
    class Subscription
    {
        /// <summary>
        /// The number of tags created for the example.
        /// </summary>
        private const int NumTags = 2;

        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Create the TagManager for communicating with the server and the
             * tag writer for sending tag values to the server.
             */
            using (var manager = new TagManager(configuration))
            using (var writer = manager.CreateWriter(NumTags * 2))
            {
                /*
                 * Create two tags. Make one a double and the other an int.
                 * Using Update enables creating both tags in a single API call.
                 */
                var doubleTag = new TagData("example.subscription.double",
                    DataType.Double);
                var intTag = new TagData("example.subscription.int",
                    DataType.Int32);
                Console.WriteLine("Creating example tags...");
                manager.Update(new[] { doubleTag, intTag });

                var doubleWriter = new DoubleTagValueWriter(writer, doubleTag);
                var intWriter = new Int32TagValueWriter(writer, intTag);

                using (var selection = manager.CreateSelection(doubleTag, intTag))
                {
                    try
                    {
                        /*
                         * Subscribe to receive events when the current values
                         * of the tags change.
                         */
                        Console.WriteLine("Subscribing to tag value changes...");
                        using (var subscription = selection.CreateSubscription())
                        using (var handler = new TagChangedHandler(subscription))
                        {
                            /*
                             * Write to each tag and send them to the server.
                             */
                            Console.WriteLine("Writing tag values (1/2)...");
                            doubleWriter.Write(Math.PI);
                            intWriter.Write((int)Math.PI);
                            writer.SendBufferedWrites();

                            /*
                            * Wait for our subscription to receive each value.
                            * There may be a delay between the value changing
                            * on the server and the event firing.
                            */
                            if (!handler.WaitForNotifications(NumTags,
                                TimeSpan.FromSeconds(10)))
                            {
                                Console.WriteLine("Did not receive all tag writes");
                                return;
                            }

                            /*
                             * Subscriptions only receive the latest value when
                             * multiple writes to the same tag occur in a short
                             * period of time.
                             */
                            Console.WriteLine("Writing tag values (2/2)...");
                            doubleWriter.Write(Math.E);
                            doubleWriter.Write(12.123);
                            intWriter.Write((int)Math.E);
                            intWriter.Write(12);
                            // Writes are sent automatically due to buffer size.

                            if (!handler.WaitForNotifications(NumTags * 2,
                                TimeSpan.FromSeconds(10)))
                            {
                                Console.WriteLine("Did not receive all tag writes");
                                return;
                            }

                            /*
                             * Disposing of the subscription will unsubscribe.
                             */
                            Console.WriteLine("Unsubscribing...");
                        }
                    }
                    finally
                    {
                        Console.WriteLine("Deleting example tags...");
                        selection.DeleteTagsFromServer();
                    }
                }
            }
        }

        /// <summary>
        /// Contains a handler for <see cref="ITagSubscription.TagChanged"/>
        /// events.
        /// </summary>
        private sealed class TagChangedHandler : IDisposable
        {
            private int _receivedEvents;
            private readonly object _lock;
            private readonly ITagSubscription _subscription;

            /// <summary>
            /// Initializes a <see cref="ITagSubscription.TagChanged"/> handler.
            /// </summary>
            public TagChangedHandler(ITagSubscription subscription)
            {
                _lock = new object();
                _subscription = subscription;

                /*
                 * The TagChanged event will notify us of changes in tag value.
                 */
                subscription.TagChanged += NotifyTagChanged;
            }

            /// <summary>
            /// Waits until <paramref name="number"/> tag change events have
            /// been handled or <paramref name="timeout"/> time has passed.
            /// </summary>
            /// <returns>True if all of the notifications have been received.</returns>
            public bool WaitForNotifications(int number, TimeSpan timeout)
            {
                lock (_lock)
                {
                    while (_receivedEvents < number)
                    {
                        if (!Monitor.Wait(_lock, timeout))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public void Dispose()
            {
                _subscription.TagChanged -= NotifyTagChanged;
            }

            /// <summary>
            /// The subscription calls this method whenever a subscribed tag's
            /// current value changes.
            /// </summary>
            private void NotifyTagChanged(object sender, TagChangedEventArgs args)
            {
                switch (args.Tag.DataType)
                {
                case DataType.Double:
                    Console.WriteLine("[subscription] {0} value changed to {1}",
                        args.Tag.Path, args.DoubleValue.Read());
                    break;

                case DataType.Int32:
                    Console.WriteLine("[subscription] {0} value changed to {1}",
                        args.Tag.Path, args.Int32Value.Read());
                    break;

                default:
                    Console.WriteLine("[subscription] Unexpected {0} of type {1}",
                        args.Tag.Path, args.Tag.DataType);
                    break;
                }

                lock (_lock)
                {
                    ++_receivedEvents;
                    Monitor.PulseAll(_lock);
                }
            }
        }
    }
}
