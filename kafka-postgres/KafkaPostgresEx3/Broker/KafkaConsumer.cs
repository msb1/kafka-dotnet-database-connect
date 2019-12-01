using Confluent.Kafka;
using Dapper;
using KafkaPostgresEx3.Models;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Broker.KafkaPostgresEx3
{
    class KafkaConsumer
    {
        private string connectionString = "User ID=barnwaldo;Password=shakeydog;Host=192.168.21.5;Port=5432;Database=barnwaldo;Pooling=true;";
        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        public void RunConsumer()
        {
            // Kafka Consumer config
            var conf = new ConsumerConfig
            {
                GroupId = "toptech",
                BootstrapServers = "192.168.21.5:9092",
                //AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (IDbConnection pg = Connection)
            using (var consumer = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                // open database
                pg.Open();
                // subscribe to consumer
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
                        // create data record to write to database
                        DataRecord dr = new DataRecord
                        {
                            Topic = epd.Topic,
                            CurrentTime = epd.CurrentTime,
                            // store categories and sensors as JSON string in SQL database
                            Categories = JsonConvert.SerializeObject(epd.CatMap),
                            Sensors = JsonConvert.SerializeObject(epd.SensorMap)
                        };
                        // write data record to database

                        if (pg.State == ConnectionState.Open)
                        {
                            dr.Id = pg.Query<long>(@"INSERT INTO epdata.datarecord
                                    ( CurrentTime, Topic, Categories, Sensors ) VALUES 
                                    ( @CurrentTime, @Topic, @Categories, @Sensors ) 
                                    RETURNING id", dr).First();
                        }
                        else
                        {
                            Console.WriteLine("Postgres DB is not open... Program will close...");
                            consumer.Close();
                            break;
                        }
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
