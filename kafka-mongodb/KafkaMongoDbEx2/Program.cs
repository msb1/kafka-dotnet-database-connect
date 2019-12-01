using KafkaMongoDbEx2.Broker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaMongoDbEx2
{
    class Program
    {
        static void Main(string[] args)
        {
            // start Kafka Consumer and write to database
            Thread t = new Thread(() => new KafkaConsumer().RunConsumer());
            t.Start();
            t.Join();
        }
    }
}
