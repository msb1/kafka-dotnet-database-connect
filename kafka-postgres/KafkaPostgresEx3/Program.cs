using Broker.KafkaPostgresEx3;
using System.Threading;


namespace KafkaPostgresEx3
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
