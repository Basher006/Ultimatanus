using BotFW_CvSharp_01;
using BotFW_CvSharp_01.GameClientThings;
using BotFW_CvSharp_01.GlobalStructs;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Unltimanus.Utils;

namespace Unltimanus.App
{
    public class MainLoop
    {
        private Random _rnd;

        private Zone _zone = Zone.None;

        private Config _cfg;
        private ModifersPriority _modifers;
        private RECT _screenRECT;

        private Mat _screen_color;
        private Mat _screen_gray;

        private Mat _screen_FirstMod;
        private Mat _screen_SecondMod;
        private Mat _screen_ThridMod;


        private Dictionary<string, Mat> _templates;
        private Mat _ultimatumTemplate;
        private Dictionary<int, OpenCvSharp.Point> _modifersClickPoints;
        private OpenCvSharp.Point _acceptButtonPixelPoint;

        private Game _client;

        public bool Flag_AppIsRun => Program.IS_RUN;
        public bool Flag_ClientIsActive;
        public bool Flag_UltimatumWindowActive;
        public bool Flag_ModifersFinded;
        public bool Flag_AcceptButtonGlow;

        private string? _lastPikedModifer;

        private PoeLogReader? _logReader;
        PortalFinder _portalFinder;

        private string[] _hoZonesText = ["Hideout", "hideout"];
        private string[] _mapZonesText = ["Dunes"];

        private int _portalClickAttemptCounter = 0;
        private int _portalClickMaxAttempt = 4;

        public MainLoop(Config cfg, ModifersPriority modifers, PoeLogReader? logReader)
        {
            _rnd = new Random();

            _cfg = cfg;
            _modifers = modifers;
            _screenRECT = new RECT(_cfg.ScreenRect);

            _screen_color = new();
            _screen_gray = new();

            _screen_FirstMod = new();
            _screen_SecondMod = new();
            _screen_ThridMod = new();

            _templates = Utils.TemplateLoader.LoadTemplates(_cfg.TemplatesDirectiryPath, _cfg.IMG_FIND_COLOR_MODE);
            _ultimatumTemplate = Utils.TemplateLoader.LoadUltimatumTemplate(_cfg.TemplatesDirectiryPath, _cfg.UltimatumWindowTemplateFileName, _cfg.IMG_FIND_COLOR_MODE);
            _modifersClickPoints = new()
            {
                { 1, _cfg.FirstClickPoint },
                { 2, _cfg.SecondClickPoint },
                { 3, _cfg.ThridClickPoint },
            };

            int x = _cfg.AcceptChekColorPixelPoint.X - _cfg.ScreenRect.X;
            int y = _cfg.AcceptChekColorPixelPoint.Y - _cfg.ScreenRect.Y;
            _acceptButtonPixelPoint = new OpenCvSharp.Point(x, y);

            _client = new Game(_cfg.GAME_WINDOW_NAME, debug:Program.DEBUG);
            _client.TryFindWindow();
            UpdateScreen();

            _logReader = logReader;
            if (_logReader != null)
                _logReader.OnZoneWasChanged += OnZoneChanged;

            _portalFinder = new();

            OnZoneChanged();
        }

        public void Run()
        {
            while (true)
            {
                while (Flag_AppIsRun)
                {
                    GC.Collect();
                    _logReader?.Chek();

                    UpdateFlags();
                    if (Flag_ClientIsActive)
                    {
                        if (Program.PORTAL_IN)
                        {
                            PortalEnterEvent();
                        }

                        UpdateScreen();
                        UpdateFlags();

                        if (Flag_UltimatumWindowActive)
                        {
                            Print.PrintUltimatumFinded();
                            UpdateFlags(out var firstRes, out var secondRes, out var thridRes);
                            if (Flag_AppIsRun && Flag_ClientIsActive && Flag_UltimatumWindowActive && Flag_ModifersFinded)
                            {
                                var best = WhatModiferHaveBestPriority(firstRes, secondRes, thridRes);
                                int priority = 0;
                                if (_lastPikedModifer != null)
                                    priority = _modifers.Modifers_current[_lastPikedModifer];
                                Print.PrintModiferFinded(best, _lastPikedModifer, priority);

                                Thread.Sleep(500 + _rnd.Next(500, 1500));
                                ClickModifer(best);
                                MoveMouseToDefaultPosition();

                                Thread.Sleep(333);
                                UpdateScreen();

                                UpdateFlags(true);
                                if (Flag_AppIsRun && Flag_ClientIsActive && Flag_UltimatumWindowActive && Flag_AcceptButtonGlow)
                                {
                                    if (AcceptButtonClickLoop())
                                    {
                                        Print.AcceptButtonSucsess();
                                        IncreaseModifrePriority();
                                        Thread.Sleep(3000); // 3s
                                    }
                                    else
                                    {
                                        Print.AccepbuttonHardError();
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                Print.PrintModiferNotFinded();
                                continue;
                            }
 
                        }
                        else
                        {
                            Print.PrintUltimatumNotFinded();
                        }
                    }
                    else
                    {
                        Thread.Sleep(333);
                        _client.TryFindWindow();
                        continue;
                    }
                }

                Thread.Sleep(333); // sleep 0.333s
            }

            
        }

        private bool UltimatumWindowIsOpen()
        {
            var result = ModFinder.FindImage(_cfg.IMG_FIND_COLOR_MODE ? _screen_color : _screen_gray, _ultimatumTemplate);
            var max = result.MaxResLoc;
            return max.Loc.X == _cfg.UltimatumWindowPos.X && max.Loc.Y == _cfg.UltimatumWindowPos.Y;
        }

        private void ClickModifer(int best)
        {
            BotFW.MouseMove(_modifersClickPoints[best].X, _modifersClickPoints[best].Y, debug: Program.DEBUG);
            Thread.Sleep(55);
            BotFW.MouseClik(debug: Program.DEBUG);
            Thread.Sleep(71);
        }

        private void ClickPortal(OpenCvSharp.Point point)
        {
            BotFW.MouseMove(point.X, point.Y, debug: Program.DEBUG);
            Thread.Sleep(55);
            BotFW.MouseClik(debug: Program.DEBUG);
            Thread.Sleep(71);
            MoveMouseToDefaultPosition();
        }

        private void IncreaseModifrePriority()
        {
            if (_lastPikedModifer != null)
                _modifers.IncreasePriority(_lastPikedModifer);
        }

        private bool AcceptButtonClickLoop()
        {
            for (int i = 0; i < 5; i++)
            {
                ClickAccept();
                MoveMouseToDefaultPosition();
                Thread.Sleep(100);
                UpdateScreen();
                UpdateFlags(true);
                if (Flag_AppIsRun && Flag_ClientIsActive && (Flag_UltimatumWindowActive && !Flag_AcceptButtonGlow) || (!Flag_UltimatumWindowActive))
                {
                    return true;
                }
                else
                {
                    Print.AccepbuttonError(i);
                }
            }

            return false;
        }

        private void ClickAccept()
        {
            BotFW.MouseMove(_cfg.AcceptClickPoint.X, _cfg.AcceptClickPoint.Y , debug: Program.DEBUG);
            Thread.Sleep(55);
            BotFW.MouseClik(debug: Program.DEBUG);
            Thread.Sleep(71);
        }

        private void MoveMouseToDefaultPosition()
        {
            var rnd_x = _rnd.Next(_cfg.MouseDefaultPosition_randomAddRange.X);
            var rnd_y = _rnd.Next(_cfg.MouseDefaultPosition_randomAddRange.Y);

            var finalPoint_x = _cfg.MouseDefaultPosition.X + rnd_x;
            var finalPoint_y = _cfg.MouseDefaultPosition.Y + rnd_y;

            BotFW.MouseMove(finalPoint_x, finalPoint_y, debug: Program.DEBUG);

            Thread.Sleep(33);

        }

        private int WhatModiferHaveBestPriority(ModFinderResult firstRes, ModFinderResult secondRes, ModFinderResult thridRes)
        {
            var firstModiferPriority = _modifers.Modifers_current[firstRes.Name];
            var secondModiferPriority = _modifers.Modifers_current[secondRes.Name];
            var thridModiferPriority = _modifers.Modifers_current[thridRes.Name];
            if (firstModiferPriority < secondModiferPriority && firstModiferPriority < thridModiferPriority)
            {
                _lastPikedModifer = firstRes.Name;
                return 1;
            }
            else if (secondModiferPriority < firstModiferPriority && secondModiferPriority < thridModiferPriority)
            {
                _lastPikedModifer = secondRes.Name;
                return 2;
            }
            else if (thridModiferPriority < firstModiferPriority && thridModiferPriority < secondModiferPriority)
            {
                _lastPikedModifer = thridRes.Name;
                return 3;
            }

            throw new Exception($"Invalid modifers priority! (first: {firstModiferPriority}, secod :{secondModiferPriority}, thrid: {thridModiferPriority})");
        }

        private bool TryFindModifers(out ModFinderResult firstRes, out ModFinderResult secondRes, out ModFinderResult thridRes)
        {
            firstRes = ModFinder.FindMostMathMod(_screen_FirstMod, _templates);
            secondRes = ModFinder.FindMostMathMod(_screen_SecondMod, _templates);
            thridRes = ModFinder.FindMostMathMod(_screen_ThridMod, _templates);

            return firstRes.Match > _cfg.FindTR && secondRes.Match > _cfg.FindTR && thridRes.Match > _cfg.FindTR;
        }

        private void UpdateScreen()
        {
            using var bmp = new Bitmap(_cfg.ScreenRect.Width, _cfg.ScreenRect.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BotFW.GetScreen(bmp, _screenRECT, BotFW_CvSharp_01.GameClientThings.WinState.FullScreen);

            _screen_color.Dispose();
            _screen_FirstMod.Dispose();
            _screen_ThridMod.Dispose();
            _screen_ThridMod.Dispose();

            _screen_color = bmp.ToMat();

            if (_cfg.IMG_FIND_COLOR_MODE)
            {
                _screen_FirstMod = new Mat(_screen_color, _cfg.FirstModRect);
                _screen_SecondMod = new Mat(_screen_color, _cfg.SecondModRect);
                _screen_ThridMod = new Mat(_screen_color, _cfg.ThridModRect);
            }
            else
            {
                BotFW.ConvertToGray(_screen_color, _screen_gray);

                _screen_FirstMod = new Mat(_screen_gray, _cfg.FirstModRect);
                _screen_SecondMod = new Mat(_screen_gray, _cfg.SecondModRect);
                _screen_ThridMod = new Mat(_screen_gray, _cfg.ThridModRect);
            }
        }

        private bool AcceptButtonIsGlow()
        {
            var pixel = GetPixel(_cfg.IMG_FIND_COLOR_MODE ? _screen_color : _screen_gray, _acceptButtonPixelPoint.X, _acceptButtonPixelPoint.Y);
            byte red = pixel.Value.Item2;
            return red >= _cfg.AcceptButtonGlowTR;
        }

        private Vec3b? GetPixel(Mat img, int x, int y)
        {
            // Проверки
            if (img == null || img.Empty())
                return null;

            if (x < 0 || x >= img.Cols || y < 0 || y >= img.Rows)
                return null;

            if (img.Channels() == 3)
            {
                return img.At<Vec3b>(y, x);
            }
            else if (img.Channels() == 1)
            {
                var gray = img.At<byte>(y, x);
                return new Vec3b(gray, gray, gray);
            }

            throw new Exception("Exeption when read pixel from Mat");
        }

        private void UpdateFlags(bool acceptButton = false)
        {
            Flag_ClientIsActive = false;
            Flag_UltimatumWindowActive = false;
            Flag_ModifersFinded = false;
            Flag_AcceptButtonGlow = false;

            Flag_ClientIsActive = _client.IsActive;
            Print.PrintClientWindowStatus(Flag_ClientIsActive);

            if (Flag_ClientIsActive)
            {
                Flag_UltimatumWindowActive = UltimatumWindowIsOpen();
            }

            if (acceptButton)
            {
                Flag_AcceptButtonGlow = AcceptButtonIsGlow();
            }
        }

        private void UpdateFlags(out ModFinderResult firstRes, out ModFinderResult secondRes, out ModFinderResult thridRes)
        {
            firstRes = null;
            secondRes = null;
            thridRes = null;

            Flag_ClientIsActive = false;
            Flag_UltimatumWindowActive = false;
            Flag_ModifersFinded = false;
            Flag_AcceptButtonGlow = false;

            Flag_ClientIsActive = _client.IsActive;
            Print.PrintClientWindowStatus(Flag_ClientIsActive);
            if (Flag_ClientIsActive)
            {
                Flag_UltimatumWindowActive = UltimatumWindowIsOpen();
                if (Flag_UltimatumWindowActive)
                {
                    Flag_ModifersFinded = TryFindModifers(out firstRes, out secondRes, out thridRes);
                    if (Flag_ModifersFinded)
                    {
                        Flag_AcceptButtonGlow = AcceptButtonIsGlow();
                    }
                }
            }
        }

        private void OnZoneChanged()
        {
            Print.PrintZoneChanged();
            UpdateZone();
            _modifers.ResetCurrent();
        }

        private void UpdateZone()
        {
            if (_logReader != null && _logReader.LastZoneChangedText != null && _logReader.LastZoneChangedText != "")
            {
                foreach (var t in _hoZonesText)
                {
                    if (_logReader.LastZoneChangedText.Contains(t))
                    {
                        _zone = Zone.HO;
                        Print.PrintZoneName("Hideout");
                        return;
                    }
                }

                foreach (var t in _mapZonesText)
                {
                    if (_logReader.LastZoneChangedText.Contains(t))
                    {
                        _zone = Zone.Map;
                        Print.PrintZoneName("Map");
                        return;
                    }
                }
            }

            _zone = Zone.None;
            Print.PrintZoneName("None(Error)");
        }

        private void PortalEnterEvent()
        {
            var oldZone = _zone;
            _portalClickAttemptCounter = 0;
            
            while (_zone == oldZone && _portalClickAttemptCounter <= _portalClickMaxAttempt)
            {
                Console.WriteLine($"Пытаюсь зайти в портал: попытка №{_portalClickAttemptCounter}");
                if (_portalFinder.TryGetPortalClickPoint(_zone, out var point))
                {
                    ClickPortal(point);
                    Console.WriteLine($"Кликнул на портал: {point}");
                }
                else
                {
                    Console.WriteLine($"Не на шел портал!");
                }

                _portalClickAttemptCounter++;

                int sleepCounter = 0;
                int maxSleep = 1300;
                int sleepStep = 100;

                while (true)
                {
                    Thread.Sleep(sleepStep);
                    sleepCounter += sleepStep;
                    if (sleepCounter >= maxSleep)
                        break;

                    _logReader?.Chek();
                    UpdateZone();
                }
                
            }

            Program.PORTAL_IN = false;
        }

        public enum Zone
        {
            None,
            HO,
            Map
        }
    }
}
