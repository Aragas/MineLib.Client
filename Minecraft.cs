using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MineLib.Network;
using MineLib.Network.IO;
using MineLib.Network.Module;
using MineLib.PCL.Data.BigData;

namespace MineLib.PCL
{
    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft : IMinecraftClient
    {
        public event Storage GetStorage;

        public event LoadAssembly LoadAssembly;

        #region Properties

        public string AccessToken { get; set; }

        public string ClientLogin { get; set; }
        private string _clientUsername;

        public string ClientUsername
        {
            get { return _clientUsername ?? ClientLogin; } 
            set { _clientUsername = value; }
        }
        public string ClientPassword { get; set; }
        public bool UseLogin { get; private set; }

        public string ClientToken { get; set; }

        public string SelectedProfile { get; set; }

        public string PlayerName { get; set; }
        public string ClientBrand
        {
            get { return "MineLib.Network";}
            set { throw new NotImplementedException(); }
        }

        public string ServerBrand { get; set; }

        public string ServerHost { get; set; }

        public ushort ServerPort { get; set; }

        public string ServerSalt { get; set; }

        public string ServerName { get; set; }

        public string ServerMOTD { get; set; }

        public ProtocolType Mode { get; private set; }
        public ConnectionState ConnectionState { get { return _networkHandler.ConnectionState; } }

	    public bool Connected { get { return _networkHandler.Connected; } }

	    #endregion Properties

        public bool ReducedDebugInfo;

        public World World;
        public Player Player;

        private INetworkHandler _networkHandler;

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="login">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="mode"></param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        /// <param name="serverSalt"></param>
        public IMinecraftClient Initialize(string login, string password, ProtocolType mode, INetworkTCP tcp, bool nameVerification = false, string serverSalt = null)
        {
            ClientLogin = login;
            ClientPassword = password;
            UseLogin = nameVerification;
            Mode = mode;
            ServerSalt = serverSalt;

            AsyncReceiveHandlers = new Dictionary<Type, Action<IAsyncReceive>>();
            RegisterSupportedReceiveEvents();

            World = new World();
            Player = new Player();

            _networkHandler = new NetworkHandler();
            var modules = _networkHandler.GetModules();
            _networkHandler.Initialize(ChoseModule(modules), this, tcp, true);

            return this;
        }

        public ProtocolModule ChoseModule(List<ProtocolModule> modules)
        {
            return modules[0];
        }

        public void Connect(string ip, ushort port)
        {
            _networkHandler.Connect(ip, port);
        }

        public void Disconnect()
        {
            _networkHandler.Disconnect();
        }

        /// <summary>
        /// Connects to the Minecraft Server. If connected, don't call EndConnect.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        public IAsyncResult BeginConnect(string ip, ushort port, AsyncCallback asyncCallback, object state)
        {
            ServerHost = ip;
            ServerPort = port;
            return _networkHandler.BeginConnect(ServerHost, ServerPort, asyncCallback, state);
        }

        private void EndConnect(IAsyncResult asyncResult)
        {
        }

        public IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.BeginDisconnect(asyncCallback, state);
        }

        public void EndDisconnect(IAsyncResult asyncResult)
        {
            _networkHandler.EndDisconnect(asyncResult);
        }


        public void Dispose()
        {
            if (_networkHandler != null) 
                _networkHandler.Dispose();
        }
    }
}