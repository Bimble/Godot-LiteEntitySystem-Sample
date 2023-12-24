using Godot;
using LiteEntitySystem;

namespace Sample.Shared
{
	internal class ClientController : BasePlayerController<VisualPlayer>
	{
		public ClientController(EntityParams entityParams) : base(entityParams)
		{
		}
		protected override void GenerateInput(out PlayerInputPacket input)
		{
			var vector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			input.X = vector.X;
			input.Y = vector.Y;

			ControlledEntity.SetInput(input);
		}
	}
}
