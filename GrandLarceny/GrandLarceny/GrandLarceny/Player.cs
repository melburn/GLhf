using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class Player : Entity
	{

        private const int PLAYERSPEED = 200;

        enum State
        {
            Stop,
            Walking,
            Jumping,
            Sliding,
            Climbing
        }
        State m_currentState = State.Stop;

		public Player(Vector2 a_posV2, string a_sprite)
			: base(a_posV2, a_sprite)
		{
		}

        public override void update(GameTime a_gameTime)
        {
            base.update(a_gameTime);

            KeyboardState t_keyInput = Keyboard.GetState();

            switch (m_currentState)
            {
                case State.Stop:
                {
                    updateStop(t_keyInput);
                    break;
                }
                case State.Walking:
                {
                    updateWalking(t_keyInput);
                    break;
                }
                case State.Jumping:
                {
                    updateJumping(t_keyInput);
                    break;
                }
                case State.Sliding:
                {
                    updateSliding(t_keyInput);
                    break;
                }
                case State.Climbing:
                {
                    updateClimbing(t_keyInput);
                    break;
                }
            }


        }

        //TODO, player ska kunna hoppa sig när han står still
        private void updateStop(KeyboardState t_keyInput)
        {
            if (t_keyInput.IsKeyDown(Keys.Left) || t_keyInput.IsKeyDown(Keys.Right))
            {
                m_currentState = State.Walking;
                changeAnimation();
                if (t_keyInput.IsKeyDown(Keys.Left))
                {
                    m_speed.X = -PLAYERSPEED;
                }
                else
                {
                    m_speed.X = PLAYERSPEED;
                }
            }
        }

        //TODO, player ska kunna hoppa här också, samt kollidering risk finns när han rör sig
        private void updateWalking(KeyboardState t_keyInput)
        {
            if (!t_keyInput.IsKeyDown(Keys.Left) || !t_keyInput.IsKeyDown(Keys.Right))
            {
                m_currentState = State.Stop;
                changeAnimation();
                m_speed.X = 0;
            }
        }

        private void updateJumping(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        private void updateSliding(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }
        
        private void updateClimbing(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        //TODO, titta sin state och ändra till rätt animation
        private void changeAnimation()
        {

        }

        public override void draw(GameTime a_gameTime)
        {

        }

	}
}
