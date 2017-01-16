using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameClient
{
    public class Player : Sprite
    {
        //creating enum to determine direction the player is going
        public enum STATE { NORMAL, CHERRYFORM};
        STATE _state = STATE.NORMAL;

        public Guid id;

        public bool sneaky;
        public bool attractive;

        public bool cherryform = false;

        public int health;
        public int score;

        public STATE Direction
        {
            get { return _state; }
            set { _state = value; }
        }

        float _speed;
        Texture2D[] _textures;

        public int rageTimer = 600;

        //inheriting from AnimatedSprite
        public Player(Texture2D[] texture, Vector2 pos, int framecount, float speed)
            :base(texture[0], pos, framecount)
        {
            _speed = speed;
            _textures = texture;
            health = 100;
            sneaky = false;
            attractive = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //setting the dfault direction to standing
            _state = STATE.NORMAL;

            if(cherryform)
            {
                _state = STATE.CHERRYFORM;

                rageTimer--;
            }
            else
            {
                _state = STATE.NORMAL;
                rageTimer = 600;
            }

            if(rageTimer <= 0)
            {
                cherryform = false;
            }

            //moving in various directions and setting the direction variable to the direction 
            //it corresponds to
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                base.Move(new Vector2(-1, 0) * _speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                base.Move(new Vector2(0, -1) * _speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                base.Move(new Vector2(1, 0) * _speed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                base.Move(new Vector2(0, 1) * _speed);
            }

            //setting the image we are using to the direction variable that is cast as an int
            SpriteImage = _textures[(int)_state];
        }
    }




    public class speedyPlayer : Player
    {

        public speedyPlayer(Texture2D[] texture, Vector2 pos, int framecount, float speed)
           : base  (texture, pos , framecount , speed)
        {
            speed = speed * 3000f;
        }

    }


    public class healthyPlayer : Player
    {

        public healthyPlayer(Texture2D[] texture, Vector2 pos, int framecount, float speed)
           : base(texture, pos, framecount, speed)
        {
            this.health = 200;
        }

    }

    public class ladyPlayer : Player
    {

        public ladyPlayer(Texture2D[] texture, Vector2 pos, int framecount, float speed)
           : base(texture, pos, framecount, speed)
        {
            attractive = true;
          //  this.health = 200;
        }
    }

    public class sneakPlayer : Player
    {

        public sneakPlayer(Texture2D[] texture, Vector2 pos, int framecount, float speed)
           : base(texture, pos, framecount, speed)
        {
            this.sneaky = true;
            speed = speed/2;
        }
    }

}
