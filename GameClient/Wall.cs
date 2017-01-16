using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GameClient
{

    public class Wall
    {
        //sprite texture and position
        Texture2D spriteImage;

        public Vector2 position;

        public bool visible = true;


        SpriteEffects _effect;

        public Rectangle BoundingRect;

        // track collision  state
        public bool InCollision = false;

        public Wall(Texture2D texture, Vector2 Position, Vector2 wallSize/*, int framecount*/)
        {
            // wall boundaries
            BoundingRect = new Rectangle(
                Convert.ToInt32(Position.X),
                Convert.ToInt32(Position.Y),
                Convert.ToInt32(wallSize.X),
                Convert.ToInt32(wallSize.Y)
                );

            spriteImage = texture;
            position = Position;
            

            _effect = SpriteEffects.None;

        }

        public void draw(SpriteBatch sp)
        {
            if (visible)
                sp.Draw(spriteImage, position, Color.White);
        }


        public virtual void Update(GameTime gametime)
        {}


        public bool collisionDetect(Sprite otherSprite)
        {
            if (BoundingRect.Intersects(otherSprite.BoundingRect))
            {
                InCollision = true;
                return true;
            }
            else
            {
                InCollision = false;
                return false;
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //draw the sprite , specify the postion and source for the image withtin the sprite sheet
            //spriteBatch.Begin();
            // Changed to allow for sprite effect
            if (visible)
                spriteBatch.Draw(spriteImage, position, BoundingRect, Color.White, 0f, Vector2.Zero, 1.0f, _effect, 0f);

            //spriteBatch.End();
        }

    }


}
