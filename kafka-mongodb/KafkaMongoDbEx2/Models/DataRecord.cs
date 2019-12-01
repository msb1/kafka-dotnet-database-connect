using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KafkaMongoDbEx2.Models
{
    // EpdData class has simulator or endpoint outputs for categories and sensors
    public class EpdData
    {
        public string CurrentTime { get; set; }
        public string Topic { get; set; }
        public Dictionary<string, string> CatMap { get; set; }
        public Dictionary<string, string> SensorMap { get; set; }
        public int Result { get; set; }

        public EpdData()
        {
            CatMap = new Dictionary<string, string>();
            SensorMap = new Dictionary<string, string>();
        }
    }
}
