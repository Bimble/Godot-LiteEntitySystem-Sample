using LiteEntitySystem.Transport;
using LiteEntitySystem;
using LiteNetLib.Utils;
using LiteNetLib;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Numerics;
using Sample.Shared;

namespace Sample.Server
{
    public class ServerLogic : INetEventListener
    {
        private NetManager? _netManager;
        private NetPacketProcessor? _packetProcessor;
        private ServerEntityManager? _serverEntityManager;

        static ServerLogic()
        {
        }

        public void Awake()
        {
            Logger.LoggerImpl = new ConsoleLogger();
            EntityManager.RegisterFieldType<Vector2>(Vector2.Lerp);
            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                PacketPoolSize = 1000,
                SimulateLatency = true,
                SimulationMinLatency = 50,
                SimulationMaxLatency = 60,
                SimulatePacketLoss = false,
                SimulationPacketLossChance = 10
            };

            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnJoinReceived);

            var typesMap = new EntityTypesMap<GameEntities>()
                .Register(GameEntities.Player, e => new BasePlayer(e))
                .Register(GameEntities.PlayerController, e => new BasePlayerController<BasePlayer>(e));

            _serverEntityManager = new ServerEntityManager(
                typesMap,
                new InputProcessor<PlayerInputPacket>(),
                (byte)PacketType.EntitySystem,
                NetworkGeneral.GameFPS,
                ServerSendRate.HalfOfFPS);

            _netManager.Start(10515);
            Logger.LoggerImpl.Log("Started");
        }

        private void OnDestroy()
        {
            _netManager.Stop();
            _serverEntityManager = null;
        }

        public void Update()
        {
            _netManager.PollEvents();
            _serverEntityManager?.Update();
        }

        private void OnJoinReceived(JoinPacket joinPacket, NetPeer peer)
        {
            Logger.LoggerImpl.Log("[S] Join packet received: " + joinPacket.UserName);

            var serverPlayer = _serverEntityManager.AddPlayer(new LiteNetLibNetPeer(peer, true));

            var player = _serverEntityManager.AddEntity<BasePlayer>(e =>
            {
                //e.Spawn(Vector2.Normalize(new Vector2(4 * (Random.Shared.NextSingle() - 0.5f), 4 * (Random.Shared.NextSingle() - 0.5f))));
                //e.Name.Value = joinPacket.UserName;
            });
            _serverEntityManager.AddController<BasePlayerController<BasePlayer>>(serverPlayer, e => e.StartControl(player));
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            Logger.LoggerImpl.Log("[S] Player connected: " + peer.EndPoint);
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.LoggerImpl.Log("[S] Player disconnected: " + disconnectInfo.Reason);

            if (peer.Tag != null)
            {
                _serverEntityManager.RemovePlayer((LiteNetLibNetPeer)peer.Tag);
            }
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.LoggerImpl.Log("[S] NetworkError: " + socketError);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.PeekByte();
            switch ((PacketType)packetType)
            {
                case PacketType.EntitySystem:
                    _serverEntityManager.Deserialize((LiteNetLibNetPeer)peer.Tag, reader.AsReadOnlySpan());
                    break;

                case PacketType.Serialized:
                    reader.GetByte();
                    _packetProcessor.ReadAllPackets(reader, peer);
                    break;

                default:
                    Logger.LoggerImpl.Log("Unhandled packet: " + packetType);
                    break;
            }
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {

        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("ExampleGame");
        }
    }
}
