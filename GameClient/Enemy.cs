using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace GameClient
{
    class Enemy : Sprite
    {
        protected Game myGame;
        private float _velocity = 4.0f;

        public float Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        protected Vector2 startPosition;

        public Enemy(Texture2D texture, Vector2 userPosition, int framecount) : base(texture, userPosition, framecount)
        {
            startPosition = userPosition;
        }

    }
}
