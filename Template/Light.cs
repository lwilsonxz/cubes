using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    class Light
    {
        public Vector3 position;
        public Color color;
        public Vector3 velocity;

        public Light(Random random)
        {
            position = new Vector3((float)(random.NextDouble() * 180 - 90), (float)(random.NextDouble() * 180 - 90), (float)(random.NextDouble() * 180 - 90)); //a random position inside the arena
            color = new Color((float)(random.NextDouble() * .5 + .5), (float)(random.NextDouble() * .5 + .5), (float)(random.NextDouble() * .5 + .5));
            velocity = new Vector3((float)(random.NextDouble() * 2 - 1));
            velocity.Normalize();
            velocity *= (float)(random.NextDouble() * 5 + 5);
        }
         
        public void UpdateLight(float time)
        {
            position += velocity * time;
            if (position.Y > 90 || position.Y < -90)
                velocity.Y = velocity.Y * -1;
            if (position.X > 90 || position.X < -90)
                velocity.X = velocity.X * -1;
            if (position.Z > 90 || position.Z < -90)
                velocity.Z = velocity.Z * -1;
        }
    }
}
