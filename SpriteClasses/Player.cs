using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace SpriteClasses
{
    public class Player : Sprite
    {
        ContentManager content;
        public Texture2D bullet, shieldImage, gravBombImage;
        //These bools are used to determine which sprite to load. IE banking left, right or idle
        public bool PlaneIdle { get; set; }
        public bool PlaneLeft { get; set; }
        public bool PlaneRight { get; set; }

        //used to keep track of shooting cooldown
        public int ShootCD { get; set; }

        //Will be upped when grabbing power ups and will cause badasser guns
        public int BulletPower { get; set; }

        //The Number of Bombs you have in your Inventory.
        public int Bombs { get; set; }

        public int Lives { get; set; }
        public bool isRecovering { get; set; }
        public bool flicker { get; set; }
        public float RecoveryTimer { get; set; }
        public float FlickerTimer { get; set; }
        public const float RECOVERY_TIME = 2000.0f;
        public const float FLICKER_TIME = 250.0f;

        //Max speed of the ship and gun and ammo limits and shoot timer
        public const int MAX_SPEED = 250;
        public const int PLAYER_GUNS = 5;
        public const int PLAYER_AMMO = 15;
        public const int SHOOT_CD = 100;
        public const int MAX_BOMB_AMMO = 3;

        //UPDATED: now uses one multidimensional array instead of five arrays!
        public Bullet[,] guns = new Bullet[PLAYER_GUNS, PLAYER_AMMO];
        public Shield shield;
        public GravityBomb gravBomb;

        //Laser Sound
        public SoundEffect playerLaser, shipHit;
        public SoundEffectInstance shot;
        public SoundEffect shieldHit;

        //Constructor which calls the base class constructor and modifies it
        //UPDATE: added in a content manager from Game to clean up Game1
        public Player(ContentManager Content, Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect, int playerNumber)
            : base(textureImage, position, velocity, useOrigin, rotationSpeed, scale, spriteEffect)
        {
            PlaneIdle = true;
            ShootCD = 0;
            content = Content;
            Bombs = 2;

            Lives = 3;
            isRecovering = false;
            flicker = false;
            RecoveryTimer = RECOVERY_TIME;
            FlickerTimer = FLICKER_TIME;

            shieldHit = Content.Load<SoundEffect>("Sounds/shieldHit");
            playerLaser = content.Load<SoundEffect>("Sounds/playerLaser");
            shipHit = content.Load<SoundEffect>("Sounds/loselife");
            shot = playerLaser.CreateInstance();
            shot.Volume = 0.5f;

            //loaded in the bullet image here so the array could be initialized here rather than in Game1
            bullet = content.Load<Texture2D>("bullets");
            shieldImage = content.Load<Texture2D>("playerShield");
            gravBombImage = content.Load<Texture2D>("black-hole");
            shield = new Shield(Content, shieldImage, Position);
            BulletPower = 1;

            if (useOrigin)
            {
                //Had to hardcode this because the actual image is larger than 76px
                origin.X = 76 / 2;
                origin.Y = textureImage.Height / 2;
            }
            //Creates gravitybomb object
            gravBomb = new GravityBomb(false, gravBombImage, Vector2.Zero, new Vector2(0, -MAX_SPEED - 200), true, 2.0f, 1.0f, SpriteEffects.None);

            //initializes the array to avoid null pointer exceptions
            for (int i = 0; i < PLAYER_GUNS; i++)
                for (int ii = 0; ii < PLAYER_AMMO; ii++)
                    guns[i, ii] = new Bullet(false, true, BulletPower, bullet, Vector2.Zero, Vector2.Zero, false, 0.0f, 1.0f, SpriteEffects.None);

        }

        //Will keep the player on the screen but not inverse his velocity
        //Also calls the update method for any bullet that has been fired. Aka Alive
        public override void Update(GameTime gameTime, GraphicsDevice device)
        {

            if (Alive)
            {
                shield.Position = Position;

                Update(gameTime);

                if (position.Y > (device.Viewport.Height - (origin.Y * Scale)))
                {
                    position.Y = device.Viewport.Height - (origin.Y * Scale);
                    velocity.Y = 0;
                }
                if (position.X > (device.Viewport.Width - (origin.X * Scale)))
                {
                    position.X = device.Viewport.Width - (origin.X * Scale);
                    velocity.X = 0;
                }
                if (position.Y < (origin.Y * Scale))
                {
                    position.Y = origin.Y * Scale;
                    velocity.Y = 0;
                }
                if (position.X < origin.X)
                {
                    position.X = origin.X;
                    velocity.X = 0;
                }
                if(shield.Alive)
                    shield.UpdateShield();

            }

            //updates all the bullets
            for (int i = 0; i < PLAYER_GUNS; i++)
                for (int ii = 0; ii < PLAYER_AMMO; ii++)
                {
                    if (guns[i, ii].Alive)
                        guns[i, ii].Update(gameTime, device);
                }

            if (isRecovering)
            {
                if (FlickerTimer <= 0)
                {
                    flicker = !flicker;
                    FlickerTimer = FLICKER_TIME;
                }
                else
                    FlickerTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (RecoveryTimer <= 0)
                {
                    shield = new Shield(content, shieldImage, Position);
                    BulletPower = 1;

                    isRecovering = false;
                    flicker = false;
                    RecoveryTimer = RECOVERY_TIME;
                }
                else
                    RecoveryTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (Lives < 0)
            {
                shield.Alive = false;
                Alive = false;
            }
        }

        //Draws plane and any Alive bullets
        public override void Draw(SpriteBatch spriteBatch)
        {
            //draws all living bullets
            for (int i = 0; i < PLAYER_GUNS; i++)
                for (int ii = 0; ii < PLAYER_AMMO; ii++)
                {
                    if (guns[i, ii].Alive)
                    {

                        guns[i, ii].Draw(spriteBatch);
                    }
                }

            if (Alive && !flicker)
            {
                //These statements change the rectangle used to draw the image according to its state
                if (PlaneIdle)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(0, 0, 76, 75), Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
                if (PlaneLeft)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(76, 0, 76, 75), Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
                if (PlaneRight)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(150, 0, 76, 75), Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
               
                if (shield.Health > 0)
                    shield.Draw(spriteBatch);


            }

        }

        public override void Left()
        {
            PlaneIdle = false;
            PlaneRight = false;
            velocity.X -= InitialVelocity.X;

            if ((velocity.X * -1) > MAX_SPEED)
                velocity.X = (MAX_SPEED * -1);

            PlaneLeft = true;
        }

        public override void Right()
        {
            PlaneIdle = false;
            PlaneLeft = false;
            velocity.X += InitialVelocity.X;

            if (velocity.X > MAX_SPEED)
                velocity.X = MAX_SPEED;

            PlaneRight = true;
        }

        public override void Idle()
        {
            PlaneIdle = true;
            PlaneRight = false;
            PlaneLeft = false;
            Velocity = Velocity * .90f;
        }

        public override void Up()
        {
            velocity.Y -= InitialVelocity.Y;

            if ((velocity.Y * -1) > MAX_SPEED)
                velocity.Y = MAX_SPEED * -1;
        }

        public override void Down()
        {
            velocity.Y += InitialVelocity.Y;

            if (velocity.Y > MAX_SPEED)
                velocity.Y = MAX_SPEED;
        }

        //Shoots bullets based on which rows in the runs haven't been fired.
        //Further Details below
        public void ShootBomb()
        {
            Bombs--;
            gravBomb.Position = new Vector2(Position.X, Position.Y);
            gravBomb.Alive = true;
        }
        public void Shoot(GameTime gameTime)
        {
            //if its off cooldown
            if(Alive)
            if (ShootCD <= 0)
            {
                
                //adding this here helps everything run smoother if the player is mashing space
                //as opposed to holding it.
                //if (Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                //{
                /*Based on the ships bulletpower (which will increase with collecting powerups) the guns array will
                 * be checked for a row of bullets that have not been fired or are off screen and fire them.  It also
                 * sets the properties of the bullets */
                if (BulletPower == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        
                        for (int ii = 0; ii < PLAYER_AMMO; )
                        {

                            if (!guns[i, ii].Alive && !guns[(i + 1), ii].Alive)
                            {
                                //creates bullets with all the required player attributes. IE. matches bullet power and makes
                                //the bullet friendly.  Check bullet constructor for details on what each parameter is.
                                guns[i, ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X - (Origin.X / 2), Position.Y), new Vector2(0, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 1), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X + (Origin.X / 2), Position.Y), new Vector2(0, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                shot.Play();
                                //sets the cool down time
                                ShootCD = SHOOT_CD;

                                //aborts the loop in order for the whole array to be checked again when the button is pressed
                                ii = PLAYER_AMMO;
                                i = 2;
                            }
                            else
                            {
                                ii++;

                                //aborts the loop to protect against out of array bounds crash because of the (i + 1)
                                if (ii >= PLAYER_AMMO)
                                    i = 2;
                            }

                        }
                    }
                }
                else if (BulletPower == 2)
                {
                    for (int i = 0; i < PLAYER_GUNS - 2; i++)
                        for (int ii = 0; ii < PLAYER_AMMO; )
                        {
                            if (!guns[i, ii].Alive && !guns[(i + 1), ii].Alive && !guns[(i + 2), ii].Alive)
                            {
                                shot.Play();
                                guns[i, ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X - (Origin.X), Position.Y), new Vector2(100, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 1), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X + (Origin.X), Position.Y), new Vector2(-100, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 2), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X, (Position.Y - (Origin.Y / 2))), new Vector2(0, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);

                                ShootCD = SHOOT_CD;

                                ii = PLAYER_AMMO;
                                i = 2;
                            }
                            else
                            {
                                ii++;

                                if (ii >= PLAYER_AMMO)
                                    i = 2;
                            }
                        }
                }
                else if (BulletPower == 3)
                {
                    for (int i = 0; i < PLAYER_GUNS; i++)
                        for (int ii = 0; ii < PLAYER_AMMO; )
                        {
                            if (!guns[i, ii].Alive && !guns[(i + 1), ii].Alive && !guns[(i + 2), ii].Alive && !guns[(i + 3), ii].Alive && !guns[(i + 4), ii].Alive)
                            {
                                shot.Play();
                                guns[i, ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X - (Origin.X / 2), Position.Y), new Vector2(100, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 1), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X + (Origin.X / 2), Position.Y), new Vector2(-100, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 2), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X, (Position.Y - (Origin.Y / 2))), new Vector2(0, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 3), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X - Origin.X, Position.Y), new Vector2(200, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                guns[(i + 4), ii] = new Bullet(true, true, BulletPower, bullet, new Vector2(Position.X + Origin.X, Position.Y), new Vector2(-200, MAX_SPEED + 100), false, 0.0f, 1.0f, SpriteEffects.None);
                                
                                ShootCD = SHOOT_CD;

                                ii = PLAYER_AMMO;
                                i = PLAYER_GUNS;
                            }
                            else
                            {
                                ii++;

                                if (ii >= PLAYER_AMMO)
                                    i = PLAYER_GUNS;
                            }
                        }
                }

                //}
            }
            //if on cool down, this statement subtracts passing milliseconds from the cooldown timer
            else
            {
                ShootCD -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

    }
}
