using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NationalInstruments.SystemLink.Clients.Message;

namespace NationalInstruments.SystemLink.Clients.Examples.Message
{
    /// <summary>
    /// Example for the SystemLink Message Client API that communicates with
    /// the Host.vi from the Async Messaging LabVIEW example.
    /// </summary>
    class LabVIEW
    {
        /// <summary>
        /// The delay between sending periodic messages to the host topic.
        /// </summary>
        static readonly TimeSpan MessageDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The message topic for sending messages to the Host VI.
        /// </summary>
        const string HostTopic = "Example to Host";

        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Open a session and subscribe to a topic. Once subscribed, the
             * server will begin to queue messages published by Host.vi.
             */
            Console.WriteLine("Opening message session...");
            int messagesSent;
            int messagesReceived;

            using (var session = MessageSession.Open(configuration))
            using (var exitSource = new CancellationTokenSource())
            {
                session.Subscribe("Example to Client");

                /*
                 * Set up a CancelKeyPress handler to clean up the session
                 * when pressing Ctrl+C to stop the example. The handler will
                 * cancel our CancellationTokenSource, signalling our async
                 * loops to stop.
                 */
                Console.CancelKeyPress += (s, e) =>
                {
                    if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
                    {
                        // Allow Ctrl+Break to terminate the process.
                        return;
                    }

                    exitSource.Cancel();
                    Console.WriteLine("Stopping...");
                    e.Cancel = true;
                };

                /*
                 * Begin asynchronous tasks to read and publish messages until
                 * the token is canceled, then wait for the tasks to complete.
                 */
                var mainLoopTask = RunLoopsAsync(session, exitSource.Token);
                (messagesSent, messagesReceived) = mainLoopTask.Result;
            }

            Console.WriteLine("Message session closed");
            Console.WriteLine("Published {0} and received {1} messages",
                messagesSent, messagesReceived);
        }

        /// <summary>
        /// Asynchronously performs the examples tasks in a loop until the
        /// <paramref name="token"/> is canceled.
        /// </summary>
        /// <returns>
        /// A task that when complete contains a tuple with the number of
        /// messages published and the number of messages received in total.
        /// </returns>
        static async Task<(int messagesSent, int messagesReceived)> RunLoopsAsync(
            IQueuedMessageSession session,
            CancellationToken token)
        {
            Console.WriteLine("Run Host.vi to send and receive messages from this example.");
            Console.WriteLine("Press Ctrl+C to stop.");
            Console.WriteLine();

            /*
             * Begin each loop and wait for both to complete.
             */
            var publishLoop = PublishLoopAsync(session, token);
            var readLoop = ReadMessageLoopAsync(session, token);
            await Task.WhenAll(publishLoop, readLoop).ConfigureAwait(false);
            return (publishLoop.Result, readLoop.Result);
        }

        /// <summary>
        /// Asynchronously publishes messages to Host.vi with a
        /// <see cref="MessageDelay"/> delay before each message until the
        /// <paramref name="token"/> is canceled.
        /// </summary>
        /// <returns>A task that when complete contains the number of messages
        /// published by the loop.</returns>
        static async Task<int> PublishLoopAsync(IMessageSession session, CancellationToken token)
        {
            int messagesSent = 0;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(MessageDelay, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Return when the token is canceled during the delay.
                    break;
                }

                var message = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                await session.PublishAsync(HostTopic, message).ConfigureAwait(false);
                ++messagesSent;
            }

            return messagesSent;
        }

        /// <summary>
        /// Asynchronously reads messages from Host.vi until
        /// <paramref name="token"/> is canceled.
        /// </summary>
        /// <returns>A task that when complete contains the number of messages
        /// read by the loop.</returns>
        static async Task<int> ReadMessageLoopAsync(IQueuedMessageSession session, CancellationToken token)
        {
            int messagesReceived = 0;

            while (!token.IsCancellationRequested)
            {
                /*
                 * Try to read a message if one is available. The timeout
                 * determines the responsiveness of stopping the example,
                 * because we wait for the read to complete before returning.
                 */
                var message = await session.ReadAsync(timeoutMilliseconds: 2000)
                    .ConfigureAwait(false);

                if (message == null)
                {
                    // No message has been queued, so loop around to read again.
                    continue;
                }

                ++messagesReceived;
                var date = DateTime.Now.ToString("d", CultureInfo.CurrentCulture);
                var time = DateTime.Now.ToString("T", CultureInfo.CurrentCulture);

                Console.WriteLine("Received message:");
                Console.WriteLine("\tDate: {0}", date);
                Console.WriteLine("\tTime: {0}", time);
                Console.WriteLine("\tTopic: {0}", message.Topic);
                Console.WriteLine("\tMessage: {0}", message.Message);
                Console.WriteLine();

                /*
                 * Send a message back to the host to indicate we received
                 * the message. The reply is sent using a different topic than
                 * the one on which we received the message.
                 */
                await session.PublishAsync(HostTopic,
                    "Replying to " + message.Message).ConfigureAwait(false);
            }

            return messagesReceived;
        }
    }
}
