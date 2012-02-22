using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class ImageManager
	{
		private Texture2D m_image;
		private int m_animationWidth;
		private int m_animationFrames;
		public bool m_looping;
		private bool m_stopped = false;
		private string m_imagePath;

		//millisecond per frame
		public float m_animationSpeed;

		//säger vilken subbild i animationen den ligger på, med decimaler.
		public float m_subImageNumber;

		public ImageManager(string a_sprite)
		{
			setSprite(a_sprite);
			m_animationSpeed = 33f;
			m_looping = true;
		}

		public void update(GameTime a_gameTime)
		{
			if (!m_stopped)
			{
				m_subImageNumber += m_animationSpeed * a_gameTime.ElapsedGameTime.Milliseconds/1000;
				if (m_subImageNumber >= m_animationFrames)
				{
					if (m_looping)
					{
						m_subImageNumber = m_subImageNumber % m_animationFrames;
					}
					else
					{
						m_subImageNumber = m_animationFrames;
						m_stopped = true;
					}
				}
			}
		}

		public void draw(Vector2 a_imgPosition, float a_rotation, Color a_color, SpriteEffects a_spriteEffect = SpriteEffects.None, float a_layer = 0.0f, float a_xScale = 1.0f, float a_yScale = 1.0f)
		{
			if (a_xScale <= 0)
			{
				throw new ArgumentException("xScale has to be positive. was "+a_xScale);
			}
			if (a_yScale <= 0)
			{
				throw new ArgumentException("yScale has to be positive. was "+a_yScale);
			}
			if (a_color == null)
			{
				a_color = Color.White;
			}
			//Vector2 t_worldPosV2 = a_position.getGlobalCartesianCoordinates();

			Game.getInstance().getSpriteBatch().Draw(
				m_image,
				new Rectangle((int)(Math.Round(a_imgPosition.X)), (int)(Math.Round(a_imgPosition.Y)), (int)(m_animationWidth * a_xScale), (int)(m_image.Height * a_yScale)),
				new Rectangle(m_animationWidth * ((int)(m_subImageNumber)), 0, m_animationWidth, m_image.Height),
				a_color,
				a_rotation,
				Vector2.Zero,
				a_spriteEffect,
				a_layer
			);
		}

		public void setSprite(string a_sprite)
		{
			
			if (a_sprite == null)
			{
				m_image = null;
				m_stopped = true;
			}
			else if (!a_sprite.Equals(m_imagePath))
			{
				m_stopped = false;
				m_image = Game.getInstance().Content.Load<Texture2D>(a_sprite);
				m_animationFrames = Loader.getInstance().getAnimationFrames(a_sprite);
				m_animationWidth = m_image.Width / m_animationFrames;
				m_subImageNumber = 0;
				
			}
			m_imagePath = a_sprite;
		}

		public void stop()
		{
			m_stopped = true;
		}
	
		public void run()
		{
			m_stopped = (m_image == null);
		}

		public Vector2 getSize()
		{
			return new Vector2(m_animationWidth, m_image.Height);
		}

		public void setAnimationSpeed(float a_speed)
		{
			if (a_speed < 0)
			{
				throw new ArgumentException("Animation speed cannot be negative");
			}
			m_animationSpeed = a_speed;
		}

		public string getImagePath()
		{
			return m_imagePath;
		}

		public float getSubImageIndex()
		{
			return m_subImageNumber;
		}
	}
}
