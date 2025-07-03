namespace Unltimanus.App
{
    public static class Print
    {
        private static int StopSpamIndex = 0;
        private static bool LastPoeWindowStatus = false;

        public static void PrintAppStatus(bool isRun)
        {
            if (isRun)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Работаю!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" (F6 чтобы выключить)\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Выключен!");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" (F6 чтобы включить)\n");
                Console.ResetColor();
            }
        }

        public static void PrintClientWindowStatus(bool isActive)
        {
            if (LastPoeWindowStatus == isActive)
                return;
            LastPoeWindowStatus = isActive;
            Console.Write("Окно игры: ");
            if (isActive)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Активно!\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Не активно!\n");
                Console.ResetColor();
            }
            
        }

        public static void PrintUltimatumFinded()
        {
            if (StopSpamIndex == 20)
                return;
            StopSpamIndex = 20;
            Console.WriteLine("Обнаруженн интерфейс ультиматума!");
        }

        public static void PrintUltimatumNotFinded()
        {
            if (StopSpamIndex == 21)
                return;
            StopSpamIndex = 21;
            Console.WriteLine("Жду появления интрфейса ультиматума..");
        }

        public static void PrintModiferFinded(int index, string? name, int priority)
        {
            if (StopSpamIndex == 30)
                return;
            StopSpamIndex = 30;
            string n = name ?? "N/A";
            Console.WriteLine($"Найдены модификаты, лучший выбор: №{index}({n}: {priority}).");
        }

        public static void PrintModiferNotFinded()
        {
            if (StopSpamIndex == 31)
                return;
            StopSpamIndex = 31;
            Console.WriteLine($"Жду появления модификаторв ультиматума.");
        }

        public static void AcceptButtonSucsess()
        {
            if (StopSpamIndex == 41)
                return;
            StopSpamIndex = 41;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Успешно нажал кнопку потдвердить.. Жду следующей волны..");
            Console.ResetColor();
        }

        public static void AccepbuttonError(int attempt)
        {
            if (StopSpamIndex == 41)
                return;
            StopSpamIndex = 41;
            Console.WriteLine($"Не удалось кликнуть на кнопку поддтвердить, попытка: {attempt}");
        }

        public static void AccepbuttonHardError()
        {
            if (StopSpamIndex == 41)
                return;
            StopSpamIndex = 41;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Не удалось кликнуть на кнопку поддтвердить. ОШИБКА");
            Console.ResetColor();
        }

        public static void PrintPoeLogFilePatchError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"(!) ОШИБКА !!! Пропишите в файле cfg/config.json путь до вашего файла Client.txt (!)");
            Console.ResetColor();
        }

        public static void PrintZoneChanged()
        {
            if (StopSpamIndex == 50)
                return;
            StopSpamIndex = 50;
            Console.WriteLine($"Персоонаж изменил локацию, сбрасываю приоритеты..");
            Console.ResetColor();
        }

        public static void PrintZoneName(string zone)
        {
            if (StopSpamIndex == 60)
                return;
            StopSpamIndex = 60;
            Console.WriteLine($"Текущая зона определенна как: {zone}");
            Console.ResetColor();
        }
    }
}
