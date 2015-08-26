using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.InGame
{
    public class QuadRenderer : MineLibComponent
    {
        private readonly VertexPositionTexCoordRayIndex[] _quadVertices;
        private readonly short[] _indexBuffer;

        public QuadRenderer(Client game) : base(game)
        {
            _quadVertices = new[]
            {
                new VertexPositionTexCoordRayIndex(new Vector3(0, 0, 0), new Vector3(1, 1, 0)),
                new VertexPositionTexCoordRayIndex(new Vector3(0, 0, 0), new Vector3(0, 1, 1)),
                new VertexPositionTexCoordRayIndex(new Vector3(0, 0, 0), new Vector3(0, 0, 3)),
                new VertexPositionTexCoordRayIndex(new Vector3(0, 0, 0), new Vector3(1, 0, 2))
            };
            _indexBuffer = new short[] {0, 1, 2, 2, 3, 0};
        }

        public void RenderFullScreenQuad(Effect effect) { RenderQuad(Vector2.One * -1, Vector2.One, effect); }

        public void RenderQuad(Vector2 v1, Vector2 v2, Effect effect)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _quadVertices[0].Position = new Vector3(v2.X, v1.Y, 0);
                _quadVertices[1].Position = new Vector3(v1.X, v1.Y, 0);
                _quadVertices[2].Position = new Vector3(v1.X, v2.Y, 0);
                _quadVertices[3].Position = new Vector3(v2.X, v2.Y, 0);

                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quadVertices, 0, 4, _indexBuffer,
                    0, 2);
            }
        }



        private struct VertexPositionTexCoordRayIndex : IVertexType
        {
            public Vector3 Position { get; set; }
            public Vector3 TextureCoordinate { get; set; }

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration (
                new VertexElement(0,    VertexElementFormat.Vector3,    VertexElementUsage.Position,            0),
                new VertexElement(12,   VertexElementFormat.Vector3,    VertexElementUsage.TextureCoordinate,   0));

            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

            public VertexPositionTexCoordRayIndex(Vector3 position, Vector3 texcoordRayindex) : this()
            {
                Position = position;
                TextureCoordinate = texcoordRayindex;
            }
        }


        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime) { }

        public override void Dispose() { }
    }
}
