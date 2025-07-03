using LaunchDarkly.EventSource;
using System;
using System.Text;

namespace Unltimanus.Utils
{
    public static class FireBaseSignals
    {
        const string firebaseBaseUrl = "https://ultimatanus-default-rtdb.firebaseio.com/";
        const string commandPath = "command";
        private static readonly string firebaseUrl = $"{firebaseBaseUrl}{commandPath}.json";

        private static EventSource _eventSource;

        public static async Task Send(string command)
        {
            var client = new HttpClient();
            var content = new StringContent(command, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(firebaseUrl, content);
            Console.WriteLine("Команда заходить в порталы отправленна..");
        }

        public static async Task StartLisen()
        {
            var config = Configuration.Builder(new Uri(firebaseUrl)).Build();

            _eventSource = new EventSource(config);

            _eventSource.MessageReceived += (sender, args) =>
            {
                var data = args.Message.Data;
                if (string.IsNullOrWhiteSpace(data) || data == "null")
                    return;

                Console.WriteLine($"[Команда получена]: {data}");
                DoAction();
            };

            _eventSource.Error += (sender, args) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR]: {args.Exception.Message}");
                Console.ResetColor();
            };

            Console.WriteLine("Слушаю команды...");
            await _eventSource.StartAsync();
        }

        public static async Task<string> Read()
        {
            var client = new HttpClient();
            string url = "https://ultimatanus-default-rtdb.firebaseio.com/command.json";
            var response = await client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            return result.Trim('"');
        }

        private static void DoAction()
        {
            Console.WriteLine("Полученна команда входить в порталы!");
            Program.PORTAL_IN = true;
        }
    }
}
