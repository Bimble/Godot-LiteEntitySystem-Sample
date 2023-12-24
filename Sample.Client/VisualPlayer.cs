using Godot;
using LiteEntitySystem;

namespace Sample.Shared
{
    [UpdateableEntity(true)]
    internal class VisualPlayer : BasePlayer
    {
        Player _player;
        private PlayerInputPacket _input;

        public VisualPlayer(Node parentNode, EntityParams entityParams) : base(entityParams)
        {
            var scene = GD.Load<PackedScene>("res://player.tscn");
            _player = (Player)scene.Instantiate();
            parentNode.AddChild(_player);
        }

        protected override void VisualUpdate()
        {
            _player.Position = new Vector2(Position.Value.X, Position.Value.Y);
        }

        public void SetInput(PlayerInputPacket input)
        {
            _input = input;
        }

        protected override void Update()
        {
            Position += 200 * new System.Numerics.Vector2(_input.X, _input.Y) * EntityManager.DeltaTimeF;
            _player.Position = new Vector2(Position.Value.X, Position.Value.Y);
        }
    }
}
