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
using SpriteClasses;

namespace FinalProject
{
    public class Boss: Sprite
    {
        public enum AttackPatterns
        {
            Spray,
            Wave,
            Rain,
            None
        }

        public AttackPatterns atp;

        //Limits for his attacks
        public const int SPRAY_SHOTS = 17;
        public const int WAVE_AMOUNT = 3;
        public const int RAIN_SHOTS = 24;

        //To reference the number of shots made during an attack
        public int attackIndex;
        public int sprayIndex;

        //Boolean flags
        public bool isActive { get; set; }
        public bool isEngaged { get; set; }
        public bool isShooting { get; set; }

        public override Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle(0, 0, Image.Width, Image.Height / 5);
            }
        }

        public const float TOTAL_HEALTH = 2000.0f;
        public float Health { get; set; }

        private Texture2D bulletImg;
        private Texture2D healthBar;
        
        Color color;
        float FadeTimer;

        //Handles cooldowns for over all attacking, and time between shots of his attacks
        public const float BASE_CD = 2500.0f;
        public const float SPRAY_CD = 250.0f;
        public const float WAVE_CD = 2000.0f;
        public const float RAIN_CD = 250.0f;
        public float attackCD { get; set; }
        public float sprayCD { get; set; }
        public float waveCD { get; set; }
        public float rainCD { get; set; }

        //His own list of bullets. Special
        public List<Sprite> BulletList;

        //Utility
        ContentManager Content;
        Random rnd;
        SoundEffect bossMusic;
        public SoundEffectInstance bossMusicInstance;

        public Boss(Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect, ContentManager content)
            : base(textureImage, position, velocity, true, rotationSpeed, scale, spriteEffect)
        {
            atp = AttackPatterns.None;
            attackIndex = 0;

            isActive = false;
            isEngaged = false;
            isShooting = false;
            Alive = false;
            FadeTimer = 1;
            color = Color.White;
            Health = TOTAL_HEALTH;
            BulletList = new List<Sprite>();
            rnd = new Random();
            Content = content;

            bulletImg = Content.Load<Texture2D>("bossBullet");
            healthBar = Content.Load<Texture2D>("b_healthBar");

            bossMusic = Content.Load<SoundEffect>("Sounds/bossMusic");

            bossMusicInstance = bossMusic.CreateInstance();
            bossMusicInstance.Volume = 0.5f;
            bossMusicInstance.IsLooped = true;
        }

        //Will handle the animation of the boss coming slowly into view from the top of the screen
        //He will be invulnerable until the animation is complete. Upon which he will behin shooting
        public void beginAnimation()
        {
            //The first instant he is brought into the game
            //The condition ensures it is only done once.
            if (!isActive)
            {
                //Should start him off screen
                Position = new Vector2(Origin.X - 50, -Origin.Y);
                //So he'll slowly come in to view
                Velocity = new Vector2(0, 0.5f);

                isActive = true;
            }
            else if (isActive && !isEngaged)
            {
                if (Position.Y + Origin.Y < Origin.Y)
                {
                    Position += Velocity;
                }
                else
                {
                    Velocity = Vector2.Zero;
                    isEngaged = true;
                    bossMusicInstance.Play();
                }
            }
        }

        public override void Update(GameTime gameTime, GraphicsDevice graphics)
        {

            //When his attack is no longer cooling down he will select a new attack
            //The isShooting variable will ensure he doesnt do more than one attack at a time
            // ...Hopefully
            if (!isShooting)
            {
                if (attackCD < 0)
                {
                    atp = (AttackPatterns)rnd.Next(3);
                    isShooting = true;

                    sprayIndex = 0;
                    attackIndex = 0;

                    attackCD = BASE_CD * (Health / TOTAL_HEALTH);
                }
                else
                    attackCD -= gameTime.ElapsedGameTime.Milliseconds;
            }

            //Updates its own bullets
            for (int i = 0; i < BulletList.Count; i++)
            {
                Physics2D.moveSprite(BulletList[i], Game1.gravForce, Vector2.Zero, gameTime.ElapsedGameTime.Milliseconds / 1000.0f, true);

                if (BulletList[i].isOffScreen(graphics))
                    BulletList.Remove(BulletList[i]);
            }

            if (Health <= 0)
            {
                isShooting = false;
                for (int i = 0; i < BulletList.Count; i++)
                {
                    ((Bullet)BulletList[i]).isFriendly = true;
                }
                if (FadeTimer > 0)
                {
                    FadeTimer -= 0.005f;
                    color = Color.White * FadeTimer;
                }
                else   
                    Alive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, color, 0.0f, Origin, Scale, SpriteEffects.None, 1.0f);
            //Draws its own bullets
            foreach (Bullet shot in BulletList)
            {
                shot.Draw(spriteBatch);
            }

            if (isEngaged)
            {
                //Draws health bar box
                spriteBatch.Draw(healthBar, new Vector2(520 - 50, 200), new Rectangle(0, 0, 45, healthBar.Height), Color.White);

                //Adjusts colour of the health bar based on remaining health %
                Color healthBarColor = Color.Green;

                if (Health / TOTAL_HEALTH < 0.2f)
                    healthBarColor = Color.Red;
                else if (Health / TOTAL_HEALTH < 0.35f)
                    healthBarColor = Color.Orange;
                else if (Health / TOTAL_HEALTH < 0.5f)
                    healthBarColor = Color.Yellow;

                //Fills health bar
                spriteBatch.Draw(healthBar, new Vector2(520 - 22, 215), new Rectangle(60, 15, 8, (int)(328 * (Health / TOTAL_HEALTH))), healthBarColor);
            }
        }

        public void Attack(Vector2 playerPosition, GameTime gameTime, GraphicsDevice graphics)
        {
            //Update his current attack
            if (atp == AttackPatterns.Spray)
            {
                if (attackIndex <= SPRAY_SHOTS)
                {
                    if (sprayCD < 0)
                    {
                        attackIndex++;

                        if (attackIndex < SPRAY_SHOTS / 2)
                        {
                            sprayIndex++;

                            BulletList.Add(new Bullet(true, false, 4, bulletImg, new Vector2(((sprayIndex - 1) * bulletImg.Width) * 15 + bulletImg.Width, Position.Y + Origin.Y),
                                new Vector2(0, -200), true, 0.0f, 1.0f, SpriteEffects.None));
                        }
                        else
                        {
                            sprayIndex--;

                            BulletList.Add(new Bullet(true, false, 4, bulletImg, new Vector2((sprayIndex * bulletImg.Width) * 15 + bulletImg.Width, Position.Y + Origin.Y),
                                new Vector2(0, -200), true, 0.0f, 1.0f, SpriteEffects.None));
                        }

                        sprayCD = SPRAY_CD * (Health / TOTAL_HEALTH);
                    }
                    else
                        sprayCD -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                    isShooting = false;
            }
            else if (atp == AttackPatterns.Wave)
            {
                if (attackIndex <= WAVE_AMOUNT)
                {
                    if (waveCD < 0)
                    {
                        //Sets the size of the gap
                        int min = rnd.Next(graphics.Viewport.Width - 150);
                        int max = min + 150;

                        //Loops through the width of the screen incrementing by the width of the bullet
                        for (int i = 0; i < graphics.Viewport.Width; i += bulletImg.Width)
                        {
                            //If not within the gap
                            if (i < min || i > max)
                            {
                                BulletList.Add(new Bullet(true, false, 4, bulletImg, new Vector2(i, Position.Y + Origin.Y),
                                    new Vector2(0, -175 * (1 + (1 - Health / TOTAL_HEALTH))), true, 0.0f, 1.0f, SpriteEffects.None));
                            }
                            else
                                continue;
                        }

                        attackIndex++;
                        waveCD = WAVE_CD; //this has to be scaled somehow with the speed of the shots
                    }
                    else
                        waveCD -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                    isShooting = false;
            }
            else if (atp == AttackPatterns.Rain)
            {
                if (attackIndex <= RAIN_SHOTS)
                {
                    if (rainCD < 0)
                    {
                        //Calculates the vector between the bullet and the boss and adjusts speed
                        Vector2 startPos = new Vector2((float)rnd.Next(graphics.Viewport.Width), Position.Y + Origin.Y);
                        Vector2 startVelocity = Vector2.Normalize(playerPosition - startPos) * -250;

                        BulletList.Add(new Bullet(true, false, 4, bulletImg, startPos, startVelocity, true, 0.0f, 1.0f, SpriteEffects.None));

                        attackIndex++;
                        rainCD = RAIN_CD * (Health / TOTAL_HEALTH);
                    }
                    else
                        rainCD -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                    isShooting = false;
            }
        }

        public void checkCollision(Player player, BasicParticleSystem bps)
        {
            for (int i = 0; i < BulletList.Count; i++)
            {
                if (BulletList[i].CollisionRectangle.Intersects(player.shield.CollisionRectangle))
                {
                    if (BulletList[i].CollisionSprite(player.shield) && player.Alive && !((Bullet)BulletList[i]).isFriendly)
                    {
                        if (player.shield.Alive)
                        {
                            player.shield.Health--;
                            player.shieldHit.Play();
                            BulletList.Remove(BulletList[i]);
                        }
                        else
                        {
                            if (!player.isRecovering)
                            {
                                player.Lives--;
                                player.shipHit.Play();
                                player.isRecovering = true;
                            }

                            BulletList.Remove(BulletList[i]);
                        }
                    }
                }
            }


            for(int i = 0; i < Player.PLAYER_GUNS; i++)
                for (int ii = 0; ii < Player.PLAYER_AMMO; ii++)
                {
                    if (player.guns[i, ii].Alive && Alive)
                    {
                        if (player.guns[i, ii].CollisionRectangle.Intersects(CollisionRectangle))
                        {
                            //if (this.CollisionSprite(player.guns[i,ii]))
                            //{
                                Health--;
                                player.guns[i, ii].Alive = false;
                            //}
                        }
                    }
                }

        }

    }
}
