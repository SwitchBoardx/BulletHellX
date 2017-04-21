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
    public class Shield : Sprite
    {
        public int Health { get; set; }
        public Color shieldColor;
        public float radius { get; set; }
        SoundEffect shieldPowerDown;
        public Shield(ContentManager content, Texture2D textureImage, Vector2 playerPosition)
            : base(textureImage, Vector2.Zero, Vector2.Zero, true, 0.0f, 1.0f, SpriteEffects.None)
        {
            Position = playerPosition;
            Health = 7;
            Image = textureImage;
            shieldPowerDown = content.Load<SoundEffect>("Sounds/shieldDown");
            shieldColor = Color.White * 0.3f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Alive)
                spriteBatch.Draw(Image, Position, null, shieldColor, 0.0f, Origin, Scale, SpriteEffect, 0.0f);
        }

        public void UpdateShield()
        {
            if (Health <= 0)
            {
                shieldPowerDown.Play();
                Alive = false;
            }
            if (Health < 3)
            {
                shieldColor = Color.Red * 0.3f;
            }
            else if (Health < 5)
            {
                shieldColor = Color.Orange * 0.4f;
            }
            else if (Health < 7)
            {
                shieldColor = Color.Yellow * 0.3f;
            }
        }
    }
}
