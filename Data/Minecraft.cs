using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Aragas.Core.Wrappers;

using MineLib.Core;
using MineLib.Core.Events;
using MineLib.Core.Loader;

namespace MineLib.PGL.Data
{
    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public sealed partial class Minecraft : MineLibClient
    {
        public override event EventHandler<ChoseModuleEventArgs> ChoseModule;

        public override string ClientBrand => "MineLib";

        public override string ServerHost => NetworkHandler.Host;
        public override ushort ServerPort => NetworkHandler.Port;

        public override ConnectionState ConnectionState => NetworkHandler.ConnectionState;
        public override bool Connected => NetworkHandler.Connected;

        private string ServerSalt { get; set; }
        
        public World World { get; } = new World();
        public Player Player { get; } = new Player();
        

        public Minecraft(string login, string password, ProtocolType mode, bool nameVerification = false,string serverSalt = null)
            : base(login, password, mode, nameVerification)
        {
            ServerSalt = serverSalt;

            ReceiveHandlers = new Dictionary<Type, List<Func<ReceiveEvent, Task>>>();
            RegisterSupportedReceiveEvents();

            var modules = GetModules();

            #region Module

            ProtocolAssembly module;
            if (ChoseModule == null)
                module = modules[0];
            else
            {
                var args = new ChoseModuleEventArgs(modules);
                ChoseModule(this, args);

                module = args.ChosedModule;
            }

            #endregion Module

            NetworkHandler = new ModularNetworkHandler(this, module, true);
        }
        private List<ProtocolAssembly> GetModules()
        {
            var protocols = new List<ProtocolAssembly>();

            if (FileSystemWrapper.AssemblyFolder != null)
                foreach (var file in FileSystemWrapper.AssemblyFolder.GetFilesAsync().Result)
                    if (FitsMask(file.Name, "Protocol*.dll"))
                        protocols.Add(new ProtocolAssembly(file.Name));

#if DEBUG //|| !DEBUG
            if (protocols.Count == 0)
                protocols.Add(new ProtocolAssembly("ProtocolModern.Portable"));
#endif

            return protocols;
        }
        private static bool FitsMask(string sFileName, string sFileMask)
        {
            var mask = new Regex(sFileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(sFileName);
        }

        public override void Connect(string ip, ushort port)
        {
            NetworkHandler.Connect(ip, port);
        }
        public override void Disconnect()
        {
            NetworkHandler.Disconnect();
        }

        public override void Dispose()
        {
            NetworkHandler?.Dispose();

            World?.Dispose();

            Player?.Dispose();
        }
    }
}