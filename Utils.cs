using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modding_Utils
{
    public class AutoNumberToStringConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ?
                    l.ToString() :
                    reader.GetDouble().ToString();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class WorkingDirEmptyException : Exception
    {
        public WorkingDirEmptyException(){}
    }

    public class Utils
    {
        public static string WorkingDirPath = Properties.Settings.Default.WorkingDirPath;
        public static string MapDataPath = $"{WorkingDirPath}\\gn_dat5.arc\\map\\data\\";
        public static string EventsPath = $"{WorkingDirPath}\\gn_dat5.arc\\script\\event\\";
        public static List<Mentries> mentries = new List<Mentries>();
        public static List<EventInfo> events = new List<EventInfo>();
        public static bool loaded = false;

        public static void CheckWorkingDir()
        {
            if (Properties.Settings.Default.WorkingDirPath == "")
            {
                throw new WorkingDirEmptyException();
            }
        }

        public static void StoreMentries()
        {
            foreach (string dir in Directory.GetDirectories(MapDataPath))
            {
                foreach (string files in Directory.GetFiles(dir))
                {
                    if (Path.GetFileName(files).Split('.')[0].EndsWith("_obs"))
                    {
                        string jsonString = File.ReadAllText(files);
                        Mentries mentrie = JsonSerializer.Deserialize<Mentries>(jsonString);
                        mentrie.mapID = Path.GetFileName(files).Split('.')[0].Split('_')[0];
                        mentries.Add(mentrie);
                    }
                }
            }
        }

        public static void StoreEventsPaths()
        {
            foreach (string dir in Directory.GetDirectories(EventsPath))
            {
                foreach (string filepath in Directory.GetFiles(dir))
                {
                    EVStoSEVS(filepath);
                }
            }
        }

        public static void EVStoSEVS(string filepath)
        {
            try
            {
                byte[] buf = File.ReadAllBytes(filepath);
                if (buf.Length != 640)
                    throw new Exception("Incorrect file size.");

                string outfmt = "G";
                string[] lines = new string[32];
                string[] fields = new string[10];
                for (int i = 0; i < 32; ++i)
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        int index = (i * 20) + (j * 2);
                        ushort val = BitConverter.ToUInt16(buf, index);
                        fields[j] = val.ToString(outfmt);
                    }
                    lines[i] = string.Join(", ", fields);
                }
                EventInfo evs = new EventInfo();
                evs.lines = lines;
                evs.path = filepath;
                events.Add(evs);
            }
            catch (Exception){}
        }
    }
}
