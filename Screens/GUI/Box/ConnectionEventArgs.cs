using System;

using MineLib.Core.Data;

namespace MineLib.PGL.Screens.GUI.Box
{
    public class ConnectionEventArgs : EventArgs
    {
        public Server Entry { get; }
        
        public ConnectionEventArgs(Server entry)
        {
            Entry = entry;
        }
    }
}