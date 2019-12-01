using Confluent.Kafka;
using KafkaMongoDbEx2.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace KafkaMongoDbEx2.Broker
{
    class KafkaConsumer
    {
        private static int ctr = 0;
        private string connectionString = "mongodb://barnwaldo:shakeydog@192.168.21.5:27017";

        public void RunConsumer()
        {
            // connect to MongoDB database
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("barnwaldo");
            var collection = database.GetCollection<EpdData>("EpdSim");

            // Kafka Consumer config
            var conf = new ConsumerConfig
            {
                GroupId = "toptech",
                BootstrapServers = "192.168.21.5:9092",
                //AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                List<string> ctopics = new List<string>() { "epd00" };
                consumer.Subscribe(ctopics);
                while (true)
                {
                    try
                    {
                        // Read next message for topic
                        var msg = consumer.Consume();
                        Console.WriteLine($"Consumed message '{msg.Value}' at: '{msg.TopicPartitionOffset}'.\n");
                        if (msg.Value == "**ENDCODE**")
                        {
                            consumer.Close();
                            break;
                        }
                        // deserialize consumed message to data object
                        EpdData epd = JsonConvert.DeserializeObject<EpdData>(msg.Value);
                        // write data record to database
                        collection.InsertOne(epd);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }

            }
        }
    }

}
