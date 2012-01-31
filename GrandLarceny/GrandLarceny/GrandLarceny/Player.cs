using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class Player : Entity
	{

        private const int PLAYERSPEED = 200;

		KeyboardState m_currentKeyInput;
		KeyboardState m_previousKeyInput;

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
			m_currentState = State.Walking;
		}

        public override void update(GameTime a_gameTime)
        {
            m_currentKeyInput = Keyboard.GetState();

            switch (m_currentState)
            {
                case State.Stop:
                {
                    updateStop();
                    break;
                }
                case State.Walking:
                {
                    updateWalking();
                    break;
                }
                case State.Jumping:
                {
                    updateJumping();
                    break;
                }
                case State.Sliding:
                {
                    updateSliding();
                    break;
                }
                case State.Climbing:
                {
                    updateClimbing();
                    break;
                }
            }
			System.Console.WriteLine(m_position.getGlobalCartesianCoordinates());
			m_previousKeyInput = m_currentKeyInput;
			base.update(a_gameTime);
        }

        //TODO, player ska kunna hoppa sig när han står still
        private void updateStop()
        {
            if (m_currentKeyInput.IsKeyDown(Keys.Left) || m_currentKeyInput.IsKeyDown(Keys.Right))
            {
                m_currentState = State.Walking;
                changeAnimation();
                if (m_currentKeyInput.IsKeyDown(Keys.Left))
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
        private void updateWalking()
        {
            if (!m_currentKeyInput.IsKeyDown(Keys.Left) && !m_currentKeyInput.IsKeyDown(Keys.Right))
            {
                m_currentState = State.Stop;
                changeAnimation();
                m_speed.X = 0;
            }
        }

        private void updateJumping()
        {
            throw new NotImplementedException();
        }

        private void updateSliding()
        {
            throw new NotImplementedException();
        }
        
        private void updateClimbing()
        {
            throw new NotImplementedException();
        }

        //TODO, titta sin state och ändra till rätt animation
        private void changeAnimation()
        {

        }
		
        public override void draw(GameTime a_gameTime)
        {
			base.draw(a_gameTime);
        }

		internal override void collisionCheck(List<Entity> a_collisionList)
		{

			m_currentState = State.Stop;
		}
	}
}
