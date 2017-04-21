#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using SpriteClasses;
#endregion

namespace FinalProject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Random rnd;

        //All variables associated with spawning enemies
        public const int TOTAL_WAVES = 12;
        public const float TIME_TO_SPAWN = 5000.0f;
        public const float BOSS_EXPLOSION_TIMER = 200.0f;
        public const float RESET_TIMER = 10000;
        public const float HOW_TO_PLAY_TIMER = 4000;
        public float waveTimer;
        public int currentWave;


        public static Vector2 gravForce;
        public static Vector2 windForce;

        //Start Screen Objects/Images
        Texture2D arrowSelector, bgStartScreen, howtoplayImage;
        Sprite startCursor, startBG, howtoplay;
        float HowToPlayTimer;
        string strPlayers;
        int numPlayers;
        Vector2 playerTextPosition;
        Vector2[] startCursorPosition;
        Color textColor;
        SoundEffect menuMusic;
        SoundEffectInstance menuMusicInstance;

        //Game Screen Objects/Images
        Texture2D backgroundImage, bgPlanetImage, fighterPlaneOne, fighterPlaneTwo, enemyOnePicture, enemyTwoPicture, bossImage, particleImage;
        Player playerOne;
        Player playerTwo;
        Sprite bgPlanet;
        SpriteFont font, text;
        string strLives;
        public List<Sprite> EnemyList;
        ParallaxBackground background;
        Vector2 livesPositionOne, livesPositionTwo;
        Boss boss;
        BasicParticleSystem particles;
        TimeSpan totalParticleTime;
        float bossExplosionTimer;

        //Pause Screen Images and Objects
        Texture2D pauseScreenImage, yellowCursorImage;
        Sprite pauseBG, pauseCursor;
        Vector2[] pauseCursorPosition;
        int cursorPosition;
        float pauseMenuTimer;

        //Win/Lose Images
        Texture2D winScreen, loseScreen;
        int WinLoseScreenTimer;
        Sprite winBG, loseBG;
        float ResetTimer;

        GameState gameState;
        SoundEffect music;
        SoundEffectInstance bgMusic;
        SoundEffect gravBombSound;
        SoundEffectInstance gravBombActive;
        SoundEffect shieldHit;
        SoundEffect bulletUp;
        SoundEffect shieldUp;
        SoundEffect bombUp;
        SoundEffect shipHit;
        SoundEffect bossExplode;
        SoundEffect enemyExplode;

        //GameStates for Start, Pause, Game and End screen
        public enum GameState
        {
            StartScreen,
            HowToPlay,
            GameScreen,
            PauseScreen,
            WinOver,
            GameOver
        }


        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.Title = "Bullet Hell X";
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 520;
            
        }

        protected override void Initialize()
        {
            WinLoseScreenTimer = 0;
            ResetTimer = RESET_TIMER;
            HowToPlayTimer = HOW_TO_PLAY_TIMER;
            numPlayers = 1;
            strPlayers = numPlayers.ToString();

            textColor = new Color(166, 8, 16);
            playerTextPosition = new Vector2(325, 435);
            livesPositionOne = new Vector2(0, 678);
            livesPositionTwo = new Vector2(390, 678);
            rnd = new Random();
            Window.SetPosition(new Point(400, 0));

            EnemyList = new List<Sprite>();
            Enemy.enemyGuns = new List<Bullet>();

            //These forces are required for the physics engine
            gravForce = Vector2.Zero;
            windForce = Vector2.Zero;

            //Sets the timer
            currentWave = 0;
            waveTimer = TIME_TO_SPAWN;

            bossExplosionTimer = 0;

            //Creates an object for the game state
            gameState = GameState.StartScreen;


            //Position of the cursor for the start screen
            startCursorPosition = new Vector2[3];
            startCursorPosition[0] = new Vector2(140, 404);
            startCursorPosition[1] = new Vector2(140, 454);
            startCursorPosition[2] = new Vector2(140, 504);

            //Position of the cursor for pause screen.
            pauseCursorPosition = new Vector2[3];
            pauseCursorPosition[0] = new Vector2(145, 363);
            pauseCursorPosition[1] = new Vector2(145, 425);
            pauseCursorPosition[2] = new Vector2(145, 480);
            cursorPosition = 0;
            pauseMenuTimer = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loads all images, sounds, and fonts
            try
            {
                howtoplayImage = Content.Load<Texture2D>("howtoplay");
                particleImage = Content.Load<Texture2D>("circle");
                backgroundImage = Content.Load<Texture2D>("spaceBackground");
                fighterPlaneOne = Content.Load<Texture2D>("planespritesheet");
                fighterPlaneTwo = Content.Load<Texture2D>("planeTwoSpriteSheet");
                enemyOnePicture = Content.Load<Texture2D>("EnemyShip1");
                enemyTwoPicture = Content.Load<Texture2D>("Enemy2");
                bgPlanetImage = Content.Load<Texture2D>("bgPlanet");
                music = Content.Load<SoundEffect>("Sounds/bgmusic");
                gravBombSound = Content.Load<SoundEffect>("Sounds/gravBombActive");
                shieldHit = Content.Load<SoundEffect>("Sounds/shieldHit");
                bulletUp = Content.Load<SoundEffect>("Sounds/bulletPowerUp");
                menuMusic = Content.Load<SoundEffect>("Sounds/menumusic");
                shieldUp = Content.Load<SoundEffect>("Sounds/shieldUp");
                shipHit = Content.Load<SoundEffect>("Sounds/loseLife");
                bossExplode = Content.Load<SoundEffect>("Sounds/bossexplode");
                bombUp = Content.Load<SoundEffect>("Sounds/bombup");
                enemyExplode = Content.Load<SoundEffect>("Sounds/enemyexplode");
                arrowSelector = Content.Load<Texture2D>("arrowSelect");
                bgStartScreen = Content.Load<Texture2D>("startScreen");
                pauseScreenImage = Content.Load<Texture2D>("pauseScreen");
                loseScreen = Content.Load<Texture2D>("loseScreen");
                winScreen = Content.Load<Texture2D>("winScreen");
                yellowCursorImage = Content.Load<Texture2D>("arrowSelectY");
                font = Content.Load<SpriteFont>("Fonts/impact");
                text = Content.Load<SpriteFont>("Fonts/arial");
                bossImage = Content.Load<Texture2D>("boss");
            }
            catch (ContentLoadException ex)
            {
                System.Windows.Forms.MessageBox.Show("Recieved an error of type: " + ex.GetType().ToString());
            }

            //Creates the moving background and starts it moving
            background = new ParallaxBackground(GraphicsDevice);
            background.AddLayer(backgroundImage, 1.0f, 100.0f);
            background.SetMoveUpDown();
            background.StartMoving();

            //Begins playing the background music
            bgMusic = music.CreateInstance();
            bgMusic.Volume = 0.1f;
            bgMusic.IsLooped = true;

            menuMusicInstance = menuMusic.CreateInstance();
            menuMusicInstance.IsLooped = true;
            menuMusicInstance.Volume = 0.5f;

            gravBombActive = gravBombSound.CreateInstance();
            gravBombActive.Volume = 0.5f;
            gravBombActive.IsLooped = true;

            //Initializes all of our objects
            howtoplay = new Sprite(howtoplayImage, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            particles = new BasicParticleSystem(particleImage);
            pauseBG = new Sprite(pauseScreenImage, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            winBG = new Sprite(winScreen, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            loseBG = new Sprite(loseScreen, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            pauseCursor = new Sprite(yellowCursorImage, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            startBG = new Sprite(bgStartScreen, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            startCursor = new Sprite(arrowSelector, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);
            startCursor.Position = new Vector2(175, 404);
            bgPlanet = new Sprite(bgPlanetImage, new Vector2(GraphicsDevice.Viewport.Width / 2, -bgPlanetImage.Height / 3), new Vector2(0, 1.75f), true, 0.0f, 1, SpriteEffects.None);
            playerOne = new Player(Content, fighterPlaneOne, new Vector2(250, 500), new Vector2(50, 50), true, 0.0f, 1.0f, SpriteEffects.None, 1);
            playerTwo = new Player(Content, fighterPlaneTwo, new Vector2(350, 500), new Vector2(50, 50), true, 0.0f, 1.0f, SpriteEffects.None, 2);
            boss = new Boss(bossImage, new Vector2(bossImage.Width / 2, -bossImage.Height), Vector2.Zero, true, 0.0f, 1.0f, SpriteEffects.None, Content);
        }


        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //Update input is always called, but its purpose is changed based on gamestate.
            UpdateInput(gameTime);
            if (gameState == GameState.StartScreen)
            {
                menuMusicInstance.Play();
            }
            if (gameState == GameState.HowToPlay)
            {
                if (HowToPlayTimer < 0)
                    gameState = GameState.GameScreen;
                else
                    HowToPlayTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            if (gameState == GameState.WinOver || gameState == GameState.GameOver)
            {
                if (gameState == GameState.WinOver)
                {
                    totalParticleTime += gameTime.ElapsedGameTime;

                    particles.Update(totalParticleTime, gameTime.ElapsedGameTime);
                    Fireworks(gameTime);
                }
                if (ResetTimer < 0)
                {
                    bgMusic.Stop();
                    boss.bossMusicInstance.Stop();
                    Initialize();
                }
                else
                    ResetTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            //If the game is being plays    
            if (gameState == GameState.GameScreen)
            {
                totalParticleTime += gameTime.ElapsedGameTime;
                particles.Update(totalParticleTime, gameTime.ElapsedGameTime);
                //fetches the timelapse for use in the physics calculations
                float timeStep = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

                if (boss.Health <= 0)
                {
                    bossExplosion(gameTime);
                }
                else if (numPlayers == 1 && playerOne.Lives < 0)
                    gameState = GameState.GameOver;
                else if (numPlayers == 2 && playerOne.Lives < 0 && playerTwo.Lives < 0)
                    gameState = GameState.GameOver;
                //Updates the background and the music
                bgMusic.Play();
                background.Update(gameTime);

                //The planet slowly scrolls as the game is played.  If it reaches a certain
                //location the scrolling of it stops
                if (bgPlanet.Position.Y >= bgPlanetImage.Height / 5)
                {
                    bgPlanet.Velocity = Vector2.Zero;
                }

                //required to have the gravity bomb animate as intended
                if (playerOne.gravBomb.Alive)
                {
                    playerOne.gravBomb.Update(gameTime);

                }
                if (playerTwo.gravBomb.Alive && numPlayers == 2)
                    playerTwo.gravBomb.Update(gameTime);

                //Moves the planet sloooooowwwwlllyyyyyy
                Physics2D.setDisplacement(bgPlanet, timeStep, false);

                //Obvious
                playerOne.Update(gameTime, GraphicsDevice);

                //Obvious as well
                if (numPlayers == 2)
                    playerTwo.Update(gameTime, GraphicsDevice);

                //Runs through the enemies currently on screen and updates them in a way
                //depending on whether or not a gravity bomb has been fired or not
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    if (EnemyList[i] is Enemy)
                        if (!playerOne.gravBomb.Alive && !playerTwo.gravBomb.Alive)
                            //The sin (cos) wave function is called within here
                            EnemyList[i].Update(gameTime, GraphicsDevice);
                    //Turn Bomb off when screen is cleared

                    if (EnemyList[i].CollisionRectangle.Intersects(playerOne.shield.CollisionRectangle) && EnemyList[i] is Enemy)
                    {
                        if (!playerOne.isRecovering)
                        {
                            if (EnemyList[i].CollisionSprite(playerOne.shield))
                            {
                                if (playerOne.shield.Health > 0)
                                {
                                    if (playerOne.shield.Health > 1)
                                        shieldHit.Play();
                                    playerOne.shield.Health -= 2;
                                }
                                else
                                {
                                    if (playerOne.Alive)
                                    {
                                        if (!playerOne.isRecovering)
                                        {
                                            shipHit.Play();
                                            //todo: lose life/invulnerability for a second
                                            playerOne.Lives--;
                                            playerOne.isRecovering = true;
                                        }
                                    }
                                }

                                particles.AddExplosion(EnemyList[i].Position);
                                enemyExplode.Play();
                                EnemyList.Remove(EnemyList[i]);
                                break;
                            }
                        }
                    }
                    if (EnemyList[i].CollisionRectangle.Intersects(playerTwo.shield.CollisionRectangle) && EnemyList[i] is Enemy)
                    {
                        if (!playerTwo.isRecovering)
                        {
                            if (EnemyList[i].CollisionSprite(playerTwo.shield) && numPlayers == 2)
                            {
                                if (playerTwo.shield.Health > 0)
                                {
                                    if (playerTwo.shield.Health > 1)
                                        shieldHit.Play();
                                    playerTwo.shield.Health -= 2;
                                }
                                else
                                {
                                    if (playerTwo.Alive)
                                    {
                                        if (!playerTwo.isRecovering)
                                        {
                                            shipHit.Play();

                                            playerTwo.Lives--;
                                            playerTwo.isRecovering = true;
                                        }
                                    }
                                }

                                particles.AddExplosion(EnemyList[i].Position);
                                enemyExplode.Play();
                                EnemyList.Remove(EnemyList[i]);
                                break;
                            }
                        }
                    }

                    /* FUCKING WORKING */
                    if (EnemyList[i] is Enemy && ((Enemy)EnemyList[i]).EnemyType == 1)
                    {
                        Physics2D.moveSprite(EnemyList[i], gravForce, windForce, timeStep, false);
                        ((Enemy)EnemyList[i]).UpdateHealth(numPlayers);
                    }
                    else if (EnemyList[i] is Enemy && ((Enemy)EnemyList[i]).EnemyType == 2)
                        ((Enemy)EnemyList[i]).UpdateHealth(numPlayers);
                    else if (EnemyList[i] is PowerUp)
                    {
                        ((SpriteFromSheet)EnemyList[i]).Update(gameTime);

                        if (EnemyList[i].Position.Y > GraphicsDevice.Viewport.Height)
                        {
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                    }

                }

                //Updates enemies bullets with gravity bomb existing, and without
                //Added 03/15/2014
                for (int i = 0; i < Enemy.enemyGuns.Count; i++)
                {
                    if (!playerOne.gravBomb.Alive && !playerTwo.gravBomb.Alive)
                    {
                        Enemy.enemyGuns[i].Update(gameTime, GraphicsDevice);

                        if (Enemy.enemyGuns[i].CollisionRectangle.Intersects(playerOne.shield.CollisionRectangle))
                        {
                            if (Enemy.enemyGuns[i].CollisionSprite(playerOne.shield))
                                if (playerOne.shield.Health > 0)
                                {
                                    if (playerOne.shield.Health > 1)
                                        shieldHit.Play();
                                    playerOne.shield.Health--;
                                    Enemy.enemyGuns.Remove(Enemy.enemyGuns[i]);
                                    break;
                                }
                                else
                                {
                                    if (playerOne.Alive)
                                    {
                                        if (!playerOne.isRecovering)
                                        {
                                            shipHit.Play();
                                            //todo: lose life/invulnerability for a second
                                            playerOne.Lives--;
                                            playerOne.isRecovering = true;
                                        }
                                    }
                                }
                        }
                        if (Enemy.enemyGuns[i].CollisionRectangle.Intersects(playerTwo.shield.CollisionRectangle))
                        {
                            if (Enemy.enemyGuns[i].CollisionSprite(playerTwo.shield) && numPlayers == 2)
                                if (playerTwo.shield.Health > 0)
                                {
                                    if (playerTwo.shield.Health > 1)
                                        shieldHit.Play();
                                    playerTwo.shield.Health--;
                                    Enemy.enemyGuns.Remove(Enemy.enemyGuns[i]);
                                    break;
                                }
                                else
                                {
                                    if (playerTwo.Alive)
                                    {
                                        if (!playerTwo.isRecovering)
                                        {
                                            shipHit.Play();

                                            playerTwo.Lives--;
                                            playerTwo.isRecovering = true;
                                        }
                                    }
                                }
                        }

                    }
                    if (playerOne.gravBomb.Alive || playerTwo.gravBomb.Alive)
                    {
                        //ensures that any bullet being sucked into the well will not damage the player
                        Enemy.enemyGuns[i].isFriendly = true;

                        if (playerOne.gravBomb.Alive)
                            Enemy.enemyGuns[i].Position += Physics2D.GravitationalForce(playerOne.gravBomb, Enemy.enemyGuns[i]);

                        if (playerTwo.gravBomb.Alive && numPlayers == 2)
                            Enemy.enemyGuns[i].Position += Physics2D.GravitationalForce(playerTwo.gravBomb, Enemy.enemyGuns[i]);
                    }
                    Physics2D.moveSprite(Enemy.enemyGuns[i], gravForce, windForce, timeStep, true);
                }

                //Removes enemy bullets if they get trapped in the well or make it off screen
                for (int i = 0; i < Enemy.enemyGuns.Count; i++)
                {
                    if (Enemy.enemyGuns[i].isOffScreen(GraphicsDevice))
                    {
                        Enemy.enemyGuns.Remove(Enemy.enemyGuns[i]);
                        break;
                    }
                    if (playerOne.gravBomb.Alive || playerTwo.gravBomb.Alive)
                    {
                        if (playerOne.gravBomb.CollisionSprite(Enemy.enemyGuns[i]) && playerOne.gravBomb.Alive)
                        {
                            Enemy.enemyGuns.Remove(Enemy.enemyGuns[i]);
                            break;
                        }
                        if (playerTwo.gravBomb.CollisionSprite(Enemy.enemyGuns[i]) && playerTwo.gravBomb.Alive)
                        {
                            Enemy.enemyGuns.Remove(Enemy.enemyGuns[i]);
                            break;
                        }
                    }
                }
                if (playerOne.gravBomb.Alive)
                    if (playerOne.gravBomb.isOffScreen(GraphicsDevice))
                    {
                        waveTimer = 500;
                        for (int i = EnemyList.Count - 1; i >= 0; i--)
                        {
                            if (EnemyList[i] is Enemy)
                            {
                                particles.AddExplosion(EnemyList[i].Position);
                                enemyExplode.Play();
                                EnemyList.RemoveAt(i);
                            }
                        }
                        gravBombActive.Stop();
                        playerOne.gravBomb.Alive = false;
                    }
                if (playerTwo.gravBomb.Alive)
                    if (playerTwo.gravBomb.isOffScreen(GraphicsDevice))
                    {
                        waveTimer = 500;
                        gravBombActive.Stop();
                        playerTwo.gravBomb.Alive = false;
                    }


                //Removes enemies if they are trapped in well or make it off screen
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    // Does PowerUp Things.
                    if (playerOne.shield.CollisionSprite(EnemyList[i]) && EnemyList[i] is PowerUp && playerOne.Alive)
                    {
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Weapon)
                        {
                            bulletUp.Play();
                            if (playerOne.BulletPower < 3)
                                playerOne.BulletPower++;
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Shield)
                        {
                            shieldUp.Play();
                            playerOne.shield = new Shield(Content, playerOne.shieldImage, playerOne.Position);
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Bomb)
                        {
                            bombUp.Play();
                            if (playerOne.Bombs < 2)
                                playerOne.Bombs++;
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                    }
                    if (numPlayers > 1 && playerTwo.shield.CollisionSprite(EnemyList[i])
                        && EnemyList[i] is PowerUp && playerTwo.Alive)
                    {
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Weapon)
                        {
                            bulletUp.Play();
                            if (playerTwo.BulletPower < 3)
                                playerTwo.BulletPower++;

                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Shield)
                        {
                            shieldUp.Play();
                            playerTwo.shield = new Shield(Content, playerOne.shieldImage, playerOne.Position);
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                        if (((PowerUp)EnemyList[i]).Type == PowerUp.PowerUpType.Bomb)
                        {
                            bombUp.Play();
                            if (playerTwo.Bombs < 2)
                                playerTwo.Bombs++;
                            EnemyList.Remove(EnemyList[i]);
                            break;
                        }
                    }
                    if (EnemyList[i].isOffScreen(GraphicsDevice))
                    {
                        EnemyList.Remove(EnemyList[i]);
                        break;
                    }
                    //GravityBomb Code
                    //Added 03/15/2014
                    if (playerOne.gravBomb.Alive || playerTwo.gravBomb.Alive)
                    {
                        if (EnemyList[i] is Enemy)
                        {
                            if (playerOne.gravBomb.Alive)
                                EnemyList[i].Position += Physics2D.GravitationalForce(playerOne.gravBomb, EnemyList[i]);
                            if (playerTwo.gravBomb.Alive)
                                EnemyList[i].Position += Physics2D.GravitationalForce(playerTwo.gravBomb, EnemyList[i]);
                            if (EnemyList[i].CollisionSprite(playerOne.gravBomb) || EnemyList[i].CollisionSprite(playerTwo.gravBomb))
                            {
                                spawnPowerUp(i);
                                particles.AddExplosion(EnemyList[i].Position);
                                enemyExplode.Play();
                                EnemyList.Remove(EnemyList[i]);
                                break;
                            }

                        }
                    }

                    //Checks collisions between player bullets and enemies.
                    //Checking is done here for both players
                    for (int ii = 0; ii < Player.PLAYER_GUNS; ii++)
                        for (int iii = 0; iii < Player.PLAYER_AMMO; iii++)
                        {

                            if (playerOne.guns[ii, iii].Alive && EnemyList.Count > i)
                                if (playerOne.guns[ii, iii].CollisionRectangle.Intersects(EnemyList[i].CollisionRectangle))
                                {
                                    if (playerOne.guns[ii, iii].CollisionSprite(EnemyList[i]))
                                        if (EnemyList[i] is Enemy)
                                            if (((Enemy)EnemyList[i]).Health > 0)
                                            {
                                                ((Enemy)EnemyList[i]).Health--;
                                                playerOne.guns[ii, iii].Alive = false;
                                                break;
                                            }
                                            else
                                            {
                                                spawnPowerUp(i);
                                                particles.AddExplosion(EnemyList[i].Position);
                                                enemyExplode.Play();
                                                EnemyList.Remove(EnemyList[i]);
                                                playerOne.guns[ii, iii].Alive = false;
                                                break;
                                            }
                                }
                            if (numPlayers == 2)
                                if (playerTwo.guns[ii, iii].Alive && EnemyList.Count > i)
                                    if (playerTwo.guns[ii, iii].CollisionRectangle.Intersects(EnemyList[i].CollisionRectangle))
                                    {
                                        if (playerTwo.guns[ii, iii].CollisionSprite(EnemyList[i]))
                                            if (EnemyList[i] is Enemy)
                                            {
                                                if (((Enemy)EnemyList[i]).Health > 0)
                                                {
                                                    ((Enemy)EnemyList[i]).Health--;
                                                    playerTwo.guns[ii, iii].Alive = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    spawnPowerUp(i);
                                                    particles.AddExplosion(EnemyList[i].Position);
                                                    enemyExplode.Play();
                                                    EnemyList.Remove(EnemyList[i]);


                                                    playerTwo.guns[ii, iii].Alive = false;
                                                    break;
                                                }
                                            }
                                    }
                        }

                }

                //BOSS LOGIC
                if (boss.Alive && EnemyList.Count == 0)
                {
                    if (!boss.isEngaged)
                    {
                        boss.beginAnimation();
                        bgMusic.Volume *= 0.99f;
                    }
                    else
                    {

                        boss.Update(gameTime, GraphicsDevice);
                        boss.checkCollision(playerOne, particles);

                        for (int i = 0; i < boss.BulletList.Count; i++)
                        {
                            if (playerOne.gravBomb.Alive)
                            {
                                ((Bullet)boss.BulletList[i]).isFriendly = true;
                                boss.BulletList[i].Position += Physics2D.GravitationalForce(playerOne.gravBomb, boss.BulletList[i]);
                            }
                            if (playerTwo.gravBomb.Alive)
                            {
                                ((Bullet)boss.BulletList[i]).isFriendly = true;
                                boss.BulletList[i].Position += Physics2D.GravitationalForce(playerTwo.gravBomb, boss.BulletList[i]);
                            }

                        }

                        if (numPlayers == 2)
                            boss.checkCollision(playerTwo, particles);

                        if (boss.isShooting)
                        {
                            if (numPlayers == 1)
                                boss.Attack(playerOne.Position, gameTime, GraphicsDevice);
                            else
                            {
                                int playerPick = rnd.Next(2);

                                if (playerPick == 0)
                                {
                                    if (playerOne.Alive)
                                        boss.Attack(playerOne.Position, gameTime, GraphicsDevice);
                                    else
                                        boss.Attack(playerTwo.Position, gameTime, GraphicsDevice);
                                }
                                else
                                {
                                    if(playerTwo.Alive)
                                        boss.Attack(playerTwo.Position, gameTime, GraphicsDevice);
                                    else
                                        boss.Attack(playerOne.Position, gameTime, GraphicsDevice);
                                }
                            }

                        }
                    }
                }

                if (currentWave <= TOTAL_WAVES)
                {
                    if (!playerOne.gravBomb.Alive && !playerTwo.gravBomb.Alive)
                        spawnEnemy(gameTime);
                }

                else if (currentWave >= TOTAL_WAVES && boss.Health > 0)
                {
                    boss.Alive = true;
                }
            }
            base.Update(gameTime);
        }

        //Particle Animation for the boss to die
        private void bossExplosion(GameTime gameTime)
        {
            if (bossExplosionTimer < 0)
            {
                particles.AddExplosion(new Vector2(rnd.Next((int)boss.Position.X - 200, (int)boss.Position.X + 200), rnd.Next((int)boss.Position.Y, (int)boss.Position.Y + 200)));
                bossExplode.Play();
                WinLoseScreenTimer++;
                bossExplosionTimer = BOSS_EXPLOSION_TIMER;
            }
            else
                bossExplosionTimer -= gameTime.ElapsedGameTime.Milliseconds;

            if (WinLoseScreenTimer >= 15)
                gameState = GameState.WinOver;
        }

        //Win Screen Particle Effect
        private void Fireworks(GameTime gameTime)
        {
            if (bossExplosionTimer < 0)
            {
                particles.AddExplosion(new Vector2(rnd.Next(0, GraphicsDevice.Viewport.Width), rnd.Next(0, GraphicsDevice.Viewport.Height)));
                particles.AddExplosion(new Vector2(rnd.Next(0, GraphicsDevice.Viewport.Width), rnd.Next(0, GraphicsDevice.Viewport.Height)));
                bossExplosionTimer = BOSS_EXPLOSION_TIMER;
            }
            else
                bossExplosionTimer -= gameTime.ElapsedGameTime.Milliseconds;
        }

        //Spawns a powerup on an enemy's death
        private void spawnPowerUp(int index)
        {
            int powerUpChance = rnd.Next(50);
            PowerUp powerUp = null;

            if (powerUpChance < 10)
            {
                powerUp = new PowerUp(Content.Load<Texture2D>("bulletPowerup"), EnemyList[index].Position,
                    new Vector2(0, 100), new Vector2(3, 2),
                    Vector2.Zero, 0.7f, true, Content);
            }
            if (powerUp != null)
                EnemyList.Add(powerUp);
        }

        //Spawns Enemies
        private void spawnEnemy(GameTime gameTime)
        {
            if (waveTimer < 0)
            {
                int incoming = rnd.Next(2); //will be changed to be random to ensure a randomized series of enemies

                if (incoming == 0)
                {
                    //Determines a random position for the enemies to be spawned
                    int xPosition = rnd.Next(enemyOnePicture.Width, GraphicsDevice.Viewport.Width - enemyOnePicture.Width);

                    //Creates a temporary list that will be passed into the main enemy list
                    //This had to be done to avoid a bug created with updating the waves enemies moved on
                    List<Sprite> newEnemies = new List<Sprite>();

                    //Adds enemies to the temporary list and staggers their y position
                    for (int i = 0; i < 4; i++)
                    {
                        newEnemies.Add(new Enemy(1, Content, enemyOnePicture, new Vector2(xPosition, (i * 100) * -1),
                            new Vector2(0, 100), true, 2.0f, 1.0f, SpriteEffects.None));
                        //in order to have the enemies move in opposite waves we had to make sure every other
                        //enemy added to the list had a boolean that was flagged opposite to the previous enemy
                        if (i > 0)
                            ((Enemy)newEnemies[i]).physics.sinNeg = !((Enemy)newEnemies[i - 1]).physics.sinNeg;
                    }

                    //adds the temporary list to the real list
                    for (int i = 0; i < newEnemies.Count; i++)
                        EnemyList.Add(newEnemies[i]);


                }
                else if (incoming == 1)
                {
                    int xPos = rnd.Next(GraphicsDevice.Viewport.Width);
                    int xVelo = 0;

                    if (xPos > GraphicsDevice.Viewport.Width / 2)
                    {
                        xVelo = -(rnd.Next(30, 50));
                    }
                    else
                        xVelo = rnd.Next(30, 50);


                    for (int i = 0; i < 3; i++)
                    {
                        EnemyList.Add(new Enemy(2, Content, enemyTwoPicture, new Vector2(xPos - (i * xVelo), -100 - (i * 100)), new Vector2(xVelo, 200),
                            true, 0.0f, 1.0f, SpriteEffects.None));
                    }
                }

                //increments the current wave and resets the timer.
                //The plan is to have the boss spawn when a certain number of waves is reached
                currentWave++;
                waveTimer = TIME_TO_SPAWN;
            }
            else
                waveTimer -= gameTime.ElapsedGameTime.Milliseconds;

        }

        //Input for each Game State
        private void UpdateInput(GameTime gameTime)
        {
            GamePadState gamePadOne = GamePad.GetState(PlayerIndex.One);
            GamePadState gamePadTwo = GamePad.GetState(PlayerIndex.Two);
            KeyboardState keyState = Keyboard.GetState();

            //Handles controls for every gamestate
            if (gameState == GameState.StartScreen)
            {
                startCursor.Position = startCursorPosition[cursorPosition];
                if (pauseMenuTimer > 200)
                {
                    if (keyState.IsKeyDown(Keys.Up) || gamePadOne.ThumbSticks.Left.Y > 0 || gamePadTwo.ThumbSticks.Left.Y > 0)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition > 0)
                            cursorPosition--;
                        else if (cursorPosition == 0)
                            cursorPosition = 2;

                    }
                    if (keyState.IsKeyDown(Keys.Down) || gamePadOne.ThumbSticks.Left.Y < 0 || gamePadTwo.ThumbSticks.Left.Y < 0)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition < 2)
                            cursorPosition++;
                        else if (cursorPosition == 2)
                            cursorPosition = 0;
                    }

                    if (keyState.IsKeyDown(Keys.Space) || gamePadOne.Buttons.A == ButtonState.Pressed || keyState.IsKeyDown(Keys.Enter) || gamePadTwo.Buttons.A == ButtonState.Pressed)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition == 0)
                        {
                            menuMusicInstance.Stop();
                            gameState = GameState.HowToPlay;
                        }
                        else if (cursorPosition == 1)
                        {
                            if (numPlayers == 1)
                            {
                                numPlayers++;
                                playerOne.Position = new Vector2(150, 500);
                            }
                            else if (numPlayers == 2)
                            {
                                numPlayers--;
                                playerOne.Position = new Vector2(250, 500);
                                
                            }
                            strPlayers = numPlayers.ToString();
                        }
                        else if (cursorPosition == 2)
                        {
                            Exit();
                        }

                    }
                }
                else
                    pauseMenuTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            //This little bit of code adjusts the control schemes for multiple players
            //And is based on whether or not 1 or 2 gamepads are connected
            if (gameState == GameState.GameScreen)
            {
                if (numPlayers == 1)
                    OnePlayerInput(gamePadOne, keyState, gameTime);
                else if (numPlayers == 2 && gamePadTwo.IsConnected)
                    TwoPlayerInput(gamePadOne, gamePadTwo, gameTime);
                else if (numPlayers == 2 && !gamePadTwo.IsConnected)
                    TwoPlayerInput(gamePadOne, keyState, gameTime);
            }
            if (gameState == GameState.WinOver || gameState == GameState.GameOver)
            {
                if (keyState.IsKeyDown(Keys.Escape) || gamePadOne.Buttons.Start == ButtonState.Pressed || gamePadTwo.Buttons.Start == ButtonState.Pressed)
                {
                    bgMusic.Stop();
                    boss.bossMusicInstance.Stop();
                    Initialize();
                }
            }
            if (gameState == GameState.PauseScreen)
            {
                pauseCursor.Position = pauseCursorPosition[cursorPosition];
                if (pauseMenuTimer > 200)
                {
                    if (keyState.IsKeyDown(Keys.Space) || gamePadOne.Buttons.A == ButtonState.Pressed || keyState.IsKeyDown(Keys.Enter) || gamePadTwo.Buttons.A == ButtonState.Pressed)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition == 0)
                        {

                            if (playerOne.gravBomb.Alive || playerTwo.gravBomb.Alive)
                                gravBombActive.Play();
                            if (boss.isActive)
                                boss.bossMusicInstance.Volume = 1.0f;
                            else if (!boss.isActive)
                                bgMusic.Volume = 0.2f;
                            gameState = GameState.GameScreen;
                        }
                        else if (cursorPosition == 1)
                        {
                            bgMusic.Stop();
                            boss.bossMusicInstance.Stop();
                            Initialize();
                        }
                        else if (cursorPosition == 2)
                        {
                            Exit();
                        }

                    }
                    if (keyState.IsKeyDown(Keys.Up) || gamePadOne.ThumbSticks.Left.Y > 0 || gamePadTwo.ThumbSticks.Left.Y > 0)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition > 0)
                            cursorPosition--;
                        else if (cursorPosition == 0)
                            cursorPosition = 2;

                    }
                    if (keyState.IsKeyDown(Keys.Down) || gamePadOne.ThumbSticks.Left.Y < 0 || gamePadTwo.ThumbSticks.Left.Y < 0)
                    {
                        pauseMenuTimer = 0;
                        if (cursorPosition < 2)
                            cursorPosition++;
                        else if (cursorPosition == 2)
                            cursorPosition = 0;
                    }

                }
                else
                    pauseMenuTimer += gameTime.ElapsedGameTime.Milliseconds;
            }
        }
        public void OnePlayerInput(GamePadState gamePad1, KeyboardState keyState, GameTime gameTime)
        {
            bool keyPressed = false;
            if (gamePad1.Buttons.Start == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
            {
                if (boss.isActive)
                    boss.bossMusicInstance.Volume = 0.25f;  
                else if (!boss.isActive)
                    bgMusic.Volume = 0.1f;
                gravBombActive.Stop();

                gameState = GameState.PauseScreen;
            }

            if (playerOne.Alive)
            {
                if (keyState.IsKeyDown(Keys.Up) || gamePad1.ThumbSticks.Left.Y > 0)
                {
                    playerOne.Up();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Down) || gamePad1.ThumbSticks.Left.Y < 0)
                {
                    playerOne.Down();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Right) || gamePad1.ThumbSticks.Left.X > 0)
                {
                    playerOne.Right();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Left) || gamePad1.ThumbSticks.Left.X < 0)
                {
                    playerOne.Left();
                    keyPressed = true;
                }
                if (!keyPressed)
                {
                    playerOne.Idle();
                }

                if (keyState.IsKeyDown(Keys.Space) || gamePad1.Buttons.A == ButtonState.Pressed)
                {
                    if(!playerOne.isRecovering)
                        playerOne.Shoot(gameTime);
                }

                if ((keyState.IsKeyDown(Keys.B) || gamePad1.Buttons.B == ButtonState.Pressed) && playerOne.Bombs > 0 && playerOne.Alive)
                {
                    if (!playerOne.gravBomb.Alive)
                    {
                        gravBombActive.Play();
                        playerOne.ShootBomb();
                    }

                }
            }
        }
        public void TwoPlayerInput(GamePadState gamePad2, KeyboardState keyState, GameTime gameTime)
        {
            bool keyPressed = false;
            //First Player
            if (keyState.IsKeyDown(Keys.Escape) || gamePad2.Buttons.Start == ButtonState.Pressed)
            {
                gravBombActive.Stop();
                if (boss.isActive)
                    boss.bossMusicInstance.Volume = 0.25f;
                else if (!boss.isActive)
                    bgMusic.Volume = 0.1f;
                gameState = GameState.PauseScreen;
            }

            if (playerOne.Alive)
            {
                if (keyState.IsKeyDown(Keys.Up))
                {
                    playerOne.Up();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Down))
                {
                    playerOne.Down();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Right))
                {
                    playerOne.Right();
                    keyPressed = true;
                }
                if (keyState.IsKeyDown(Keys.Left))
                {
                    playerOne.Left();
                    keyPressed = true;
                }
                if (!keyPressed)
                {
                    playerOne.Idle();
                }
                if (keyState.IsKeyDown(Keys.Space) && !playerOne.isRecovering)
                {
                    playerOne.Shoot(gameTime);
                }
                if (keyState.IsKeyDown(Keys.B) && !playerOne.gravBomb.Alive && playerOne.Bombs > 0 && playerOne.Alive)
                {
                    gravBombActive.Play();
                    playerOne.ShootBomb();
                }
            }

            //Second Player
            if (playerTwo.Alive)
            {
                if (gamePad2.ThumbSticks.Left.Y > 0)
                {
                    playerTwo.Up();
                }
                if (gamePad2.ThumbSticks.Left.Y < 0)
                {
                    playerTwo.Down();
                }
                if (gamePad2.ThumbSticks.Left.X > 0)
                {
                    playerTwo.Right();
                }
                if (gamePad2.ThumbSticks.Left.X < 0)
                {
                    playerTwo.Left();
                }
                if (gamePad2.ThumbSticks.Left.X == 0 && gamePad2.ThumbSticks.Left.Y == 0)
                {
                    playerTwo.Idle();
                }
                if (gamePad2.Buttons.A == ButtonState.Pressed && !playerTwo.isRecovering)
                {
                    playerTwo.Shoot(gameTime);
                }
                if (gamePad2.Buttons.B == ButtonState.Pressed && !playerTwo.gravBomb.Alive && playerTwo.Bombs > 0 && playerTwo.Alive)
                {
                    gravBombActive.Play();
                    playerTwo.ShootBomb();
                }
            }
        }
        public void TwoPlayerInput(GamePadState gamePad1, GamePadState gamePad2, GameTime gameTime)
        {
            bool keyPressed = false;
            //First Player
            if (gamePad1.Buttons.Start == ButtonState.Pressed || gamePad2.Buttons.Start == ButtonState.Pressed)
            {
                gravBombActive.Stop();
                if (boss.isActive)
                    boss.bossMusicInstance.Volume = 0.25f;
                else if (!boss.isActive)
                    bgMusic.Volume = 0.1f;
                gameState = GameState.PauseScreen;
            }

            if (playerOne.Alive)
            {
                if (gamePad1.ThumbSticks.Left.Y > 0)
                {
                    playerOne.Up();
                    keyPressed = true;
                }
                if (gamePad1.ThumbSticks.Left.Y < 0)
                {
                    playerOne.Down();
                    keyPressed = true;
                }
                if (gamePad1.ThumbSticks.Left.X > 0)
                {
                    playerOne.Right();
                    keyPressed = true;
                }
                if (gamePad1.ThumbSticks.Left.X < 0)
                {
                    playerOne.Left();
                    keyPressed = true;
                }
                if (!keyPressed)
                {
                    playerOne.Idle();
                }
                if (gamePad1.Buttons.A == ButtonState.Pressed && !playerOne.isRecovering)
                {
                    playerOne.Shoot(gameTime);
                }
                if (gamePad1.Buttons.B == ButtonState.Pressed && !playerOne.gravBomb.Alive && playerOne.Bombs > 0 && playerOne.Alive)
                {
                    gravBombActive.Play();
                    playerOne.ShootBomb();
                }
            }

            //Second Player
            if (playerTwo.Alive)
            {
                if (gamePad2.ThumbSticks.Left.Y > 0)
                {
                    playerTwo.Up();
                }
                if (gamePad2.ThumbSticks.Left.Y < 0)
                {
                    playerTwo.Down();
                }
                if (gamePad2.ThumbSticks.Left.X > 0)
                {
                    playerTwo.Right();
                }
                if (gamePad2.ThumbSticks.Left.X < 0)
                {
                    playerTwo.Left();
                }
                if (gamePad2.ThumbSticks.Left.X == 0 && gamePad2.ThumbSticks.Left.Y == 0)
                {
                    playerTwo.Idle();
                }

                if (gamePad2.Buttons.A == ButtonState.Pressed && !playerTwo.isRecovering)
                {
                    playerTwo.Shoot(gameTime);
                }
                if (gamePad2.Buttons.B == ButtonState.Pressed && !playerTwo.gravBomb.Alive && playerTwo.Bombs > 0 && playerTwo.Alive)
                {
                    gravBombActive.Play();
                    playerTwo.ShootBomb();
                }
            }
        }

        //Draw code based on Game State
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == GameState.StartScreen)
            {
                spriteBatch.Begin();
                startBG.Draw(spriteBatch);
                spriteBatch.DrawString(font, strPlayers, playerTextPosition, textColor);
                startCursor.Draw(spriteBatch);
                spriteBatch.End();
            }

            if (gameState == GameState.GameScreen || gameState == GameState.PauseScreen || gameState == GameState.GameOver || gameState == GameState.WinOver || gameState == GameState.HowToPlay)
            {
                background.Draw();
                spriteBatch.Begin();
                bgPlanet.Draw(spriteBatch);
                if (playerOne.gravBomb.Alive)
                    playerOne.gravBomb.Draw(spriteBatch);
                if (playerTwo.gravBomb.Alive)
                    playerTwo.gravBomb.Draw(spriteBatch);
                foreach (Sprite enemy in EnemyList)
                {
                    if (enemy is PowerUp)
                        ((PowerUp)enemy).Draw(spriteBatch);
                    else
                        enemy.Draw(spriteBatch);
                }
                particles.Draw(spriteBatch);
                foreach (Bullet shot in Enemy.enemyGuns)
                    shot.Draw(spriteBatch);

                playerOne.Draw(spriteBatch);

                if (numPlayers == 2)
                    playerTwo.Draw(spriteBatch);

                if (boss.Alive)
                    boss.Draw(spriteBatch);
                if (gameState == GameState.GameScreen || gameState == GameState.PauseScreen)
                {
                    if(playerOne.Lives >= 0)
                        strLives = "P1: " + playerOne.Lives.ToString() + "\nBombs " + playerOne.Bombs.ToString();
                    else if (playerOne.Lives < 0)
                        strLives = "P1: 0\nBombs " + playerOne.Bombs.ToString();

                    spriteBatch.DrawString(text, strLives, livesPositionOne, Color.Yellow);
                    if (numPlayers == 2)
                    {
                        if(playerTwo.Lives >= 0)
                            strLives = "P2: " + playerTwo.Lives.ToString() + "\nBombs " + playerTwo.Bombs.ToString();
                        else if(playerTwo.Lives < 0)
                            strLives = "P2: 0\nBombs " + playerOne.Bombs.ToString();
                        spriteBatch.DrawString(text, strLives, livesPositionTwo, Color.Yellow);
                    }
                }
                spriteBatch.End();
            }
            if (gameState == GameState.PauseScreen)
            {
                spriteBatch.Begin();
                pauseBG.Draw(spriteBatch);
                pauseCursor.Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.GameOver)
            {
                spriteBatch.Begin();
                loseBG.Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.WinOver)
            {
                spriteBatch.Begin();
                winBG.Draw(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.HowToPlay)
            {
                spriteBatch.Begin();
                howtoplay.Draw(spriteBatch);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}