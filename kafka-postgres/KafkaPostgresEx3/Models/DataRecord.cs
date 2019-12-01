using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KafkaPostgresEx3.Models
{
    public class DataRecord
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string CurrentTime { get; set; }
        [Required]
        public string Topic { get; set; }
        public string Categories { get; set; }
        public string Sensors { get; set; }
        [Required]
        public int Result { get; set; }
    }

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
