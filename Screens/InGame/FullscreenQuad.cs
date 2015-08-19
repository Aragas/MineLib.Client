using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PGL.Screens.InGame
{
    internal class FullscreenQuad
    {
        //Vertex Buffer
        private readonly VertexBuffer _vb;
        //Index Buffer
        private readonly IndexBuffer _ib;
        //Constructor
        public FullscreenQuad(GraphicsDevice graphicsDevice)
        {
            //Vertices
            VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };
            //Make Vertex Buffer
            _vb = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.None);
            _vb.SetData(vertices);
            //Indices
            ushort[] indices = {0, 1, 2, 2, 3, 0};
            //Make Index Buffer
            _ib = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            _ib.SetData(indices);
        }

        //Draw and Set Buffers
        public void Draw(GraphicsDevice graphicsDevice)
        {
            //Set Vertex Buffer
            graphicsDevice.SetVertexBuffer(_vb);
            //Set Index Buffer
            graphicsDevice.Indices = _ib;
            //Draw Quad
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

        //Set Buffers Onto GPU
        public void ReadyBuffers(GraphicsDevice graphicsDevice)
        {
            //Set Vertex Buffer
            graphicsDevice.SetVertexBuffer(_vb);
            //Set Index Buffer
            graphicsDevice.Indices = _ib;
        }

        //Draw without Setting Buffers
        public void JustDraw(GraphicsDevice graphicsDevice)
        {
            //Draw Quad
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }
    }
}
