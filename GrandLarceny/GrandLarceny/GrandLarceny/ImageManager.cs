using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GrandLarceny
{
	public class ImageManager
	{
		private Texture2D m_image;
		private int m_animationWidth;
		private int m_animationFrames;
		private bool m_looping;
		private bool m_stopped = false;
		private string m_imagePath;

		//millisecond per frame
		public float m_animationSpeed;

		//säger vilken subbild i animationen den ligger på, med decimaler.
		private float m_subImageNumber;

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
				if (m_subImageNumber >= m_animationFrames || m_subImageNumber < 0)
				{
					if (m_looping)
					{
						m_subImageNumber = m_subImageNumber % m_animationFrames;
					}
					else
					{
						if (m_subImageNumber < 0)
						{
							m_subImageNumber = 0;
						}
						else
						{
							m_subImageNumber = m_animationFrames - 0.1f;
						}
						m_stopped = true;
					}
				}
			}
		}

		public void draw(Vector2 a_imgPosition, float a_rotation, Vector2 a_origin, Color a_color, SpriteEffects a_spriteEffect = SpriteEffects.None, float a_layer = 0.0f, float a_xScale = 1.0f, float a_yScale = 1.0f)
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

			Game.getInstance().getSpriteBatch().Draw(
				m_image,
				new Rectangle((int)(Math.Round(a_imgPosition.X)+(a_origin.X*a_xScale)), (int)(Math.Round(a_imgPosition.Y)+(a_origin.Y*a_yScale)), (int)(m_animationWidth * a_xScale), (int)(m_image.Height * a_yScale)),
				new Rectangle(m_animationWidth * ((int)(m_subImageNumber)), 0, m_animationWidth, m_image.Height),
				a_color,
				a_rotation,
				a_origin,
				a_spriteEffect,
				a_layer
			);
		}

		public void setSprite(Texture2D a_texture) {
			m_stopped = true;
			m_imagePath = null;
			m_image = a_texture;
		}

		public bool setSprite(string a_sprite)
		{
			if (a_sprite == null || a_sprite.Equals(""))
			{
				m_image = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
				Color t_color = Color.Black;
				m_image.SetData(new[] { t_color });
				m_stopped = true;
				m_imagePath = null;
				return false;
			}
			else if (!a_sprite.Equals(m_imagePath))
			{
				m_stopped = false;
				m_looping = true;
				try
				{
					m_image = Game.getInstance().Content.Load<Texture2D>(a_sprite);
				}
				catch (ContentLoadException)
				{
					System.Console.WriteLine("Omega fail to load texture : " + a_sprite);
					if (a_sprite.Equals("Images//GUI//")) {
						m_image = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1);
						m_image.SetData<Color>(new[] { Color.Transparent });
					} else {
						m_image = Game.getInstance().Content.Load<Texture2D>("Images//Tile//1x1_tile_ph");
					}
				}
				m_animationFrames = Loader.getInstance().getAnimationFrames(a_sprite);
				m_animationWidth = m_image.Width / m_animationFrames;
				m_subImageNumber = 0;
				m_imagePath = a_sprite;
				return true;
			}
			else
			{
				return false;
			}
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

		public void setSubImage(float a_index)
		{
			if (a_index < 0)
			{
				throw new ArgumentException();
			}
			if (a_index < m_animationFrames)
			{
				m_subImageNumber = a_index;
			}
			else
			{
				if (m_looping)
				{
					m_subImageNumber = a_index % m_subImageNumber;
				}
				else
				{
					m_subImageNumber = m_animationFrames - 0.1f;
					m_stopped = true;
				}
			}
		}

		public bool isStopped()
		{
			return m_stopped;
		}

		public void setLooping(Boolean a_looping)
		{
			m_looping = a_looping;
		}

		public int getLength()
		{
			return m_animationFrames;
		}
	}
}
