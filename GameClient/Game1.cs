using System;
using GameData;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace GameClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IHubProxy proxy;
        HubConnection connection;
        private bool connected;
        private string message;
        private string playerValidationMessage;
        private string errorMessage;
        private PlayerData playerData;
        private List<PlayerData> allLogedInPlayers = new List<PlayerData>();

        enum GAMESTATE
        {
            MAINMENU,
            LOGIN,
            PLAYING,
            SCOREBOARD,
        }

        GAMESTATE CurrentGameState = GAMESTATE.MAINMENU;
        LogInButton btnLogIn;
        LogInButton btnSubmit;
        Texture2D backgroundTexture;
        Texture2D menuTexture;

        private GetGameInputComponent loginKeyboard;
        string gamerTag = string.Empty;
        string password = string.Empty;
        private bool loggedIn = false;
        private bool loggedInFailed = false;

        private Texture2D background; //making game background
        bool gameStart = false;
        Vector2 centreScreen;
        int screenWidth;
        int screenHeight;
        int lives = 3;
        string scoreMessage = "";
        Player myPlayer;
        ChasingEnemy[] chasers;
        //Sprite[] collectables = new Sprite[20];
        Sprite collectable;
        List<Sprite> collectables = new List<Sprite>();
        Sprite cherry;
        bool hasAdded;
        List<Wall> walls = new List<Wall>();
        Player otherPlayer;
        private List<Player> OtherPlayers = new List<Player>();
        List<CollectableData> allCollectables = new List<CollectableData>();
        bool q, w, e, r;
        public bool up, down, left, right;
        private string textMessage;
        private FadeText groupMessage;

        public SpriteFont GameFont { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
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
            //Graphics Settings {Wasn't needed but was used for testing at one point
            //graphics.PreferredBackBufferHeight = 800;
            //graphics.PreferredBackBufferWidth = 600;


            //Virtual Keyboard Initialize
            Helpers.GraphicsDevice = GraphicsDevice;
            loginKeyboard = new GetGameInputComponent(this);
            loginKeyboard.Visible = false;

            connection = new HubConnection("http://localhost:5864/");
            proxy = connection.CreateHubProxy("GameHub");
            message = "Connecting............";
            connection.StateChanged += Connection_StateChanged;
            connection.Start();
            base.Initialize();
        }

        private void Connection_StateChanged(StateChange state)
        {
            switch (state.NewState)
            {
                case ConnectionState.Connected:
                    connected = true;
                    message = "Connected to Game Hub ";
                    subscribeToMessages();
                    getCollectableData();
                    break;
            }
        }

        private void subscribeToMessages()
        {
            Action<ErrorMess> err = ShowError;
            proxy.On("error", err);

            Action<string> ErrorSent = error_Sent;
            proxy.On("Error", ErrorSent);

            Action<PlayerData> ValidatePlayer = valid_Player;
            proxy.On("PlayerValidated", ValidatePlayer);

            Action<Joined> AllPlayersStartingPositions = valid_Positions;
            proxy.On("PlayersStartingPositions", AllPlayersStartingPositions);

            Action<MoveMessage> AllPlayersPositions = valid_NewPositions;
            proxy.On("PlayersStartingPositions", AllPlayersPositions);

            Action<string> SendGroupMessage = valid_GroupMessage;
            proxy.On("ShowGroupMessage", SendGroupMessage);
            
            // all other messages from Server go here
        }

        private void valid_GroupMessage(string textMessage)
        {
            groupMessage = new FadeText(this, Vector2.Zero, textMessage );
        }

        private void valid_NewPositions(MoveMessage newPosition)
        {
            foreach (Player op in OtherPlayers)
            {
                if (op.id.ToString() == newPosition.playerID)
                    op.position = new Vector2(newPosition.NewX, newPosition.NewY);
            }
        }


        private void valid_Positions(Joined sentOtherPlayer)
        {
            foreach (var player in allLogedInPlayers)
            {
                if (player.PlayerID.ToString() == sentOtherPlayer.playerID)
                    if (sentOtherPlayer.imageName == "characterSpriteSheet1")
                    {
                        otherPlayer = new Player(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\characterSpriteSheet1"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, new Vector2(
                                                    sentOtherPlayer.X, sentOtherPlayer.Y), 2, 9f);

                        if (!hasAdded)
                        {
                            OtherPlayers.Add(otherPlayer);
                            hasAdded = true;
                        }
                    }
                    else if (sentOtherPlayer.imageName == "character2SspriteSheet")
                    {
                        otherPlayer = new Player(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\character2SspriteSheet"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, new Vector2(
                                                    sentOtherPlayer.X, sentOtherPlayer.Y), 2, 6f);

                        if (!hasAdded)
                        {
                            OtherPlayers.Add(otherPlayer);
                            hasAdded = true;
                        }

                    }
                    else if (sentOtherPlayer.imageName == "characterSpriteSheet3")
                    {
                        otherPlayer = new Player(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\characterSpriteSheet3"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, new Vector2(
                                                    sentOtherPlayer.X, sentOtherPlayer.Y), 2, 6f);
                        //otherPlayer.id = player.PlayerID;
                        //foreach (Player oPlayer in OtherPlayers)
                        //{
                        //    if (oPlayer.id != player.PlayerID)
                        //        OtherPlayers.Add(otherPlayer);
                        //}
                        if (!hasAdded)
                        {
                            OtherPlayers.Add(otherPlayer);
                            hasAdded = true;
                        }
                    }
                    else if (sentOtherPlayer.imageName == "character4SpriteSheet")
                    {
                        otherPlayer = new Player(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\character4SpriteSheet"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, new Vector2(
                                                    sentOtherPlayer.X, sentOtherPlayer.Y), 2, 4.5f);
                        if (!hasAdded)
                        {
                            OtherPlayers.Add(otherPlayer);
                            hasAdded = true;
                        }

                    }

            }
        }
        #region Action delegates for incoming server messages
        private void ShowError(ErrorMess em)
        {
            message = em.message;
        }
        #endregion


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameFont = Content.Load<SpriteFont>("GameFont");
            Services.AddService<SpriteFont>(GameFont);
            Services.AddService<SpriteBatch>(spriteBatch);
            new FadeTextManager(this);
            IsMouseVisible = true;

            //MainMenu's Content Load
            btnLogIn = new LogInButton(Content.Load<Texture2D>(@"OtherAssets\LogIn"), graphics.GraphicsDevice);

            btnSubmit = new LogInButton(Content.Load<Texture2D>(@"OtherAssets\WelcomeMat"), graphics.GraphicsDevice);

            //Moves the position of the button 
            btnLogIn.setPosition(new Vector2(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 6,
                    GraphicsDevice.Viewport.Height / 2));

            btnSubmit.setPosition(new Vector2(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 6,
                    GraphicsDevice.Viewport.Height / 2));

            //Textures
            backgroundTexture = Content.Load<Texture2D>(@"OtherAssets\Background");
            menuTexture = Content.Load<Texture2D>(@"OtherAssets\MainMenu");
            // TODO: use this.Content to load your game content here

            //load in background image
            background = Content.Load<Texture2D>(@"Backgrounds\BackGround");

            cherry = new Sprite(Content.Load<Texture2D>(@"Collectables and Exit Image\Cherry"), new Vector2(0, 0), 1);

            //centre vector
            centreScreen = new Vector2(GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            SetCollectablePositions();

            //creating an array to hold a random amount of chasers between 5 and 10
            chasers = new ChasingEnemy[Utility.NextRandom(5, 10)];

            //creating a chaser depending on the number 
            //generated from the random size of the array
            for (int i = 0; i < chasers.Length; i++)
            {
                chasers[i] = new ChasingEnemy(Content.Load<Texture2D>(@"Collectables and Exit Image/movingCollectable"),
                        new Vector2(Utility.NextRandom(GraphicsDevice.Viewport.Width),
                            Utility.NextRandom(GraphicsDevice.Viewport.Height)), 2);
                chasers[i].Velocity = (float)Utility.NextRandom(2, 5);
                chasers[i].CollisionDistance = Utility.NextRandom(3, 5);
            }

            MakeWalls();
        }

        private void SetCollectablePositions()
        {
            //creating a collectable in a randomly generated 
            //position to match the size of the collectables array
            foreach (CollectableData collections in allCollectables)
            {
                collectable = new Sprite(Content.Load<Texture2D>(@"Collectables and Exit Image\Collectable"),
                    new Vector2(collections.X, collections.Y), 1);

                collectable.CollectableScore = collections.collectableValue;
                collectables.Add(collectable);
            }
        }

        private void MakeWalls()
        {
            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                            new Vector2(centreScreen.X + 80, centreScreen.Y + 100),
                            new Vector2(500, 5)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X + 80, centreScreen.Y - 100),
                new Vector2(500, 5)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X - 580, centreScreen.Y + 100),
                new Vector2(500, 5)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X - 580, centreScreen.Y - 100),
                new Vector2(500, 5)));



            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X + 200, centreScreen.Y + 100),
                new Vector2(5, 200)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X - 200, centreScreen.Y + 100),
                new Vector2(5, 200)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X - 200, centreScreen.Y - 300),
                new Vector2(5, 200)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X + 200, centreScreen.Y - 300),
                new Vector2(5, 200)));



            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X + 400, centreScreen.Y),
                new Vector2(500, 5)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X - 900, centreScreen.Y),
                new Vector2(500, 5)));


            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X, centreScreen.Y + 200),
                new Vector2(5, 500)));

            walls.Add(new Wall(Content.Load<Texture2D>(@"Backgrounds/blackBox"),
                new Vector2(centreScreen.X, centreScreen.Y - 700),
                new Vector2(5, 500)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (!connected) return;

            MouseState mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GAMESTATE.MAINMENU:
                    if (btnLogIn.isClicked == true)
                        CurrentGameState = GAMESTATE.LOGIN;

                    btnLogIn.Update(mouse);
                    break;


                case GAMESTATE.LOGIN:
                    //Activates the login Keyboard
                    CheckLogedInPlayers();

                    if (InputEngine.IsKeyPressed(Keys.F10) && !loginKeyboard.Visible)
                    {
                        loginKeyboard.Visible = true;
                        //Clears anything written before this point
                        InputEngine.ClearState();
                    }

                    if (loginKeyboard.Done)
                    {
                        gamerTag = loginKeyboard.Name;
                        password = loginKeyboard.Password;
                        loginKeyboard.Clear();
                        InputEngine.ClearState();

                        //checks to see if the connection is connected
                        if (connection.State == ConnectionState.Connected)
                            if (gamerTag != null && password != null)
                            {
                                getPlayer();
                                subscribeToMessages();
                                CheckLogedInPlayers();
                                //getPlayerData();                
                            }

                    }

                    if (loggedIn == true)
                    {
                        if (allLogedInPlayers.Count >= 2)
                            if (btnSubmit.isClicked == true)
                                CurrentGameState = GAMESTATE.PLAYING;

                        btnSubmit.Update(mouse);
                    }

                    break;

                case GAMESTATE.PLAYING:

                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    {
                        myPlayer = new speedyPlayer(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\characterSpriteSheet1"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, centreScreen, 2, 9f);
                        gameStart = true;
                        q = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        myPlayer = new ladyPlayer(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\character2SspriteSheet"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, centreScreen, 2, 6f);
                        gameStart = true;
                        w = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.E))
                    {
                        myPlayer = new healthyPlayer(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\characterSpriteSheet3"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, centreScreen, 2, 6f);
                        gameStart = true;
                        e = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        myPlayer = new sneakPlayer(new Texture2D[] {Content.Load<Texture2D>(@"Player Images\character4SpriteSheet"),
                                                Content.Load<Texture2D>(@"Player Images\cherryPlayer")}, centreScreen, 2, 4.5f);
                        gameStart = true;
                        r = true;
                    }

                    if (gameStart) // play the game
                    {

                        Vector2 StartingPos = myPlayer.position;

                        sendNewPlayerStartingPosition(centreScreen);

                        //updating the player
                        myPlayer.Update(gameTime);

                        sendNewPlayerPosition(myPlayer.position);
                        subscribeToMessages();

                        foreach (Player othPlayer in OtherPlayers)
                        {
                            othPlayer.Update(gameTime);
                        }

                        cherry.Update(gameTime);

                        if (cherry.visible == true)
                        {
                            if (myPlayer.collisionDetect(cherry))
                            {
                                cherry.visible = false;
                                myPlayer.cherryform = true;
                            }
                        }

                        // no going through the walls
                        foreach (Wall w in walls)
                        {
                            if (w.visible)
                            {
                                if (w.collisionDetect(myPlayer))
                                {
                                    if (myPlayer.sneaky == false)
                                    {
                                        myPlayer.position = StartingPos;
                                    }
                                }
                            }
                        }


                        //clamping the player to the border
                        myPlayer.position = Vector2.Clamp(myPlayer.position, Vector2.Zero,
                            (new Vector2(screenWidth, screenHeight)
                            - new Vector2(myPlayer.SpriteWidth, myPlayer.SpriteHeight)));

                        //updating the collectables
                        for (int i = 0; i < collectables.Count; i++)
                        {
                            collectables[i].Update(gameTime);
                        }

                        //updating the collectables collision detection
                        for (int i = 0; i < collectables.Count; i++)
                        {
                            if (myPlayer.collisionDetect(collectables[i]))
                            {
                                if (collectables[i].visible)
                                {

                                    //    collectedAmount++;
                                    myPlayer.score += collectables[i].CollectableScore;
                                    collectables[i].visible = false;
                                }
                            }
                        }

                        //updating collision detect for chasers
                        for (int i = 0; i < chasers.Length; i++)
                        {
                            if (myPlayer.collisionDetect(chasers[i]))
                            {
                                if (chasers[i].visible)
                                {
                                    //   myPlayer.health -= Utility.NextRandom(40, 60);
                                    myPlayer.score += Utility.NextRandom(25, 40);
                                }
                                chasers[i].visible = false;
                            }
                        }

                        //dealing with health when hitting enemies
                        if (myPlayer.health <= 0)
                        {
                            lives -= 1;
                            myPlayer.position = centreScreen;
                            myPlayer.health = 100;
                        }

                        //updating chasers
                        foreach (ChasingEnemy chaser in chasers)
                        {
                            chaser.follow(myPlayer);
                            chaser.Update(gameTime);

                            chaser.position = Vector2.Clamp(chaser.position, Vector2.Zero,
                            (new Vector2(screenWidth, screenHeight)
                            - new Vector2(chaser.SpriteWidth, chaser.SpriteHeight)));
                        }

                        //turning the score int into a string so we can write it
                        scoreMessage = Convert.ToString(myPlayer.score);

                        if (Keyboard.GetState().IsKeyDown(Keys.F12))
                        {
                            typeMessage();
                        }
                    }

                    break;
                    // TODO: Add your update logic here
            }
            base.Update(gameTime);
        }

        private void typeMessage()
        {
            loginKeyboard.Visible = true;
            //Clears anything written before this point
            InputEngine.ClearState();

            if (loginKeyboard.Done)
            {
                textMessage = loginKeyboard.Name;
                loginKeyboard.Clear();
                InputEngine.ClearState();

                //checks to see if the connection is connected
                if (textMessage != null)
                {
                    sendAllClientMessage(textMessage);           
                }

            }
        }

        private void sendAllClientMessage(string textMessage)
        {
            proxy.Invoke("SendGroupMessage", new string[] { textMessage });
        }

        private void sendNewPlayerPosition(Vector2 position)
        {
            float x = position.X;
            float y = position.Y;

            proxy.Invoke("AllPlayersPositions", new object[] { x, y, playerData.PlayerID, });
        }

        private void sendNewPlayerStartingPosition(Vector2 StartingPosition)
        {
            float x = StartingPosition.X;
            float y = StartingPosition.Y;
            string ImageName = string.Empty;

            if (q)
                ImageName = "characterSpriteSheet1";
            else if (w)
                ImageName = "character2SspriteSheet";
            else if (e)
                ImageName = "characterSpriteSheet3";
            else if (r)
                ImageName = "character4SpriteSheet";

            proxy.Invoke("AllPlayersStartingPositions", new object[] { x, y, playerData.PlayerID, ImageName });
        }

        private void getPlayers()
        {
            proxy.Invoke("AllLogedInPlayers", new string[] { gamerTag, password });
        }

        private void CheckLogedInPlayers()
        {
            Action<List<PlayerData>> AllLogedInPlayers = ShowPlayers;
            proxy.On("PlayersValidated", AllLogedInPlayers);
        }

        private void ShowPlayers(List<PlayerData> LogedInPlayers)
        {
            loggedIn = true;

            //if(LogedInPlayers.Contains(playerData))

            allLogedInPlayers = LogedInPlayers;
        }

        private void getPlayer()
        {
            proxy.Invoke("ValidatePlayer", new string[] { gamerTag, password });
        }

        private void valid_Player(PlayerData player)
        {
            loggedIn = true;
            playerData = player;

            playerValidationMessage = "PlayerValidated GamerTag is " + player.GamerTag;

            getPlayers();
        }

        private void error_Sent(string ErrorMessage)
        {
            loggedInFailed = true;
            errorMessage = ErrorMessage;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GAMESTATE.MAINMENU:
                    GraphicsDevice.Clear(Color.Red);

                    spriteBatch.Draw(backgroundTexture, new Rectangle(
                        0,
                        0,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height),
                        Color.White);

                    spriteBatch.Draw(menuTexture, new Rectangle(
                        GraphicsDevice.Viewport.Width / 2 - menuTexture.Width / 2,
                        30,
                        GraphicsDevice.Viewport.Width / 2 - menuTexture.Width / 2,
                        GraphicsDevice.Viewport.Height / 2 - GraphicsDevice.Viewport.Height / 4),
                        Color.White);

                    spriteBatch.DrawString(GameFont, message, new Vector2(20, 20), Color.White);

                    btnLogIn.Draw(spriteBatch);
                    //btnSubmit.Draw(spriteBatch);

                    break;

                case GAMESTATE.LOGIN:
                    GraphicsDevice.Clear(Color.Black);
                    string helperMessage = "Press F10";
                    spriteBatch.DrawString(GameFont, helperMessage, new Vector2(500, 20), Color.White);

                    if (loggedIn != false || loggedInFailed != false)
                        if (playerData != null)
                            spriteBatch.DrawString(GameFont, playerValidationMessage, new Vector2(200, 50), Color.White);
                    //else
                    //spriteBatch.DrawString(GameFont, errorMessage, new Vector2(200, 50), Color.White);

                    int count = 0;

                    if (allLogedInPlayers != null)
                    {

                        foreach (PlayerData player in allLogedInPlayers)
                        {
                            string playerMessage = "Player " + player.GamerTag + " is Connected";

                            if (player.GamerTag != playerData.GamerTag)
                                spriteBatch.DrawString(GameFont, playerMessage, new Vector2(200, 60 + count), Color.White);

                            count += 30;
                        }
                    }

                    if (allLogedInPlayers.Count >= 2)
                        btnSubmit.Draw(spriteBatch);
                    break;

                case GAMESTATE.PLAYING:

                    GraphicsDevice.Clear(Color.Beige);

                    //spriteBatch.Begin();

                    spriteBatch.DrawString(GameFont, "welcome", centreScreen, Color.Black);
                    spriteBatch.DrawString(GameFont, " press Q for speed player\n press W for lady player \n press E for healthy player \n press R for sneaky player", new Vector2(centreScreen.X + 200, centreScreen.Y), Color.Black);

                    if (gameStart)
                    {
                        GraphicsDevice.Clear(Color.CornflowerBlue);

                        //spriteBatch.Begin();
                        spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

                        //drawing player stats
                        spriteBatch.DrawString(GameFont, ("Lives: " + lives + " Health: " + myPlayer.health + " Score: " + scoreMessage), new Vector2(10, -5) + myPlayer.position, Color.White);

                        cherry.Draw(spriteBatch);

                        //these are made from the AnimatedSprite class 
                        //which has its own draw so its not in the spritebatch

                        //drawing player
                        myPlayer.Draw(spriteBatch);

                        //if (OtherPlayers != null && OtherPlayers.Count > 0)
                        foreach (Player othPlayer in OtherPlayers)
                        {
                            othPlayer.Draw(spriteBatch);
                        }

                        //drawing colectables
                        for (int i = 0; i < collectables.Count; i++)
                        {
                            collectables[i].Draw(spriteBatch);
                        }

                        //drawing chasers
                        foreach (ChasingEnemy chaser in chasers)
                        {
                            chaser.Draw(spriteBatch);
                        }


                        foreach (Wall w in walls)
                        {
                            w.Draw(spriteBatch);
                        }
                    }

                    break;

            }
            ////spriteBatch.DrawString(GameFont, message, new Vector2(20, 20), Color.White);
            //if (playerData != null)
            //{
            //    spriteBatch.DrawString(GameFont, playerData.GamerTag + " is online ", new Vector2(20, 20), Color.White);
            //}

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //private void getPlayerData()
        //{
        //    proxy.Invoke<PlayerData>("getPlayer", new string[] { gamerTag }).ContinueWith(t => { playerData = t.Result; });
        //}

        private void getCollectableData()
        {
            int WorldX = GraphicsDevice.Viewport.Width;
            int WorldY = GraphicsDevice.Viewport.Height;
            int count = 10;

            proxy.Invoke<List<CollectableData>>("GetCollectables", new object[] { count, WorldX, WorldY }).ContinueWith(t =>
            {
                CreateGameCollecables(t.Result);
            });
        }

        private void CreateGameCollecables(List<CollectableData> result)
        {

            foreach (CollectableData c in result)
            {
                new FadeText(this, Vector2.Zero,
                    "Delivered " + c.CollectableName + " X: " + c.X.ToString() + " Y: " + c.X.ToString());

                allCollectables.Add(c);
            }
        }
    }
}
