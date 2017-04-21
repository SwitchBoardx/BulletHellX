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
    public class PowerUp: SpriteFromSheet
    {

        public enum PowerUpType
        {
            Weapon,
            Shield,
            Bomb
        }

        public PowerUpType Type { get; set; }
        public ContentManager Content { get; set;}

        public PowerUp(Texture2D image, Vector2 position, Vector2 velocity, Vector2 totalFrames, Vector2 startFrame,
            float totalTime, bool isLooped, ContentManager content)
            : base(image, position, velocity, totalFrames, startFrame, totalTime, isLooped)
        {
            Content = content;

            Random rnd = new Random();

            Type = (PowerUpType)rnd.Next(3);

            if (Type == PowerUpType.Weapon)
                Image = Content.Load<Texture2D>("bulletPowerup");
            else if (Type == PowerUpType.Shield)
                Image = Content.Load<Texture2D>("shieldup");
            else
                Image = Content.Load<Texture2D>("bombpowerup");
        }
    }
}
