using Newtonsoft.Json;
using OpenCvSharp;

namespace Unltimanus.App
{
    public class Config
    {
        private const string FILE_NAME = "Config.json";

        public string PoeLogFilePatch; 

        public bool IMG_FIND_COLOR_MODE;
        public string GAME_WINDOW_NAME;

        public bool SlaveMode;

        public Rect ScreenRect;

        public Point UltimatumWindowPos = new Point(934, 598);
        public Rect FirstModRect;
        public Rect SecondModRect;
        public Rect ThridModRect;

        public Point FirstClickPoint;
        public Point SecondClickPoint;
        public Point ThridClickPoint;

        public Point AcceptClickPoint;
        public Point AcceptChekColorPixelPoint;
        public int AcceptButtonGlowTR;

        public Point MouseDefaultPosition;
        public Point MouseDefaultPosition_randomAddRange;

        public double FindTR;

        public string TemplatesDirectiryPath;
        public string ModifersPriorityDirectoryPath;
        public string ModifersPriorityFile;
        public string UltimatumWindowTemplateFileName;

        public static Config Load(string directiry, string fileName = FILE_NAME)
        {
            try
            {
                if (!Directory.Exists(directiry))
                {
                    Directory.CreateDirectory(directiry);
                }

                var fullPath = $"{directiry}{fileName}";
                if (File.Exists(fullPath))
                {
                    var text = File.ReadAllText(fullPath);
                    var cfg = JsonConvert.DeserializeObject<Config>(text);
                    if (cfg == null)
                        return Default;
                    return cfg;
                }
                else
                {
                    var json = JsonConvert.SerializeObject(Default, Formatting.Indented);
                    File.WriteAllText(fullPath, json);
                }
            }
            catch 
            {

            }

            return Default;
        }

        public static Config Default = new()
        {
            IMG_FIND_COLOR_MODE = true,
            GAME_WINDOW_NAME = "Path of Exile",

            SlaveMode = true,

            PoeLogFilePatch = "E:\\games\\poe\\logs\\Client.txt",

            ScreenRect = new Rect(1136, 624, 183, 172), // absolute

            UltimatumWindowPos = new Point(39, 120), // local
            FirstModRect = new Rect(17, 18, 30, 30), // local
            SecondModRect = new Rect(78, 18, 30, 30), // local
            ThridModRect = new Rect(139, 18, 30, 30), // local

            FirstClickPoint = new Point(1169, 656), // absolute
            SecondClickPoint = new Point(1231, 655), // absolute
            ThridClickPoint = new Point(1290, 657), // absolute

            AcceptClickPoint = new Point(1240, 722), // absolute
            AcceptChekColorPixelPoint = new Point(1156, 710), // absolute
            AcceptButtonGlowTR = 15, // 10 if not glow; 21 if glow  

            MouseDefaultPosition = new Point(1488, 779), // absolute
            MouseDefaultPosition_randomAddRange = new Point(33, 33),

            FindTR = 0.9,

            TemplatesDirectiryPath = "imgs\\",
            ModifersPriorityDirectoryPath = "Settings\\",
            ModifersPriorityFile = "Modifers.txt",
            UltimatumWindowTemplateFileName = "UltimatumWindow.png",
        };
    }
}
