using LiteEntitySystem;
using System.Numerics;

namespace Sample.Shared
{
    public class BasePlayer : PawnLogic
    {
        [SyncVarFlags(SyncFlags.Interpolated | SyncFlags.LagCompensated)]
        public SyncVar<Vector2> Position;

        public BasePlayer(EntityParams entityParams) : base(entityParams)
        {
        }

        public Vector2 DeterminPosition(in PlayerInputPacket input)
        {
            return 200 * new Vector2(input.X, input.Y) * EntityManager.DeltaTimeF;
        }
    }
}