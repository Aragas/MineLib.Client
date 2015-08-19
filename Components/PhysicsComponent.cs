using Microsoft.Xna.Framework;

namespace MineLib.PGL.Components
{
    public class PhysicsComponent : GameComponent
    {
        private Data.World World { get; set; }

        public PhysicsComponent(Game game) : base(game) { }
    }
}