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
    public class GravityBomb : Sprite
    {
        public override Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)(position.X - (Origin.X * Scale)) + 90, (int)(position.Y - (Origin.Y * Scale)) + 90, 70, 70);
            }
        }
        public GravityBomb(bool isAlive, Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect)
            : base(textureImage, position, velocity, useOrigin, rotationSpeed, scale, spriteEffect)
        {
            
            Alive = isAlive;
            Mass = 75.0f;
        }
    }
}
