using OpenCvSharp;

namespace Unltimanus.Utils
{
    public static class TemplateLoader
    {
        private const string FILE_EXTENSION = ".png";

        private static List<string> _files =
        [
            "Ailment and Curse Reflection",
            "Blistering Cold",
            "Blood Altar",
            "Buffs Expire Faster",
            "Choking Miasma",
            "Deadly Monsters",
            "Dexterous Monsters",
            "Drought",
            "Escalating Damage Taken",
            "Escalating Monster Speed",
            "Hindering Flasks",
            "Less Cooldown recovery",
            "Lessened Reach",
            "Lightning Damage from Mana Costs",
            "Lethal Rare Monsters",
            "Limited Arena",
            "Occasional Impotence",
            "Overwhelming Monsters",
            "Precise Monsters",
            "Prismatic Monsters",
            "Profane Monsters",
            "Raging Dead",
            "Razor Dance",
            "Reduced Recovery",
            "Resistant Monsters",
            "Ruin",
            "Shielding Monsters",
            "Siphoned Charges",
            "Siphoning Monsters",
            "Stalking Ruin",
            "Stormcaller Runes",
            "Totem of Costly Might",
            "Totem of Costly Potency",
            "Treacherous Auras",
            "Unlucky Criticals",
            "Unstoppable Monsters"
        ];

        private static List<string> _portalTextTemplates =
            [
                "dune_template",
                "ho_template",
            ];

        public static Dictionary<string, Mat> LoadTemplates(string path, bool IMRED_MODE_COLOR = true)
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory not exist {path}!");

            var result = new Dictionary<string, Mat>();

            foreach (var fileName in _files)
            {
                var fullPatch = $"{path}{fileName}{FILE_EXTENSION}";
                if (!File.Exists(fullPatch))
                    throw new Exception($"File not exist {fullPatch}!");

                var img = new Mat(fullPatch, IMRED_MODE_COLOR ? ImreadModes.Color : ImreadModes.Grayscale);
                result.Add(fileName, img);
            }

            return result;
        }

        public static Mat LoadUltimatumTemplate(string path, string fileName, bool IMRED_MODE_COLOR = true)
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory not exist {path}!");

            var fullPatch = $"{path}{fileName}";
            if (!File.Exists(fullPatch))
                throw new Exception($"File not exist {fullPatch}!");

            return new Mat(fullPatch, IMRED_MODE_COLOR ? ImreadModes.Color : ImreadModes.Grayscale);
        }

        public static Dictionary<string, Mat> LoadPortalTemplates(string path, bool IMRED_MODE_COLOR = true)
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory not exist {path}!");

            var result = new Dictionary<string, Mat>();

            foreach (var fileName in _portalTextTemplates)
            {
                var fullPatch = $"{path}{fileName}{FILE_EXTENSION}";
                if (!File.Exists(fullPatch))
                    throw new Exception($"File not exist {fullPatch}!");

                var img = new Mat(fullPatch, IMRED_MODE_COLOR ? ImreadModes.Color : ImreadModes.Grayscale);
                result.Add(fileName, img);
            }

            return result;
        }
    }
}
