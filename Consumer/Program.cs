using Common;
using EventStore.ClientAPI;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //describe connection
            //bizim portumuz 2113 gösteriyor niye burada 1113 ve çalışıyor?
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));

            //open connection. senkron olması için wait komutunu veriyoruz.
            connection.ConnectAsync().Wait();

            StreamEventsSlice streamEvents = connection.ReadStreamEventsForwardAsync("myUserstream", 0, 1, false).Result;

            //recorded event => yazılan eventleri gösteriyor.
            RecordedEvent returnedEvent = streamEvents.Events[0].Event;

            foreach (var item in streamEvents.Events)
            {
                var i = 0;
                returnedEvent = streamEvents.Events[i].Event;

                Console.WriteLine(returnedEvent.EventNumber);
                Console.WriteLine(returnedEvent.EventStreamId);
                Console.WriteLine(returnedEvent.IsJson);

                var o = returnedEvent.Data;



                //object user = Serialize.ByteArrayToObject(returnedEvent.Data);
                //Console.WriteLine(user.Name);
                //Console.WriteLine(user.Surname);
                //Console.WriteLine(user.UserName);
                i++;
            }

            Console.ReadLine();
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

    }


}
