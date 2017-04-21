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
    public class Sprite
    {
        //Constants to be used in Game1
        public const int NUM_WARIOS = 10;
        public const int NUM_SPIKES = 5;

        //The following is all fields and their respective properties, where automatic properties
        //cannot be used.
        protected Vector2 initialVelocity;
        public Vector2 InitialVelocity
        {
            get { return initialVelocity; }
            set { initialVelocity = value; }
        }

        protected Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //Sets the bounding rectangle for a sprite. Is a read-only property
        public virtual Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)(position.X - Origin.X * Scale), (int)(position.Y - Origin.Y * Scale),
                    (int)(Image.Width * Scale), (int)(Image.Height * Scale));
            }
        }

        protected Vector2 acceleration;
        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        protected Vector2 force;
        public Vector2 Force
        {
            get { return force; }
            set { force = value; }
        }

        //Automatic properties
        public Texture2D Image { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public float Scale { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public bool UseOrigin { get; set; }
        public bool Alive { get; set; }
        public float Mass { get; set; }


        //Detects if an x and y (the mouse position) is contained within a bounding rectangle
        public bool CollisionMouse(int x, int y)
        {
            return CollisionRectangle.Contains(x, y);
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                            Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                        (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public static bool IntersectPixels(
                        Matrix transformA, int widthA, int heightA, Color[] dataA,
                        Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
        //Detects collision between two bounding rectangles
        public bool CollisionSprite(Sprite sprite)
        {

            bool hit = false;

            // Update the passed object's transform
            // SEQUENCE MATTERS HERE - DO NOT REARRANGE THE ORDER OF THE TRANSFORMATIONS BELOW
            Matrix spriteTransform =
                Matrix.CreateTranslation(new Vector3(-sprite.Origin, 0.0f)) *
                Matrix.CreateScale(sprite.Scale) *  //would go here
                Matrix.CreateRotationZ(sprite.Rotation) *
                Matrix.CreateTranslation(new Vector3(sprite.Position, 0.0f));

            // Build the calling object's transform
            // SEQUENCE MATTERS HERE - DO NOT REARRANGE THE ORDER OF THE TRANSFORMATIONS BELOW
            Matrix thisTransform =
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateScale(Scale) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            // Calculate the bounding rectangle of the passed object in world space
            //For the bounding rectangle, can't use CollisionRectangle property because
            //it adjusts for origin and scale, transform does both of those things for us, so 
            //we just need a simple bounding rectangle here

            //With transformations, don't use position here for X and Y, the transformation does that for you
            //also don't scale it or use origin, transformation does those things too          
            Rectangle spriteRectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, Convert.ToInt32(sprite.Image.Width), Convert.ToInt32(sprite.Image.Height)),
                     spriteTransform);

            // Calculate the bounding rectangle of the calling object in world space
            Rectangle thisRectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, Convert.ToInt32(Image.Width), Convert.ToInt32(Image.Height)),
                     thisTransform);

            // The per-pixel check is expensive, so check the bounding rectangles
            // first to prevent testing pixels when collisions are impossible.
            if (thisRectangle.Intersects(spriteRectangle))
            {
                // The color data for the images; used for per-pixel collision
                Color[] thisTextureData;        //calling object
                Color[] spriteTextureData;		//passed object

                //Extract collision data from calling object                
                thisTextureData =
                    new Color[Image.Width * Image.Height];
                Image.GetData(thisTextureData);

                //get colour data for other sprite                
                spriteTextureData =
                    new Color[sprite.Image.Width * sprite.Image.Height];
                sprite.Image.GetData(spriteTextureData);

                // Check collision 
                if (IntersectPixels(spriteTransform, sprite.Image.Width,
                        sprite.Image.Height, spriteTextureData,
                        thisTransform, Image.Width,
                        Image.Height, thisTextureData
                        ))
                {
                    //if per pixel is true, return true from the method
                    hit = true;
                }
            }
            //this will be false if there was no rectangle collision or if
            //there was a rectangle collision, but no per pixel collision 
            return hit;
        }

        //Uses the sixth overload of the Draw method to draw any sprite
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(Alive)
                spriteBatch.Draw(Image, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffect, 0.0f);
        }

        //Following five methods are all used to move sprites. These are not used in this assignment
        public virtual void Idle()
        {
            Velocity = Velocity * .95f;
        }

        public virtual void Left()
        {
            velocity.X -= InitialVelocity.X;
        }

        public virtual void Right()
        {
            velocity.X += InitialVelocity.X;
        }

        public virtual void Up()
        {
            velocity.Y -= InitialVelocity.Y;
        }

        public virtual void Down()
        {
            velocity.Y += InitialVelocity.Y;
        }

        //Default constructor for the sprite class. Takes seven arguments which will all be used to allow for
        //use of the sixth overload of the Draw method.
        public Sprite(Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin, float rotationSpeed,
            float scale, SpriteEffects spriteEffect)
        {
            InitialVelocity = velocity;
            Velocity = velocity;
            Position = position;
            Scale = scale;
            RotationSpeed = rotationSpeed;
            UseOrigin = useOrigin;
            SpriteEffect = spriteEffect;
            Image = textureImage;

            Force = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Mass = 0;

            if (useOrigin)
            {
                origin.X = textureImage.Width / 2;
                origin.Y = textureImage.Height / 2;
            }
            else
            {
                Origin = Vector2.Zero;
            }

            Alive = true;
        }

        //This update method moves any sprite that is alive but does not keep it on the screen
        public virtual void Update(GameTime gameTime)
        {
            if (Alive)
            {
                float timelapse = gameTime.ElapsedGameTime.Milliseconds / 1000f;

                //Added 03/15/2014
                Physics2D.setDisplacement(this, timelapse, false);
                //Position += Velocity * timelapse;
                Rotation += RotationSpeed * timelapse;
                Rotation = Rotation % (MathHelper.Pi * 2);
            }
        }

        //This update method calls the other update and ensures no sprite moves off the screen
        public virtual void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            if (Alive)
            {
                Update(gameTime);

                if (position.Y > (graphics.Viewport.Height - (origin.Y * Scale)))
                {
                    position.Y = graphics.Viewport.Height - (origin.Y * Scale);
                    velocity.Y *= -1;
                }
                if (position.X > (graphics.Viewport.Width - (origin.X * Scale)))
                {
                    position.X = graphics.Viewport.Width - (origin.X * Scale);
                    velocity.X *= -1;
                }
                if (position.Y < (origin.Y * Scale))
                {
                    position.Y = origin.Y * Scale;
                    velocity.Y *= -1;
                }
                if (position.X < origin.X)
                {
                    position.X = origin.X;
                    velocity.X *= -1;
                }
            }
        }

        public virtual bool isOffScreen(GraphicsDevice graphics)
        {
            if (position.X + Origin.X < 0 || Position.X - Origin.X > graphics.Viewport.Width ||
                Position.Y + Origin.Y < 0 || Position.Y - Origin.Y > graphics.Viewport.Height)
                return true;
            else
                return false;
        }
    }
}
