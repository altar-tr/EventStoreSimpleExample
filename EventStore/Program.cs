using Common;
using EventStore.ClientAPI;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EventStore
{
    //stream => veri tam olarak veri, sınırsız, uçsuz bucaksız.
    //recorded event => yazılan eventleri gösteriyor.
    //streameventslice => event'in tek okuma operasyonu olarak geçiyor
    //eventstore clienti nasıl kullanılıyor? birton anlamadığım şey var.
    //data ne metadata ne?

    class Program
    {
        static void Main(string[] args)
        {
            //describe connection
            //bizim portumuz 2113 gösteriyor niye burada 1113 ve çalışıyor?
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));

            //open connection. senkron olması için wait komutunu veriyoruz.
            connection.ConnectAsync().Wait();

            User u = new User();
            u.Name = "Özgün";
            u.Surname = "Türkmen";
            u.UserName = "BobbyFischer";

            byte[] bytearrayData = Serialize.ObjectToByteArray(u);

            //göndereceğimiz event yani yazılacak olan event
            EventData myEvent = new EventData(Guid.NewGuid(), "myUserEvent", false,
                            bytearrayData,
                            bytearrayData);

            //asenkron olarak stream ekleme
            connection.AppendToStreamAsync("myUserstream", ExpectedVersion.Any, myEvent).Wait();

            //bu da streami okuyor herhalde. resulttan senkron olduğu sonucuna vardım.
            //streameventslice => event'in tek okuma operasyonu olarak geçiyor
            StreamEventsSlice streamEvents = connection.ReadStreamEventsForwardAsync("myUserstream", 0, 1, false).Result;

            //recorded event => yazılan eventleri gösteriyor.
            RecordedEvent returnedEvent = streamEvents.Events[0].Event;

            foreach (var item in myEvent.Data)
            {
                //wtf? neyi yazdırdı bu ? neyi datası bu? mantık tam olarak ne? nereye gidiyoruz?
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("-------------------------------------------");


            Console.WriteLine(returnedEvent.EventNumber);
            Console.WriteLine(returnedEvent.EventStreamId);
            Console.WriteLine(returnedEvent.IsJson);


            Console.ReadLine();
        }

       
    }
   
}
