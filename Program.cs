using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Java_MC_Shape_To_VS_Shape
{
    public class VSShapeConverter : JsonConverter
    {
        JsonSerializerSettings conversionSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        static readonly Type[] Forbidden = new Type[]
        {
            typeof(VSModelJSON),
            typeof(VSElementNode),
            typeof(VSElementNode[]),
            typeof(CommonFaces),
            typeof(VSMCEditorSettings),
            typeof(Dictionary<string, string>),
            typeof(string),
            typeof(bool)
        };

        public override bool CanConvert(Type objectType)
        {
            bool canConvert = true;

            foreach (var val in Forbidden)
            {
                canConvert ^= objectType == val;
            }

            return canConvert;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string serialized = JsonConvert.SerializeObject(value, conversionSettings);
            writer.WriteRawValue(serialized);
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static MCModelJSON loadedMCModel;

        public static VSModelJSON convertedVSModel;

        static JsonSerializerSettings conversionSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        [STAThread]
        static void Main()
        {
            conversionSettings.Converters.Add(new VSShapeConverter());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MCShapeToVSShape());
        }


        public static void Save(string path)
        {
            if (convertedVSModel != null)
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    var serialized = JsonConvert.SerializeObject(convertedVSModel, conversionSettings);
                    serialized = serialized.Replace("  ", "\t");
                    var matches = Regex.Matches(serialized, @"[,:][^ \t\r\n]");

                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];
                        serialized = serialized.Insert(i + match.Index + 1, @" ");
                    }

                    tw.Write(serialized);
                    tw.Close();
                }
            }
        }

        public static void ConvertMCToVS()
        {
            if (loadedMCModel != null)
            {
                VSElementNode[] convertedNodes = new VSElementNode[loadedMCModel.Elements.Length];

                for (int i = 0; i < loadedMCModel.Elements.Length; i++)
                {
                    McRotation mcRotation = loadedMCModel.Elements[i].Rotation;

                    convertedNodes[i] = new VSElementNode()
                    {
                        Faces = loadedMCModel.Elements[i].Faces,
                        From = loadedMCModel.Elements[i].From,
                        Name = loadedMCModel.Elements[i].Name,
                        To = loadedMCModel.Elements[i].To,
                        RotationOrigin = loadedMCModel.Elements[i].Rotation.Origin,

                        RotationX = mcRotation.Axis == EnumMCAxis.x ? mcRotation.Angle : 0,
                        RotationY = mcRotation.Axis == EnumMCAxis.y ? mcRotation.Angle : 0,
                        RotationZ = mcRotation.Axis == EnumMCAxis.z ? mcRotation.Angle : 0,
                    };
                }

                convertedVSModel = new VSModelJSON()
                {
                    TextureWidth = loadedMCModel.Texture_Size[0],
                    TextureHeight = loadedMCModel.Texture_Size[1],
                    Textures = loadedMCModel.Textures,
                    Elements = convertedNodes
                };
            }
        }

    }

    public class MCModelJSON : CommonModelJson
    {
        [JsonProperty]
        public McElementNode[] Elements { get; set; }

        [JsonProperty]
        public int[] Texture_Size { get; set; } = new int[] { 16, 16 };
    }

    public class VSModelJSON : CommonModelJson
    {
        [JsonProperty(Order = 0)]
        public VSMCEditorSettings Editor { get; set; } = new VSMCEditorSettings();

        [JsonProperty(Order = 1)]
        public int TextureWidth { get; set; } = 16;

        [JsonProperty(Order = 2)]
        public int TextureHeight { get; set; } = 16;

        [JsonProperty(Order = 5)]
        public VSElementNode[] Elements { get; set; }
    }

    public class BlockBenchModelJson
    {
        [JsonProperty(Order = 4)]
        public BlockBenchModelJson Resolution { get; set; } = new BlockBenchModelJson();

        [JsonProperty(Order = 5)]
        public BlockBenchModelNode[] Elements { get; set; }
    }

    public class BlockBenchResolution
    {
        [JsonProperty(Order = 0)]
        public int Width { get; set; } = 16;

        [JsonProperty(Order = 1)]
        public int Height { get; set; } = 16;
    }

    public abstract class CommonModelJson
    {
        [JsonProperty(Order = 4)]
        public virtual Dictionary<string, string> Textures { get; set; }
    }

    public class BlockBenchOutLinerNode
    {
        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public double[] Origin { get; set; } = new double[] { 0, 0, 0 };

        [JsonProperty(Order = 2)]
        public string Uuid { get; set; }

        [JsonProperty(Order = 3)]
        public bool Export { get; set; } = true;

        [JsonProperty(Order = 4)]
        public bool IsOpen { get; set; } = true;

        [JsonProperty(Order = 5)]
        public bool Locked { get; set; } = false;

        [JsonProperty(Order = 6)]
        public bool Visibility { get; set; } = true;

        [JsonProperty(Order = 7)]
        public int AutoUV { get; set; } = 0;

        [JsonProperty(Order = 9999)]
        public BlockBenchOutLinerNode[] Children { get; set; }
    }

    public class BlockBenchModelNode : CommonElementNode
    {
        [JsonProperty(Order = 3)]
        public bool Rescale { get; set; } = false;

        [JsonProperty(Order = 4)]
        public int AutoUV { get; set; } = 1;

        [JsonProperty(Order = 5)]
        public EnumBBMarkerColor Color { get; set; } = EnumBBMarkerColor.LightBlue;

        [JsonProperty(Order = 6)]
        public bool Locked { get; set; } = false;

        [JsonProperty(Order = 7)]
        public double[] Rotation { get; set; }

        [JsonProperty(Order = 9999)]
        public string Uuid { get; set; }
    }

    public enum EnumBBMarkerColor
    {
        LightBlue, Yellow, Orange, Red, Purple, Blue, Green, Lime
    }

    public class CommonElementNode
    {
        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public double[] From { get; set; }

        [JsonProperty(Order = 2)]
        public double[] To { get; set; }

        [JsonProperty(Order = 9998)]
        public CommonFaces Faces { get; set; }
    }

    public class McElementNode : CommonElementNode
    {
        [JsonProperty]
        public McRotation Rotation { get; set; }
    }

    public class VSMCEditorSettings
    {
        [JsonProperty(Order = 0)]
        public bool AllAngles { get; set; } = true;

        [JsonProperty(Order = 1)]
        public bool EntityTextureMode { get; set; } = false;
    }

    public class VSElementNode : CommonElementNode
    {
        [JsonProperty(Order = 3)]
        public double[] RotationOrigin { get; set; }

        [JsonProperty(Order = 4)]
        public double RotationX { get; set; }

        [JsonProperty(Order = 5)]
        public double RotationY { get; set; }

        [JsonProperty(Order = 6)]
        public double RotationZ { get; set; }

        [JsonProperty(Order = 9999)]
        public VSElementNode[] Children { get; set; }
    }

    public class McRotation
    {

        [JsonProperty]
        public double Angle { get; set; }

        [JsonProperty]
        public EnumMCAxis Axis { get; set; }

        [JsonProperty]
        public double[] Origin { get; set; }
    }

    public class CommonFaces
    {
        [JsonProperty]
        public CommonFace North { get; set; }

        [JsonProperty]
        public CommonFace East { get; set; }

        [JsonProperty]
        public CommonFace South { get; set; }

        [JsonProperty]
        public CommonFace West { get; set; }

        [JsonProperty]
        public CommonFace Up { get; set; }

        [JsonProperty]
        public CommonFace Down { get; set; }
    }

    public class CommonFace
    {

        [JsonProperty]
        public double[] UV { get; set; }

        [JsonProperty]
        public double Rotation { get; set; }

        [JsonProperty]
        public string Texture { get; set; }
    }

    public enum EnumMCAxis
    {
        x, y, z
    }
}