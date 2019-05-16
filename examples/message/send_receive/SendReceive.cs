using System;
using NationalInstruments.SystemLink.Clients.Core;
using NationalInstruments.SystemLink.Clients.Message;
using Newtonsoft.Json;

namespace NationalInstruments.SystemLink.Clients.Examples.Message
{
    /// <summary>
    /// Example for the SystemLink Message client API that opens two message
    /// sessions, sends JSON messages from one, then reads those messages back
    /// from the other.
    /// </summary>
    class SendReceive
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Open a session and subscribe to a couple of topics. Once
             * subscribed, the server will begin to queue messages published
             * these topics. They remain queued until read by the session.
             */
            Console.WriteLine("Opening message read session...");

            using (var readSession = MessageSession.Open(configuration))
            {
                readSession.Subscribe("example.data");
                readSession.Subscribe("example.exit");

                /*
                 * The read session will receive all messages for the the
                 * subscribed topics, but not the message for the unknown
                 * topic. The messages are read in order they are queued on the
                 * server regardless of topic. There may be a small delay
                 * between the message being published and it being queued and
                 * available for reading.
                 */
                PublishMessages(configuration);
                Console.WriteLine("Message queue size: {0} of {1} bytes",
                   readSession.QueueSize, readSession.MaxQueueSize);
                Console.WriteLine("Reading back messages...");
                bool receivedExitMessage = false;

                while (!receivedExitMessage)
                {
                    /*
                     * Blocks until a message is available in the queue or a
                     * brief amount of time has passed. Returns null when there
                     * are no queued messages.
                     */
                    var message = readSession.Read();
                    receivedExitMessage = HandleMessage(message);
                }

                /*
                 * The server will discard any remaining queued messages when
                 * disposing the session.
                 */
                Console.WriteLine("Remaining bytes in the message queue: {0}",
                    readSession.QueueSize);
            }
        }

        private static void PublishMessages(IHttpConfiguration configuration)
        {
            /*
             * Open a separate session for publishing messages. This is not
             * required, but simulates how two separate applications can
             * communicate with each other using messages.
             */
            Console.WriteLine("Publishing messages...");

            using (var writeSession = MessageSession.Open(configuration))
            {
                /*
                 * Messages can contain any string. It's up to the application
                 * to determine the data format to use for each message topic.
                 * In this example, the example.data topic contains a JSON
                 * representation of the MessageData class.
                 */
                var data = JsonConvert.SerializeObject(new MessageData
                {
                    Message = "Hello World! PI=",
                    Value = Math.PI
                });
                writeSession.Publish("example.data", data);

                data = JsonConvert.SerializeObject(new MessageData
                {
                    Message = "Another message, E=",
                    Value = Math.E
                });
                writeSession.Publish("example.data", data);

                /*
                 * The server will ignore messages published to a topic with no
                 * subscribers, but it's not an error.
                 */
                writeSession.Publish("example.unknown", "no subscribers");

                /*
                 * Publish messages to the example.exit topic to signal the
                 * reader to stop. Both are queued, but only one is read.
                 */
                writeSession.Publish("example.exit", "first exit");
                writeSession.Publish("example.exit", "second exit");
            }
        }

        private static bool HandleMessage(MessageWithTopic message)
        {
            if (message == null)
            {
                Console.WriteLine("Read timed out, trying again");
                return false;
            }

            /*
             * Received a message. Parse the data based on the topic.
             */
            switch (message.Topic)
            {
                case "example.data":
                    var data = JsonConvert.DeserializeObject<MessageData>(
                        message.Message);
                    Console.WriteLine("Received message {0}{1}",
                        data.Message, data.Value);
                    return false;

                case "example.exit":
                    Console.WriteLine("Received exit message {0}",
                        message.Message);
                    return true;

                default:
                    Console.Error.WriteLine("Unexpected message topic {0}",
                        message.Topic);
                    return false;
            }
        }

        private class MessageData
        {
            public string Message { get; set; }
            public double Value { get; set; }
        }
    }
}
