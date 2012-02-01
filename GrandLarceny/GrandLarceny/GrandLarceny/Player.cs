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
		private const int JUMPSTREANGTH = 300;
		private Vector2 m_cameraPoint = new Vector2(0,0);
		private const int CAMERAMAXDISTANCE = 100;
		private const float CAMERASPEED = 0.1f;

		private const int ACCELERATION = 2000;
		private const int DEACCELERATION = 800;


		KeyboardState m_currentKeyInput;
		KeyboardState m_previousKeyInput;

		private Boolean m_faceingRight = false;

        enum State
        {
            Stop,
            Walking,
            Jumping,
            Sliding,
            Climbing
        }
        State m_currentState = State.Stop;

		public Player(Vector2 a_posV2)
			: base(a_posV2, "Images//WalkingSquareStand")
		{
			m_currentState = State.Jumping;
			m_gravity = 500f;
		}

        public override void update(GameTime a_gameTime)
		{
			m_previousKeyInput = m_currentKeyInput;
            m_currentKeyInput = Keyboard.GetState();
			float t_deltaTime = ((float) a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
            switch (m_currentState)
            {
                case State.Stop:
                {
                    updateStop();
                    break;
                }
                case State.Walking:
                {
					updateWalking(t_deltaTime);
                    break;
                }
                case State.Jumping:
                {
					updateJumping(t_deltaTime);
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
			base.update(a_gameTime);
			Game.getInstance().m_camera.getPosition().smoothStep(m_cameraPoint, CAMERASPEED);
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
					m_faceingRight = false;
					m_spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
					m_faceingRight = true;
					m_spriteEffects = SpriteEffects.None;
						
                }
            }
			if (m_currentKeyInput.IsKeyDown(Keys.Up))
			{
				m_speed.Y -= JUMPSTREANGTH;
				m_currentState = State.Jumping;
				changeAnimation();
			}

			//Game.getInstance().m_camera.getPosition().smoothStep(Vector2.Zero, CAMERASPEED);
        }

        //TODO, player ska kunna hoppa här också, samt kollidering risk finns när han rör sig
        private void updateWalking(float a_deltaTime)
        {
			if(m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), PLAYERSPEED);
			}
			if (m_currentKeyInput.IsKeyDown(Keys.Left))
			{
				m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -PLAYERSPEED);
			}
			if (m_speed.X > 0)
			{
				m_speed.X = Math.Max(m_speed.X - (DEACCELERATION * a_deltaTime), 0);
				m_faceingRight = true;
				m_spriteEffects = SpriteEffects.None;
			}
			else if (m_speed.X < 0)
			{
				m_speed.X = Math.Min(m_speed.X + (DEACCELERATION * a_deltaTime), 0);
				m_faceingRight = false;
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}

            if (m_speed.X == 0)
            {
                m_currentState = State.Stop;
                changeAnimation();
            }
			if (m_currentKeyInput.IsKeyDown(Keys.Up))
			{
				m_speed.Y -= JUMPSTREANGTH;
				m_currentState = State.Jumping;
				changeAnimation();
			}

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);

		
			m_img.setAnimationSpeed(Math.Abs(m_speed.X / 10f));
			
        }

		private void updateJumping(float a_deltaTime)
        {
			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
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
			if (m_currentState == State.Stop)
			{
				m_img.setSprite("Images//WalkingSquareStand");
			}
			else if (m_currentState == State.Walking)
			{
				m_img.setSprite("Images//WalkingSquareWalking");
			}
			else if (m_currentState == State.Jumping)
			{
				if (m_speed.Y > 0)
				{
					m_img.setSprite("Images//WalkingSquareJumping");
				}
				else
				{
					m_img.setSprite("Images//WalkingSquareFalling");
				}
			}



        }
		
        public override void draw(GameTime a_gameTime)
        {
			base.draw(a_gameTime);
        }

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_collider in a_collisionList)
			{
				if (t_collider is Platform)
				{
					//Colliding with ze floor
					if ((int)(m_lastPosition.Y+(m_img.getSize().Y/2)) - 2 <= (int)(t_collider.getLastPosition().Y-(t_collider.getImg().getSize().Y/2)))
					{
						m_position.setY(t_collider.getBox().Y - (m_img.getSize().Y / 2));
						m_speed.Y = 0;
						if (m_currentState == State.Jumping)
						{
							if (m_speed.X == 0)
							{
								m_currentState = State.Stop;
							}
							else
							{
								m_currentState = State.Walking;
							}
						}
					}
					
					//Colliding with ze zeeling
					if ((int)(m_lastPosition.Y - (m_img.getSize().Y / 2)) + 2 >= (int)(t_collider.getLastPosition().Y + (t_collider.getImg().getSize().Y / 2)))
					{
						m_position.setY(t_collider.getBox().Y + t_collider.getBox().Height + (m_img.getSize().Y / 2));
						m_speed.Y = 0;
					}
					//Colliding with ze left wall
					if ((int)(m_lastPosition.X - (m_img.getSize().X / 2)) + 2 >= (int)(t_collider.getLastPosition().X + (t_collider.getImg().getSize().X / 2)))
					{
						if(t_collider.getTopLeftPoint().X + t_collider.getImg().getSize().X < 0)
							setLeftPoint(t_collider.getTopLeftPoint().X + t_collider.getImg().getSize().X);
						else
							setLeftPoint(t_collider.getTopLeftPoint().X + t_collider.getImg().getSize().X);
						m_speed.X = 0;
						Console.Out.WriteLine(getLeftPoint() - t_collider.getRightPoint());
					}
					//Colliding with ze right wall
					if ((int)(m_lastPosition.X + (m_img.getSize().X / 2)) - 2 <= (int)(t_collider.getLastPosition().X - (t_collider.getImg().getSize().X / 2)))
					{
						setLeftPoint(t_collider.getTopLeftPoint().X - (m_img.getSize().X));
						m_speed.X = 0;
					}
				}
			}

			//m_currentState = State.Stop;
		}
	}
}

