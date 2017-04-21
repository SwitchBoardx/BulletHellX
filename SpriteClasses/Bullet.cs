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

namespace SpriteClasses
{
    public class Bullet : Sprite
    {
        //This bool will be used later. To prevent friendly bullets from killing players
        //Useful for multiplayer and if we start reflecting bullets
        public bool isFriendly { get; set; } 
        public int powerLevel { get; set; }
        public Color bulletColor { get; set; }
        public Bullet(bool isAlive, bool friendly, int bulletPower, Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect): base(textureImage, position, velocity, useOrigin, rotationSpeed, scale, spriteEffect)
        {
            bulletColor = Color.White;
            Alive = isAlive;
            isFriendly = friendly;
            powerLevel = bulletPower;
            Mass = 20.0f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                //Changes which bullet in the sheet will be drawn based on bulletPower
                if (powerLevel == 1)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(0, 0, 5, Image.Height), bulletColor, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
                if (powerLevel == 2)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(5, 0, 6, Image.Height), bulletColor, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
                if (powerLevel == 3)
                {
                    spriteBatch.Draw(Image, Position, new Rectangle(14, 0, (Image.Width - 14), Image.Height), bulletColor, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
                }
                if (powerLevel == 4)
                {
                    spriteBatch.Draw(Image, Position, null, Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Alive)
            {
                float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000f;

                //Position -= Velocity * timelapse;
                //Added 03/15/2014
                Physics2D.setDisplacement(this, timelapse, true);
                
            }
        }

        public override void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            if (Alive)
            {
                Update(gameTime);

                //This causes bullets to be set to !Alive if they go off screen.
                //Freeing up their slot in the gun arrays
                if (Position.Y < 0)
                    Alive = false;
            }
        }
    }
}
