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
using SpriteClasses;

namespace SpriteClasses
{
    public class Physics2D
    {
        public const float GRAVITY = 9.8f;
        public const float GRAVITATIONAL_CONST = 5f;
        Random rand = new Random();
        public bool sinNeg = false;

        //Calculation to change the gravity vector based on a gravity well.
        //We may have to tamper with the gravitational constant to get it right
        public static Vector2 GravitationalForce(Sprite gravWell, Sprite subject)
        {
            float distance;
            float magnitude;

            Vector2 difference = gravWell.Position - subject.Position;
            Vector2 gForceVector;

            distance = (float)(Math.Sqrt((double)((difference.X * difference .X) + (difference.Y * difference.Y))));

            magnitude = GRAVITATIONAL_CONST * ((gravWell.Mass * subject.Mass) / (distance * distance));

            Vector2.Normalize(difference);

            gForceVector = difference * magnitude;

            return gForceVector;
        }

        //Calculates and sets the netforce acting on a sprite
        public static void setNetForce(Sprite sprite, Vector2 gravity, Vector2 wind)
        {
            setForce_MA(sprite);
            sprite.Force += gravity + wind;
        }

        //returns the net force that will be acting on a sprite
        public static Vector2 getNetForce(Sprite sprite, Vector2 gravity, Vector2 wind)
        {
            return getForce_MA(sprite) + gravity + wind;
        }
        //calculates and sets the Force of a sprite based on mass and acceleration
        public static void setForce_MA(Sprite sprite)
        {
            sprite.Force = sprite.Mass * sprite.Acceleration;
        }

        //returns the force of a sprite based on Mass and Acceleration
        public static Vector2 getForce_MA(Sprite sprite)
        {
            return sprite.Mass * sprite.Acceleration;
        }
        //calculates and sets the Accel of a sprite based on Force and mass
        public static void setAccel_FM(Sprite sprite)
        {
            sprite.Acceleration = sprite.Force / sprite.Mass;
        }

        //returns the Accel of a sprite
        public static Vector2 getAccel_FM(Sprite sprite)
        {
            return sprite.Force / sprite.Mass;
        }

        //Calculates final Velocity
        public static void setVF(Sprite sprite, float timeStep)
        {
            Vector2 VF;

            VF = sprite.InitialVelocity + (sprite.Acceleration * timeStep);
            sprite.Velocity = VF;
        }

        //returns the final velocity of a sprite
        public static Vector2 getVF(Sprite sprite, float timeStep)
        {
            Vector2 VF;

            VF = sprite.InitialVelocity + (sprite.Acceleration * timeStep);
            return VF;
        }

        //Calculates displacement AND adjusts the sprite position accordingly
        //Added 03/15/2014 isNegative boolean to determine if you want to go up or down or left or right.
        public static void setDisplacement(Sprite sprite, float timeStep, bool isNegative)
        {
            Vector2 Displacement;

            Displacement = (sprite.Velocity * timeStep) + (0.5f * sprite.Acceleration * (timeStep * timeStep));
            if (!isNegative)
                sprite.Position += Displacement;
            else if (isNegative)
                sprite.Position -= Displacement;
        }

        public static Vector2 getDisplacement(Sprite sprite, float timeStep, bool isNegative)
        {
            Vector2 Displacement;

            Displacement = (sprite.Velocity * timeStep) + (0.5f * sprite.Acceleration * (timeStep * timeStep));
            if (!isNegative)
                return sprite.Position + Displacement;
            else
                return sprite.Position - Displacement;
        }

        //Calculates displacement AND adjusts the sprite position accordingly
        //Added 03/15/2014
        public void followSineWaveLeftRight(Sprite sprite, GameTime gameTime, float timeStep)
        {
            
            Vector2 Displacement = new Vector2();
                Displacement.X = (float)Math.Cos(timeStep * (Math.PI)) * 5;
                if (!sinNeg)
                    sprite.Position += Displacement;
                else if (sinNeg)
                    sprite.Position -= Displacement;
                
        }
        //Calculates displacement AND adjusts the sprite position accordingly
        //Added 03/15/2014
        public void followSineWaveUpDown(Sprite sprite, GameTime gameTime, float timeStep)
        {
            Vector2 Displacement = new Vector2();
            Displacement.Y += (float)Math.Sin(timeStep * (Math.PI)) * 5;
            if (!sinNeg)
                sprite.Position += Displacement;
            else if (sinNeg)
                sprite.Position -= Displacement;
        }

        //calls all the required methods to move a sprite
        //Added 03/15/2014 isNegative boolean to determine if you want to go up or down or left or right.
        public static void moveSprite(Sprite sprite, Vector2 gravity, Vector2 wind, float timeStep, bool isNegative)
        {
            setNetForce(sprite, gravity, wind);
            setAccel_FM(sprite);
            setVF(sprite, timeStep);
            setDisplacement(sprite, timeStep, isNegative);

            sprite.InitialVelocity = sprite.Velocity;
        }
    }
}
