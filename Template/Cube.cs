using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    using SharpDX.Toolkit.Graphics;
    class Cube
    {
        public Vector3 position;
        public float scale;
        public Color color;
        public Vector3 velocity;
        public static Buffer<VertexPositionNormalTexture> vertexBuffer;
        public static VertexInputLayout inputLayout;// = VertexInputLayout.New<VertexPositionNormalTexture>(0);
        public static GeometricPrimitive primitive;
        public static GraphicsDevice device;

        public Cube(Random random)
        {
            position = new Vector3((float)(random.NextDouble() * 180 - 90), (float)(random.NextDouble() * 180 - 90), (float)(random.NextDouble() * 180 - 90)); //a random position inside the arena
            scale = (float)(random.NextDouble() * 16 + 4);
            color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            velocity = new Vector3((float)(random.NextDouble() * 2 - 1));
            velocity.Normalize();
            velocity *= (float)(random.NextDouble() * 5 + 5);
        }

        public static void LoadCube( GraphicsDevice newDevice, GeometricPrimitive shape )
        {
            device = newDevice;
            var vertices = new VertexPositionNormalTexture[4]
            { 
             new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.UnitZ, new Vector2(0, 0)), 
             new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.UnitZ, new Vector2(1, 0)),
             new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.UnitZ, new Vector2(0, 1)),
             new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.UnitZ, new Vector2(1, 1)) 
            };

            vertexBuffer = Buffer.New<VertexPositionNormalTexture>(device, 4, BufferFlags.VertexBuffer);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            primitive = shape;
        }

        public void Draw(Effect effect)
        {
            //device.SetVertexBuffer<VertexPositionNormalTexture>(vertexBuffer);
            //device.SetVertexInputLayout(inputLayout);

            primitive.Draw(effect);
        }

        public void UpdateCube(float time)
        {
            position += velocity*time;
            if ( position.Y >90 || position.Y < -90 )
                velocity.Y = velocity.Y * -1;
            if (position.X > 90 || position.X < -90)
                velocity.X = velocity.X * -1;
            if (position.Z > 90 || position.Z < -90)
                velocity.Z = velocity.Z * -1;
        }
    }
}
