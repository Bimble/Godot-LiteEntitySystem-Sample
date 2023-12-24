using Godot;
using LiteEntitySystem;
using LiteEntitySystem.Transport;
using LiteNetLib;
using LiteNetLib.Utils;
using Sample.Shared;
using System;
using System.Net;
using System.Net.Sockets;

public partial class Multiplayer : Node, INetEventListener
{
	private NetDataWriter _writer;
	private NetPacketProcessor _packetProcessor;
	private NetManager? _netManager;
	private ClientEntityManager? _entityManager;
	private NetPeer? _server;

	private Action<DisconnectInfo> _onDisconnected;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Logger.LoggerImpl = new GodotLogger();
		EntityManager.RegisterFieldType<System.Numerics.Vector2>(System.Numerics.Vector2.Lerp);
		_writer = new NetDataWriter();
		_packetProcessor = new NetPacketProcessor();
		_netManager = new NetManager(this)
		{
			AutoRecycle = true,
			EnableStatistics = true,
			IPv6Enabled = false,
			SimulateLatency = true,
			SimulationMinLatency = 50,
			SimulationMaxLatency = 60,
			SimulatePacketLoss = false,
			SimulationPacketLossChance = 10
		};
		
		GD.Print("Connecting!");
		_netManager.Start();
		_netManager.Connect("127.0.0.1", 10515, "ExampleGame");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_netManager.PollEvents();
		if (_entityManager != null)
		{
			_entityManager.Update();
		}
	}

	public void OnPeerConnected(NetPeer peer)
	{
		GD.Print("Connected!");
		_server = peer;
		SendPacket(new JoinPacket { UserName = "Player" + Random.Shared.Next() }, DeliveryMethod.ReliableOrdered);

		var typesMap = new EntityTypesMap<GameEntities>()
				.Register(GameEntities.Player, e => new VisualPlayer(this, e))
				.Register(GameEntities.PlayerController, e => new ClientController(e));

		_entityManager = new ClientEntityManager(
				typesMap,
				new InputProcessor<PlayerInputPacket>(),
				new LiteNetLibNetPeer(peer, true),
				(byte)PacketType.EntitySystem,
				NetworkGeneral.GameFPS);
	}

	private void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
	{
		if (_server == null)
			return;
		_writer.Reset();
		_writer.Put((byte)PacketType.Serialized);
		_packetProcessor.Write(_writer, packet);
		_server.Send(_writer, deliveryMethod);
	}

	public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
	{
		_server = null;
		_entityManager = null;
		if (_onDisconnected != null)
		{
			_onDisconnected(disconnectInfo);
			_onDisconnected = null;
		}
	}

	public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
	{
		GD.Print("[C] NetworkError: " + socketError);
	}

	public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
	{
		byte packetType = reader.PeekByte();
		var pt = (PacketType)packetType;
		switch (pt)
		{
			case PacketType.EntitySystem:
				_entityManager.Deserialize(reader.AsReadOnlySpan());
				break;

			case PacketType.Serialized:
				reader.GetByte();
				_packetProcessor.ReadAllPackets(reader);
				break;

			default:
				GD.Print("Unhandled packet: " + pt);
				break;
		}
	}

	public void OnConnectionRequest(ConnectionRequest request)
	{
		request.Reject();
	}

	public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
	{
	}

	public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
	{
	}
}
