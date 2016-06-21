using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asciigame
{
    public class GameMode
    {
        public Game game;
        public bool exitGameMode = false;
        
        public virtual void Start(Game _game)
        {
            exitGameMode = false;
            Debug.WriteLine("Starting gamemode " + this.GetType());
            game = _game;
        }

        public virtual void Update()
        {
            if (exitGameMode)
                Quit();
        }

        public virtual void LateUpdate()
        {
            for (int i = 0; i < game.OldKeyState.Count; i++)
            {
                Keys key = game.OldKeyState.ElementAt(i).Key;
                if (!game.OldKeyState.ContainsKey(key))
                {
                    game.OldKeyState.Add(key, false);
                    //Debug.WriteLine("adding " + key.ToString());
                }
                if (!game.NewKeyState.ContainsKey(key))
                {
                    game.NewKeyState.Add(key, false);
                    //Debug.WriteLine("adding " + key.ToString());
                }

                game.OldKeyState[key] = game.NewKeyState[key];
                game.NewKeyState[key] = Game.IsKeyPressed(key);
                //if (key == Keys.Down)
                //{
                //    if (Game.IsKeyPressed(game.OldKeyState.ElementAt(i).Key)) Debug.WriteLine(game.frameCount + " setting " + game.OldKeyState.ElementAt(i).Key.ToString() + " true: " + Game.IsKeyPressed(game.OldKeyState.ElementAt(i).Key));
                //    else Debug.WriteLine(game.frameCount + " setting " + game.OldKeyState.ElementAt(i).Key.ToString() + " false: " + Game.IsKeyPressed(game.OldKeyState.ElementAt(i).Key));
                //}
            }
        }

        public virtual void Quit()
        {
            if (game == null) Debug.WriteLine("no game!");
            game.setGameMode(game.GetMainMenu);
        }

        public virtual void RequestTermination()
        {
            exitGameMode = true;
        }
    }
}
