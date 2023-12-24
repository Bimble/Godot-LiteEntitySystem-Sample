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
    }
}