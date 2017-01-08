using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Camera;
using Sprites;

namespace SimpleCamera
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
       
        AnimeSprite _player;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SimpleCam cam;
        private Texture2D background;
       // private Texture2D character;
        private float speed = 5f;
        //private Vector2 CharacterPosition;
        private SpriteFont font;
        private Vector2 WorldCamLimit;
    
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 780;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("bigback3000x3000");
            _player = new AnimeSprite(Content.Load<Texture2D>("spindash"), new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 6);
            //character = Content.Load<Texture2D>("right arrow");
            cam = new SimpleCam(GraphicsDevice.Viewport);
            font = Content.Load<SpriteFont>("debug");

            WorldCamLimit = new Vector2(background.Width - GraphicsDevice.Viewport.Width,
            background.Height - GraphicsDevice.Viewport.Height);

           
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            // this.Exit();
            // if (Keyboard.GetState().IsKeyDown(Keys.D))
            //cam.Move(new Vector2(1, 0) * speed);
            // if (Keyboard.GetState().IsKeyDown(Keys.A))
            //  cam.Move(new Vector2(-1, 0) * speed);
            // if (Keyboard.GetState().IsKeyDown(Keys.W))
            //  cam.Move(new Vector2(0, -1) * speed);
            //if (Keyboard.GetState().IsKeyDown(Keys.S))
            // cam.Move(new Vector2(0, 1) * speed);
            // rotation
            // if (Keyboard.GetState().IsKeyDown(Keys.X))
            //  cam.Rotate(0.01f);
            // if (Keyboard.GetState().IsKeyDown(Keys.Z))
            //  cam.Rotate(-0.01f);
            // zooming
            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            // cam.Zoom(0.01f);
            // if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            //   cam.Zoom(-0.01f);

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _player.Move(new Vector2(1, 0) * speed);
                cam.Move(new Vector2(1, 0) * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            { 
                _player.Move(new Vector2(-1, 0) * speed);
                cam.Move(new Vector2(-1, 0) * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _player.Move(new Vector2(0, -1) * speed);
                cam.Move(new Vector2(0, -1) * speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _player.Move(new Vector2(0, 1) * speed);
                cam.Move(new Vector2(0, 1) * speed);
            }

            //cam.pos = Vector2.Clamp(cam.pos, Vector2.Zero, WorldCamLimit);
            cam.pos = Vector2.Clamp(cam.GetCameraCoordinates(_player.position), Vector2.Zero, WorldCamLimit);
            _player.Update(gameTime);
            _player.position = Vector2.Clamp(_player.position, Vector2.Zero,
               new Vector2(background.Width - _player.SpriteWidth,
                background.Height - _player.SpriteHeight));

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
           
            spriteBatch.DrawString(font, cam.pos.X.ToString() + " Y: " + cam.pos.Y.ToString(),
                new Vector2(20, 20), Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Texture,
                                BlendState.AlphaBlend,
                                null,null,null,null,cam.get_transformation(GraphicsDevice));
            //spriteBatch.DrawString(font, "Character X:" + CharacterPosition.X.ToString() + 
                                        // " Y: " + CharacterPosition.Y.ToString(),
                // cam.GetCameraCoordinates(CharacterPosition) + new Vector2(0, -20), Color.White);
          spriteBatch.Draw(background,
                     cam.GetCameraCoordinates(Vector2.Zero), Color.White);
            // spriteBatch.Draw(character, cam.GetCameraCoordinates(CharacterPosition) , Color.White);


            _player.Draw(spriteBatch, cam);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
