using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Asciigame
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
            //while (!game.terminateApplication)
            //{
            //    game.Update();
            //}
            //Console.ReadLine();
        }        
    }

    
}
