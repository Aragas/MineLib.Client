using System;
using System.Collections.Generic;
using MineLib.Network.Data.Anvil;

namespace MineLib.Client
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<Block> b = new List<Block>();
            for (byte i = 0; i < 14; i++)
            {
                b.Add(new Block(161, i));
            }
            

            using (var game = new Graphics.Client())
                game.Run();
        }
    }
#endif
}
