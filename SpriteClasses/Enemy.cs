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
    public class Enemy : Sprite
    {
        public Texture2D bullet { get; set; }
        public static List<Bullet> enemyGuns;
        public float ShootCD { get; set; }
        public float timeStep { get; set; }
        public const float ENEMY_SHOOT_CD = 1000.0f;
        public const float MAX_HEALTH = 8;
        public float Health { get; set; }
        public int BulletPower { get; set; }
        public int EnemyType { get; set; }
        public Vector4 currentColor { get; set; }
        public Vector4 startColor { get; set; }
        public Vector4 endColor { get; set; }

        public SoundEffect laserShot;
        SoundEffectInstance shotSound;
        private ContentManager content;
        public Physics2D physics;
        

        public Enemy(int enemyType, ContentManager Content, Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect)
            : base(textureImage, position, velocity, useOrigin, rotationSpeed, scale, spriteEffect)
        {
            timeStep = 0;
            ShootCD = 0;
            Health = MAX_HEALTH;
            EnemyType = enemyType;
            content = Content;
            physics = new Physics2D();
            Mass = 5.0f;
            endColor = new Vector4(1.0f, 1.0f, 0f, 1);
            startColor = new Vector4(1.0f, 1.0f, 1.0f, 1);
            
            //loaded in the bullet image here so the array could be initialized here rather than in Game1
            bullet = content.Load<Texture2D>("bullets");

            laserShot = content.Load<SoundEffect>("Sounds/enemy1Laser");
            shotSound = laserShot.CreateInstance();
            shotSound.Volume = 0.15f;
            BulletPower = 1;

            
            
            
            if (useOrigin)
            {
                origin.X = textureImage.Width / 2;
                origin.Y = textureImage.Height / 2;
            }

        }

        public override void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            timeStep += timelapse;
            if (EnemyType == 1)
            {
                Rotation += RotationSpeed * timelapse;
                Rotation = Rotation % (MathHelper.Pi * 2);

                //FUCKING GENIUS
                //Added 03/15/2014
                physics.followSineWaveLeftRight(this, gameTime, timeStep);
                

            }
            if (EnemyType == 2)
            {
                Physics2D.moveSprite(this, new Vector2(0, -2f), Vector2.Zero, timelapse, false);
                
                //if (InitialVelocity.Y < -10)
                //    initialVelocity.Y = -10;
            }



            if (ShootCD <= 0)
            {
                shotSound.Play();
                Shoot();
            }
            else
                ShootCD -= gameTime.ElapsedGameTime.Milliseconds;

        }
        
        public void UpdateHealth(int numPlayers)
        {
            //Changes Color based on health value.
            //Added 03/15/2014
            float percentLife = Health / MAX_HEALTH;
            if (percentLife <= 0.5)
            {
                endColor = new Vector4(1.0f, 0.0f, 0f, 1);
                startColor = new Vector4(1.0f, 1.0f, 0, 1);
                currentColor = Vector4.Lerp(endColor, startColor, percentLife);
            }
            else
                currentColor = Vector4.Lerp(endColor, startColor, percentLife);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
                spriteBatch.Draw(Image, Position, null, new Color(currentColor), Rotation, Origin, Scale, SpriteEffect, 0.0f);
        }

        public void Shoot()
        {
            if (EnemyType == 1)
            {
                for (int i = -10; i <= 10; i += 10)
                    enemyGuns.Add(new Bullet(true, false, BulletPower, bullet, new Vector2(position.X + (i / 2), position.Y + origin.Y), new Vector2(-(i * 10), -(velocity.Y + 100)), true, 0.0f, 1.0f, SpriteEffects.None));

            }
            else
            {
                enemyGuns.Add(new Bullet(true, false, BulletPower, bullet, new Vector2(position.X - 5, position.Y + Origin.Y), new Vector2(0, -150), true, 0.0f, 1.0f, SpriteEffects.None));
                enemyGuns.Add(new Bullet(true, false, BulletPower, bullet, new Vector2(position.X + 5, position.Y + Origin.Y), new Vector2(0, -150), true, 0.0f, 1.0f, SpriteEffects.None));
            }

            foreach (Bullet shot in enemyGuns)
            {
                shot.bulletColor = Color.Red;
            }
            
            ShootCD = ENEMY_SHOOT_CD;
        }

        public override bool isOffScreen(GraphicsDevice graphics)
        {
            if (EnemyType == 1)
            {
                if (Position.Y - Origin.Y > graphics.Viewport.Height)
                    return true;
                else
                    return false;
            }
            else
            {
                if (Position.X > graphics.Viewport.Width ||
                    Position.X + Origin.X < 0)
                    return true;
                else
                    return false;
            }
        }


    }
}
