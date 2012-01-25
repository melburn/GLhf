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

        private void updateClimbing(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        private void updateSliding(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        private void updateJumping(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        private void updateWalking(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        private void updateStop(KeyboardState t_keyInput)
        {
            throw new NotImplementedException();
        }

        public override void draw(GameTime a_gameTime)
        {

        }

	}
}
