using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpriteClasses
{
    public class SpriteFromSheet: Sprite
    {
        protected Vector2 totalFrames;
        public Vector2 TotalFrames 
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        protected Vector2 startFrame;
        public Vector2 StartFrame 
        {
            get { return startFrame; }
            set { startFrame = value; }
        }

        protected Vector2 currentFrame;
        public Vector2 CurrentFrame 
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public Rectangle FrameSize { get; set; }
        public Rectangle CurrentImage { get; set; }

        public float TimeSinceLastFrame { get; set; }
        public float TimeBetweenFrames { get; set; }
        public float TotalTime { get; set; }

        public bool isLooped { get; set; }

        public SpriteFromSheet(Texture2D image, Vector2 position, Vector2 velocity, Vector2 totalFrames, Vector2 startFrame,
            float totalTime, bool looped)
            :base(image, position, velocity, true, 0.0f, 1.0f, SpriteEffects.None)
        {
            TotalFrames = totalFrames;
            StartFrame = startFrame;
            CurrentFrame = startFrame;

            FrameSize = new Rectangle((int)StartFrame.X, (int)StartFrame.Y,
                (int)(Image.Width / TotalFrames.X), (int)(Image.Height / TotalFrames.Y));

            CurrentImage = FrameSize;

            Origin = new Vector2(FrameSize.Width / 2, FrameSize.Height / 2);

            TotalTime = totalTime * 1000;
            TimeBetweenFrames = TotalTime / (TotalFrames.X * TotalFrames.Y);
            TimeSinceLastFrame = 0;

            isLooped = looped;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, CurrentImage, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0.0f);
        }

        public override void Update(GameTime gameTime)
        {
            //If required timelapse is met
            if (TimeSinceLastFrame > TimeBetweenFrames)
            {
                //if the current frame is not at the width of the sheet
                if (CurrentFrame.X < TotalFrames.X * FrameSize.Width - FrameSize.Width)
                {
                    currentFrame.X += FrameSize.Width;
                }
                else
                {
                    //If current frame wont go beyond the height
                    if (CurrentFrame.Y < TotalFrames.Y * FrameSize.Height - FrameSize.Height)
                    {
                        currentFrame.X = 0;
                        currentFrame.Y += FrameSize.Height;
                    }
                    //Else if it is, and is to be looped
                    else if (CurrentFrame.Y >= TotalFrames.Y * FrameSize.Height - FrameSize.Height && isLooped)
                    {
                        currentFrame = Vector2.Zero;
                    }
                    else
                    {
                        CurrentFrame = new Vector2(Image.Width - FrameSize.Width, Image.Height - FrameSize.Height);
                    }

                }

                CurrentImage = new Rectangle((int)CurrentFrame.X, (int)CurrentFrame.Y,
                    FrameSize.Width, FrameSize.Height);

                TimeSinceLastFrame = 0;
            }
            else
                TimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

            base.Update(gameTime);
        }
    }
}
