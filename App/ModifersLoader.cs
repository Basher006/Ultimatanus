namespace Unltimanus.App
{
    public class ModifersPriority
    {
        public Dictionary<string, int> Modifers_default;
        public Dictionary<string, int> Modifers_current;

        public ModifersPriority()
        {
            Modifers_default = new();
            ResetCurrent();
        }

        public void ResetCurrent()
        {
            Modifers_current = new();
            foreach (var item in Modifers_default)
            {
                Modifers_current[item.Key] = item.Value;
            }
        }

        public void IncreasePriority(int priority)
        {
            if (priority >= Modifers_current.Count - 1 || priority < 0)
                return;

            string currentModifier = Modifers_current.FirstOrDefault(x => x.Value == priority).Key;
            if (currentModifier == null)
                return;

            string higherModifier = Modifers_current.FirstOrDefault(x => x.Value == priority + 1).Key;
            if (higherModifier == null)
                return;

            Modifers_current[currentModifier] = priority + 1;
            Modifers_current[higherModifier] = priority;
        }

        public void IncreasePriority(string modifer)
        {
            if (!Modifers_current.ContainsKey(modifer))
                return;

            int currentPriority = Modifers_current[modifer];

            if (currentPriority >= Modifers_current.Count - 1)
                return;

            string higherModifier = Modifers_current.FirstOrDefault(x => x.Value == currentPriority + 1).Key;
            if (higherModifier == null)
                return;

            Modifers_current[modifer] = currentPriority + 1;
            Modifers_current[higherModifier] = currentPriority;
        }
    }

    public static class ModifersLoader
    {
        public static ModifersPriority Load(string directory, string fileName)
        {
            var fullPatch = $"{directory}{fileName}";
            if (Directory.Exists(directory) && File.Exists(fullPatch))
            {
                var lines = File.ReadAllLines(fullPatch);

                return Parse(lines);
            }
            else
                throw new Exception($"Invalid file Patch: {fullPatch}");
        }

        private static ModifersPriority Parse(string[] lines)
        {
            var result = new ModifersPriority();
            for (int i = 0; i < lines.Count(); i++)
            {
                var name = ParseLine(lines[i]);
                result.Modifers_default.Add(name, i);
            }

            result.ResetCurrent();
            return result;
        }

        private static string ParseLine(string line)
        {
            var name = line.Split('-')[0];
            name = name.Trim();

            return name;
        }
    }


}
