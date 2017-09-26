using System;
using System.Text;
using SharpDX;


namespace Assignment4
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    using System.Collections.Generic;

    /// <summary>
    /// Simple ModelViewer game using SharpDX.Toolkit.
    /// </summary>
    public class ModelViewer : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private KeyboardManager keyboard;
        private KeyboardState keyBoardState;
        
        private SpriteBatch spriteBatch;
        private SpriteFont arial16Font;

        private Matrix view;
        private Matrix projection;

        private Buffer<VertexPositionNormalTexture> vertexBuffer;
        private Effect skyEffect;
        private BasicEffect basicEffect;
        private Effect cubeEffect;
        private VertexPositionNormalTexture[] vertices;
        private VertexInputLayout inputLayout;
        private Buffer<short> indexBuffer;
        private Texture2D[] textures = new Texture2D[6];

        //variables for changing view
        private Vector3 eyePosition = new Vector3(0, 0, 50); //starting camera location
        Vector3 up = new Vector3(0, 1, 0);
        Vector3 right = new Vector3(1, 0, 0);
        Vector3 forward = new Vector3(0, 0, -1);

        private float yawAngle = 0;
        private float pitchAngle = 0;
        private float rollAngle = 0;

        private GeometricPrimitive primitive;
        Cube[] cubes = new Cube[80];
        Cube skyCube = new Cube(new Random());

        private GeometricPrimitive ballPrimitive;
        Light[] lights = new Light[3];


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelViewer" /> class.
        /// </summary>
        public ModelViewer()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            keyboard = new KeyboardManager(this);
            keyBoardState = new KeyboardState();

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Modify the title of the window
            Window.Title = "Don't Let Me Get 80 Cubes"; //this is, fittingly, a sick reference to a rap song... a rap song about acquiring 80 drones in StarCraft 2

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //initialize the array of cubes
            Random random = new Random();
            for (int i = 0; i < cubes.Length; ++i )
                cubes[i] = new Cube(random);
            for (int i = 0; i < lights.Length; ++i)
                lights[i] = new Light(random);

                //Instantiate a SpriteBatch
                spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            // Loads a sprite font
            // The [Arial16.xml] file is defined with the build action [ToolkitFont] in the project
            arial16Font = Content.Load<SpriteFont>("Arial16");

            skyEffect = Content.Load<Effect>("skybox");
            skyEffect.Parameters["SkyBoxTexture"].SetResource(Content.Load<TextureCube>("SkyBoxTexture"));
            basicEffect = new BasicEffect(GraphicsDevice);
            cubeEffect = Content.Load<Effect>("point");

            //textures[0] = Content.Load<Texture2D>("gorko");
            //textures[1] = Content.Load<Texture2D>("hunsberger");
            //textures[2] = Content.Load<Texture2D>("kontostathis");
            //textures[3] = Content.Load<Texture2D>("mcdevitt");
            //textures[4] = Content.Load<Texture2D>("mustardo");
            //textures[5] = Content.Load<Texture2D>("young");

            
            vertices = new VertexPositionNormalTexture[4]
            { 
             new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.UnitZ, new Vector2(0, 0)), 
             new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.UnitZ, new Vector2(1, 0)),
             new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.UnitZ, new Vector2(0, 1)),
             new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.UnitZ, new Vector2(1, 1)) 
            };
            
            vertexBuffer = Buffer.New<VertexPositionNormalTexture>(GraphicsDevice, 4, BufferFlags.VertexBuffer);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            inputLayout = VertexInputLayout.New<VertexPositionNormalTexture>(0);

            short[] indices = new short[4] { 0, 1, 2, 3 };
            indexBuffer = Buffer.New<short>(GraphicsDevice, 4, BufferFlags.IndexBuffer);
            indexBuffer.SetData<short>(indices);

            primitive = ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice));
            Cube.LoadCube(GraphicsDevice, primitive);

            ballPrimitive = ToDisposeContent(GeometricPrimitive.Sphere.New(GraphicsDevice));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            keyBoardState = keyboard.GetState();

            var time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

            for (int i = 0; i < cubes.Length; ++i)
                cubes[i].UpdateCube(time);

            for (int i = 0; i < lights.Length; ++i)
                lights[i].UpdateLight(time);

            if (keyBoardState.IsKeyDown(Keys.Up))
                pitchAngle += .04f;
            if (keyBoardState.IsKeyDown(Keys.Down))
                pitchAngle -= .04f;
            if (keyBoardState.IsKeyDown(Keys.Left))
                yawAngle += .04f;
            if (keyBoardState.IsKeyDown(Keys.Right))
                yawAngle -= .04f;
            //rotational bonus content
            if (keyBoardState.IsKeyDown(Keys.U))
                rollAngle += .04f;
            if (keyBoardState.IsKeyDown(Keys.I))
                rollAngle -= .04f;

            Matrix3x3 rotation = (Matrix3x3)Matrix.RotationYawPitchRoll(yawAngle, pitchAngle, rollAngle);
            forward = Vector3.Transform(-Vector3.UnitZ, rotation);
            right = Vector3.Transform(Vector3.UnitX, rotation);
            up = Vector3.Transform(Vector3.UnitY, rotation);

            if (keyBoardState.IsKeyDown(Keys.W))
                eyePosition += .4f * forward;
            if (keyBoardState.IsKeyDown(Keys.S))
                eyePosition -= .4f * forward;
            if (keyBoardState.IsKeyDown(Keys.A))
                eyePosition -= .4f * right;
            if (keyBoardState.IsKeyDown(Keys.D))
                eyePosition += .4f * right;

            //easter-egg panning feature
            if (keyBoardState.IsKeyDown(Keys.Q))
                eyePosition += .4f * up;
            if (keyBoardState.IsKeyDown(Keys.E))
                eyePosition -= .4f * up;

            view = Matrix.LookAtRH(eyePosition, eyePosition + forward, up);

            // Calculates the world and the view based on the model size
            //view = Matrix.LookAtRH(new Vector3((float)(7.0 * Math.Sin(time)), (float)(3.0 * Math.Cos(time)), (float)(7.0 * Math.Cos(time))), Vector3.Zero, Vector3.UnitY);
            //view = Matrix.LookAtRH(new Vector3(0, 0, 7), Vector3.Zero, Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, .1f, 10000.0f);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            
            GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullFront);      


            //draw a skybox
            skyEffect.Parameters["World"].SetValue(Matrix.Scaling(500));
            skyEffect.Parameters["Projection"].SetValue(projection);
            skyEffect.Parameters["View"].SetValue(view);
            skyEffect.Parameters["CameraPosition"].SetValue(eyePosition);
            skyCube.Draw(skyEffect);

            GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);      

            //draw the cubes
            foreach (EffectTechnique effectTechnique in cubeEffect.Techniques)
            {
                foreach (EffectPass pass in effectTechnique.Passes)
                {

                    cubeEffect.Parameters["LightPosition"].SetValue(0, lights[0].position);
                    cubeEffect.Parameters["LightPosition"].SetValue(1, lights[1].position);
                    cubeEffect.Parameters["LightPosition"].SetValue(2, lights[2].position);
                    cubeEffect.Parameters["LightColor"].SetValue(0, lights[0].color.ToVector4());
                    cubeEffect.Parameters["LightColor"].SetValue(1, lights[1].color.ToVector4());
                    cubeEffect.Parameters["LightColor"].SetValue(2, lights[2].color.ToVector4());
                    Matrix world;

                    for (int i = 0; i < cubes.Length; ++i)
                    {
                        pass.Apply();
                        world = Matrix.Scaling(cubes[i].scale) * Matrix.Translation(cubes[i].position);
                        cubeEffect.Parameters["World"].SetValue(world);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world));
                        cubeEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        cubeEffect.Parameters["Projection"].SetValue(projection);
                        cubeEffect.Parameters["View"].SetValue(view);
                        cubeEffect.Parameters["Camera"].SetValue(eyePosition);
                        cubeEffect.Parameters["DiffuseColor"].SetValue(cubes[i].color.ToVector4());
                        cubes[i].Draw(cubeEffect);
                    }
                }
            }

            //draw the lights
            basicEffect.Projection = projection;
            basicEffect.View = view;
            basicEffect.LightingEnabled = true;
            basicEffect.EnableDefaultLighting();
            for (int i = 0; i < 3; ++i )
            {
                basicEffect.World = Matrix.Scaling(5) * Matrix.Translation(lights[i].position);
                basicEffect.DiffuseColor = lights[i].color.ToVector4();
                basicEffect.EmissiveColor = lights[i].color.ToVector3();
                basicEffect.SpecularColor = Vector3.Zero;
                ballPrimitive.Draw(basicEffect);
            }
            spriteBatch.Begin();

            var text = new StringBuilder("Text is working~").AppendLine();

            List<Keys> keys = new List<Keys>();

            //Display pressed keys
            keyBoardState.GetDownKeys(keys);
            text.Append("Key Pressed: [");
            foreach (var key in keys)
            {
                text.Append(key.ToString());
                text.Append(" ");
            }
            text.Append("]").AppendLine();
            spriteBatch.DrawString(arial16Font, text.ToString(), new Vector2(16, 16), Color.White);
            spriteBatch.End();
     
            base.Draw(gameTime);
        }
    }
}
