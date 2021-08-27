using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modding_Utils
{
    public class Mentries
    {
        public List<MapEvents> entries { get; set; }
        [JsonIgnore]
        public string mapID { get; set; }
    }

    public class MapEvents
    {
        [JsonConverter(typeof(AutoNumberToStringConverter))]
        public string event_arg { get; set; }
        [JsonConverter(typeof(AutoNumberToStringConverter))]
        public string event_index { get; set; }
        
    }

    public class EventInfo
    {
        public string path { get; set; }
        public string[] lines { get; set; }
    }
}
