using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Asciigame
{
    public class Game
    {
        [DllImport("user32.dll")]
        public static extern ushort GetKeyState(short nVirtKey);

        public Dictionary<Keys, bool> NewKeyState = new Dictionary<Keys, bool>();
        public Dictionary<Keys, bool> OldKeyState = new Dictionary<Keys, bool>();

        //Thread MainThread;
        //public Thread ConsoleKeyListener;
        public bool terminateApplication = false;
        public Random rnd = new Random();

        public int frameCount = 0;

        MainMenu _mainMenu;
        GameMode currentGameMode;

        public ConsoleKey lastKeyPressed;

        public int desiredFrameRate = 30;
        public int currentFrameRate = 0;

        public bool displayFrameRate = true;
        public DateTime lastFrameTimeStamp = new DateTime();

        public IntVector2 bufferSize;
        public char[][] buffer;

        //public Action Update;                

        public bool inputOverride = false; // if true, the key listener will not run. For use in other text input methods.

        public const ushort keyDownBit = 0x80;

        public static bool IsKeyPressed(Keys key)
        {
            return ((GetKeyState((short)key) & keyDownBit) == keyDownBit);
        }

        public Game()
        {            
        }

        public void Start()
        {            
            bufferSize = new IntVector2(79, 24); //Console.WindowWidth, Console.WindowHeight);
            buffer = new char[bufferSize.y][];
            for (int i = 0; i < bufferSize.y; i++)
            {
                buffer[i] = new char[bufferSize.x];                
            }
            Console.SetWindowSize(bufferSize.x + 1, bufferSize.y + 1);
            lastFrameTimeStamp = DateTime.Now;
            Console.CursorVisible = false;            

            //MainThread = new Thread(new ThreadStart(startMainThread));
            //ConsoleKeyListener = new Thread(new ThreadStart(ListenKeyBoardEvent));
            //MainThread.Name = "GameLogic";
            //ConsoleKeyListener.Name = "KeyListener";
            
            setGameMode(new TypingTest());

            //ConsoleKeyListener.Start();
            //MainThread.Start();

            UpdateLoop();                        
        }

        public void setGameMode(GameMode newMode)
        {
            currentGameMode = newMode;
            newMode.Start(this);
        }

        public void writeBufferToScreen()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < bufferSize.y; i++)
            {
                Console.WriteLine(buffer[i]);
            }
        }

        public void clearBuffer()
        {
            for (int i = 0; i < bufferSize.y; i++)
            {
                for (int j = 0; j < bufferSize.x; j++)
                {
                    buffer[i][j] = ' ';
                }
            }
        }

        public int desiredTimePerFrame
        {
            get
            {
                return 1000 / desiredFrameRate;
            }
        }

        public int timeSinceLastFrame
        {
            get
            {                
                TimeSpan span = DateTime.Now - lastFrameTimeStamp;
                return (int)span.TotalMilliseconds;
            }
        }

        public int getFrameRate()
        {
            if (timeSinceLastFrame > 1)
                return 1000 / timeSinceLastFrame;
            else return 0;
        }

        public void drawFrameRate()
        {
            if (displayFrameRate)
            {                
                Console.SetCursorPosition(0, 0);
                string fpsString = "FPS: " + currentFrameRate;
                int bufferSubStart = fpsString.Length-1;
                int bufferSubEnd = bufferSize.x - fpsString.Length;
                fpsString += new string(buffer[0]).Substring(bufferSubStart, bufferSubEnd);
                buffer[0] = fpsString.ToCharArray();
            }                        
        } 

        private void UpdateLoop()
        {
            while (!terminateApplication)
            {
                frameCount++;
                currentGameMode.Update();
                currentGameMode.LateUpdate();
            }
        }

        public bool GetKeyDown(Keys key)
        {
            if (OldKeyState.ContainsKey(key))
            {
                return NewKeyState[key] && !OldKeyState[key];
            }
            else
            {
                Debug.WriteLine("adding " + key.ToString());
                OldKeyState.Add(key, false);
                return IsKeyPressed(key);
            }
        }               

        public GameMode GetMainMenu {
            get
            {
                if (_mainMenu == null)
                    _mainMenu = new MainMenu();
                return _mainMenu;
            }
        }
    }
}
