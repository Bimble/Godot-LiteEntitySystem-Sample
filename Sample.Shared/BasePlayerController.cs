using LiteEntitySystem;

namespace Sample.Shared
{
    public class BasePlayerController<T> : HumanControllerLogic<PlayerInputPacket, T> where T : BasePlayer
    {
        public BasePlayerController(EntityParams entityParams) : base(entityParams)
        {
        }

        protected override void GenerateInput(out PlayerInputPacket input)
        {
            throw new NotImplementedException();
        }

        protected override void ReadInput(in PlayerInputPacket input)
        {
            ControlledEntity.Position += ControlledEntity.DeterminPosition(input);
        }
    }
}