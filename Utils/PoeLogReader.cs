using BotFW_CvSharp_01;
using System.Text;

namespace Unltimanus.Utils
{
    public class PoeLogReader
    {
        public delegate void ZoneWasChanged();
        public ZoneWasChanged? OnZoneWasChanged;
        public string LastZoneChangedText = "";

        private static readonly string[] zonesChangedText = { "Вы вошли в область", "You have entered" };

        private string _logFilePath;
        private static bool _initDone = false;
        private static long _initlogFileSize;

        public PoeLogReader(string logFilePath)
        {
            _logFilePath = logFilePath;
            InintChek();
        }

        private void InintChek()
        {
            _initlogFileSize = GetFileSize();

            var logLines = GetLogTextLines();
            if (TryGetLastZonechangedLine(logLines, out string zonechangedtext))
            {
                LastZoneChangedText = zonechangedtext;
                OnZoneWasChanged?.Invoke();
            }

            _initDone = true;
        }

        public void Chek(string? logFilePath = null)
        {
            if (!_initDone && !string.IsNullOrEmpty(logFilePath))
                InintChek();
            else if (_initDone)
                Update();
            else
                Log.Write("Try to chek poe log before init PoeLogReader!", Log.LogType.Error);
        }

        private void Update()
        {
            var nowLogSize = GetFileSize();
            if (nowLogSize > _initlogFileSize)
            {
                var updatedBytes = ReadFileBytes(_initlogFileSize, nowLogSize - _initlogFileSize);
                var updatetdText = BytesArrayToString(updatedBytes);
                var updatetdText_lines = updatetdText.Split('\n');

                if (TryGetLastZonechangedLine(updatetdText_lines, out string zonechangedtext))
                {
                    LastZoneChangedText = zonechangedtext;
                    OnZoneWasChanged?.Invoke();
                }
            }

            _initlogFileSize = nowLogSize;
        }

        private bool TryGetLastZonechangedLine(string[] logLines, out string lastZonechangedLine)
        {
            lastZonechangedLine = "";

            bool finded_eng = TryGetLastZoneChangedLine(
                logLines, out string lastZonechangedLine_eng, out int foundPos_eng, GameClinetLanguage.Eng);
            bool finded_rus = TryGetLastZoneChangedLine(
                logLines, out string lastZonechangedLine_rus, out int foundPos_rus, GameClinetLanguage.Rus);

            if (lastZonechangedLine_eng != LastZoneChangedText && lastZonechangedLine_rus != LastZoneChangedText)
            {
                if (finded_eng && !finded_rus)
                {
                    lastZonechangedLine = lastZonechangedLine_eng;
                    return true;
                }
                else if (!finded_eng && finded_rus)
                {
                    lastZonechangedLine = lastZonechangedLine_rus;
                    return true;
                }
                else if (finded_eng && finded_rus)
                {
                    lastZonechangedLine = foundPos_eng > foundPos_rus ? lastZonechangedLine_eng : lastZonechangedLine_rus;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetLastZoneChangedLine(
            string[] logLines, out string lastZonechangedLine, out int foundPos, GameClinetLanguage lang)
        {
            lastZonechangedLine = "non";
            foundPos = -1;
            for (int i = logLines.Length - 1; i >= 0; i--)
            {
                if (logLines[i].Contains(zonesChangedText[(int)lang]))
                {
                    foundPos = i;
                    lastZonechangedLine = logLines[i];
                    return true;
                }
            }
            return false;
        }

        private string[] GetLogTextLines()
        {
            using (var fs = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string[] allData = sr.ReadToEnd().Split('\n');
                    return allData;
                }
            }
        }

        private long GetFileSize()
        {
            FileInfo fileInfo = new FileInfo(_logFilePath);
            return fileInfo.Length;
        }

        private byte[] ReadFileBytes(long offset, long count)
        {
            using FileStream fs = new(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] buffer = new byte[count];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buffer, 0, (int)count);
            return buffer;
        }

        private static string BytesArrayToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        private enum GameClinetLanguage
        {
            Rus,
            Eng
        }
    }
}
