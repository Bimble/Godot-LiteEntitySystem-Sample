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
            ControlledEntity.Position += 200 * new System.Numerics.Vector2(input.X, input.Y) * EntityManager.DeltaTimeF;
        }
    }
}