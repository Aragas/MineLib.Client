using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MineLib.Core;
using MineLib.Core.Interfaces;
using MineLib.Core.Loader;
using MineLib.PCL.Data.BigData;
using MineLib.PCL.Graphics;

namespace MineLib.PCL
{
    public delegate ProtocolAssembly ChoseModule(List<ProtocolAssembly> modules);

    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft<T>  : MineLibGameComponent, IMinecraftClient where T : struct, IVertexType
    {
        public event ChoseModule ChoseModule;

        #region Properties

        public string AccessToken { get; set; }

        public string ClientLogin { get; set; }
        private string _clientUsername;

        public string ClientUsername
        {
            get { return _clientUsername ?? ClientLogin; } 
            set { _clientUsername = value; }
        }
        public string ClientPassword { get; private set; }
        public bool UseLogin { get; private set; }

        public string ClientToken { get; set; }

        public string SelectedProfile { get; set; }

        public string PlayerName { get; private set; }
        public string ClientBrand
        {
            get { return "MineLib.Network";}
            set { }
        }

        public string ServerBrand { get; private set; }

        public string ServerHost { get; private set; }

        public ushort ServerPort { get; private set; }

        public string ServerSalt { get; private set; }

        public string ServerName { get; private set; }

        public string ServerMOTD { get; private set; }

        public ProtocolType Mode { get; private set; }
        public ConnectionState ConnectionState { get { return _networkHandler.ConnectionState; } }

	    public bool Connected { get { return _networkHandler.Connected; } }

	    #endregion Properties

        public bool ReducedDebugInfo;

        public World World { get; private set; }
        public Player Player { get; private set; }

        private INetworkHandler _networkHandler;

        public Minecraft(Client game) : base(game) { }

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="login">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="mode"></param>
        /// <param name="tcp"></param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        /// <param name="serverSalt"></param>
        public IMinecraftClient Initialize(string login, string password, ProtocolType mode, bool nameVerification = false, string serverSalt = null)
        {
            ClientLogin = login;
            ClientPassword = password;
            UseLogin = nameVerification;
            Mode = mode;
            ServerSalt = serverSalt;

            ReceiveHandlers = new Dictionary<Type, List<Func<IReceive, Task>>>();
            RegisterSupportedReceiveEvents();

            World = new World();
            Player = new Player();

            _networkHandler = new DefaultNetworkHandler();
            var modules = _networkHandler.GetModules();
            var module = ChoseModule == null ? modules[0] : ChoseModule(modules);
            _networkHandler.Initialize(this, module, true);

            return this;
        }

        public void Connect(string ip, ushort port)
        {
            _networkHandler.Connect(ip, port);
        }

        public void Disconnect()
        {
            _networkHandler.Disconnect();
        }

        public Task ConnectAsync(string ip, ushort port)
        {
            ServerHost = ip;
            ServerPort = port;
            return _networkHandler.ConnectAsync(ServerHost, ServerPort);
        }

        public bool DisconnectAsync()
        {
            return _networkHandler.DisconnectAsync();
        }


        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime) { }

        public override void Dispose()
        {
            if (_networkHandler != null) 
                _networkHandler.Dispose();

            if (World != null)
                World.Dispose();
        }
    }
}