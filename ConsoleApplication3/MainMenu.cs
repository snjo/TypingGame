using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Asciigame
{
    class MainMenu : GameMode
    {        

        private enum MainMenuItems
        {
            Typing_Test,
            Options,
            Quit,
            END
        }
        private MainMenuItems MenuSelection = MainMenuItems.Typing_Test;

        public override void Start(Game _game)
        {
            base.Start(_game);
            MenuSelection = 0;            
        }

        public override void RequestTermination()
        {
            game.terminateApplication = true;
        }

        public override void Update()
        {
            base.Update();
            game.clearBuffer();
            Console.Clear();
            Console.SetCursorPosition(0, 3);
            for (MainMenuItems i = 0; i < MainMenuItems.END; i++)
            {
                if (i == MenuSelection)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine((MainMenuItems)i);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            if (game.GetKeyDown(Keys.Down))
            {
                MenuSelection++;
            }

            if (game.GetKeyDown(Keys.Up))
            {
                MenuSelection--;
            }

            if (game.GetKeyDown(Keys.Enter))
            {
                MenuAction(MenuSelection);
            }

            if (MenuSelection < 0) MenuSelection = 0;
            if (MenuSelection > MainMenuItems.END - 1) MenuSelection = MainMenuItems.END - 1;

            game.lastFrameTimeStamp = DateTime.Now;
            Thread.Sleep(Math.Max(0, game.desiredTimePerFrame - game.timeSinceLastFrame));
        }

        private void MenuAction(MainMenuItems selection)
        {
            switch (selection)
            {
                case MainMenuItems.Typing_Test:
                    game.setGameMode(TypingTest.Instance);
                    break;
                case MainMenuItems.Quit:
                    Quit();
                    break;
            }
        }

        public override void Quit()
        {
            game.terminateApplication = true;
        }
    }
}
