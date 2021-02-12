using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Java_MC_Shape_To_VS_Shape
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static MCModelJSON loadedMCModel;

        public static VSModelJSON convertedVSModel;

        static readonly JsonSerializerSettings conversionSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        public static void Save(string path)
        {
            if (convertedVSModel != null)
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.Write(JsonConvert.SerializeObject(convertedVSModel, conversionSettings));
                    tw.Close();
                }
            }
        }

        public static void Convert()
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
                        RotationZ = mcRotation.Axis == EnumMCAxis.z ? mcRotation.Angle : 0
                    };
                }

                convertedVSModel = new VSModelJSON()
                {
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
    }

    public class VSModelJSON : CommonModelJson
    {
        [JsonProperty]
        public VSElementNode[] Elements { get; set; }
    }


    public class CommonModelJson
    {
        [JsonProperty]
        public Dictionary<string, string> Textures { get; set; }
    }

    public class CommonElementNode
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public double[] From { get; set; }

        [JsonProperty]
        public double[] To { get; set; }

        [JsonProperty]
        public CommonFaces Faces { get; set; }
    }

    public class McElementNode : CommonElementNode
    {
        [JsonProperty]
        public McRotation Rotation { get; set; }
    }

    public class VSElementNode : CommonElementNode
    {
        [JsonProperty]
        public double[] RotationOrigin { get; set; }

        [JsonProperty]
        public double RotationX { get; set; }

        [JsonProperty]
        public double RotationY { get; set; }

        [JsonProperty]
        public double RotationZ { get; set; }
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
