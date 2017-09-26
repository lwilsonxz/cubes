using System;
using System.Text;
using SharpDX;


namespace Shaders
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    /// <summary>
    /// Simple Shaders game using SharpDX.Toolkit.
    /// </summary>
    public class Shaders : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private SpriteFont arial16Font;

        private Matrix view;
        private Matrix projection;

        private Model model;
        private Texture2D shipTexture;
        
        private Effect effect;
        private float intensity = 0;
        private Vector3 light;
        private Vector3 camera = new Vector3(0.0f, 2.0f, 7.0f);
        /// <summary>
        /// Initializes a new instance of the <see cref="Shaders" /> class.
        /// </summary>
        public Shaders()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Modify the title of the window
            Window.Title = "Shaders";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Instantiate a SpriteBatch
            spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));

            // Loads a sprite font
            // The [Arial16.xml] file is defined with the build action [ToolkitFont] in the project
            arial16Font = Content.Load<SpriteFont>("Arial16");

            // Load a 3D model
            // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
            model = Content.Load<Model>("Ship");
         

            shipTexture = Content.Load<Texture2D>("ShipDiffuse");            
                       
            //effect = Content.Load<Effect>("ambient");
            //effect = Content.Load<Effect>("diffuse");
            //effect = Content.Load<Effect>("specular");
            //effect = Content.Load<Effect>("point");
            effect = Content.Load<Effect>("texture");
            

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

             var time = (float)gameTime.TotalGameTime.TotalSeconds;

           intensity += 1f;
           if (intensity > 4000f)
                intensity = 0f;

           //intensity = .1f;


            // Calculates the world and the view based on the model size
            view = Matrix.LookAtRH(camera, new Vector3(0, 0.0f, 0), Vector3.UnitY);
            projection = Matrix.PerspectiveFovRH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);

            light = new Vector3((float)(Math.Cos(time) * 5), 0, (float)(Math.Sin(time) * 5));


        }

        protected override void Draw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // ------------------------------------------------------------------------
            // Draw the 3d model
            // ------------------------------------------------------------------------
            var world = Matrix.Scaling(0.003f) *
                        Matrix.RotationY(MathUtil.DegreesToRadians(135)) *
                        //Matrix.RotationZ(time/3) *
                        Matrix.Translation(0, 0, 2.0f);

            foreach (EffectTechnique technique in effect.Techniques)
                foreach (EffectPass pass in technique.Passes)
                {
                    pass.Apply();
                    DrawModelWithEffect(model, world, view, projection, effect);
                }

            
         

            // ------------------------------------------------------------------------
            // Draw some 2d text
            // ------------------------------------------------------------------------
            spriteBatch.Begin();
            var text = new StringBuilder("Shininess: " + intensity).AppendLine();

            spriteBatch.DrawString(arial16Font, text.ToString(), new Vector2(16, 16), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {                    
                    part.Effect = effect;
                    
  
                    effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["AmbientIntensity"].SetValue<float>(.1f);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

                    effect.Parameters["Camera"].SetValue<Vector3>(camera);
                    effect.Parameters["Shininess"].SetValue<float>((float)Math.Sqrt(intensity));
                    effect.Parameters["ModelTexture"].SetResource<Texture2D>(shipTexture);

                    //effect.Parameters["LightPosition"].SetValue<Vector3>(light);
                    
                    
                    
                                        
                    part.Draw(GraphicsDevice);
                }
            }
        }
    }
}
