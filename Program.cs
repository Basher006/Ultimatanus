using BotFW_CvSharp_01.KeyboardMouse;
using Unltimanus.App;
using Unltimanus.Utils;

namespace Unltimanus
{
    internal class Program
    {
        public const bool DEBUG = false;
        public const bool DEBUG_PORTALS = false;

        public static bool IS_RUN = false;
        public static bool PORTAL_IN = false;

        private const string RunHotkey = "F6";
        private const string PortalCliclHotkey = "F2";

        private const string DEFAULT_CFG_DIRECTORY = "cfg\\";

        private static KeyboardHook _hook;

        private static ulong counter = 0;
        static void Main(string[] args)
        {
            Console.WriteLine();

            var cfg = Config.Load(DEFAULT_CFG_DIRECTORY);
            var modifers = ModifersLoader.Load(cfg.ModifersPriorityDirectoryPath, cfg.ModifersPriorityFile);
            if (cfg.SlaveMode && DEBUG_PORTALS == false)
                FireBaseSignals.StartLisen();

            InitKBHooks(cfg);

            PoeLogReader? logReader;
            if (File.Exists(cfg.PoeLogFilePatch))
            {
                logReader = new PoeLogReader(cfg.PoeLogFilePatch);
            }
            else
            {
                Print.PrintPoeLogFilePatchError();
                logReader = null;
            }

            var ml = new MainLoop(cfg, modifers, logReader);
            Print.PrintAppStatus(IS_RUN);

            ml.Run();
        }

        private static void InitKBHooks(Config cfg)
        {
            _hook = new();
            _hook.AddHook(RunHotkey, OnRunCallback, suppress:true);
            if (DEBUG_PORTALS || cfg.SlaveMode == false)
                _hook.AddHook(PortalCliclHotkey, OnPortal, suppress:true);
            _hook.HookEnable();
        }

        private static void OnRunCallback()
        {
            IS_RUN = !IS_RUN;
            PORTAL_IN = false;
            Print.PrintAppStatus(IS_RUN);
        }

        private static void OnPortal()
        {
            if (DEBUG_PORTALS)
            {
                PORTAL_IN = true;
            }
            else
            {
                FireBaseSignals.Send(counter++.ToString());
            }
            Console.WriteLine("F2 нажата! БЕГИТЕ В ПОРТАЛЫ! ГЛУПЦЫ!");
        }
    }
}
