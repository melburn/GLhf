using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class Box : Environment
	{
		#region Members
		private Texture2D			m_boxTexture;
		private int					m_width;
		private int					m_height;
		private LinkedList<Line>	m_lineList;
		private Color				m_boxColor;
		private bool				m_worldBox;

		private Vector2 m_from;
		private Vector2 m_to;
		private float m_timer;
		private float m_timeStart;
		#endregion

		#region Constructor & Load
		public Box(Vector2 a_position, int a_width, int a_height, Color a_color, bool a_worldBox)
			: base(a_position, "", 0.110f)
		{
			m_boxTexture	= new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			if (a_worldBox)
			{
				m_position	= new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2);
			}
			else
			{
				m_position	= new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());				
			}
			m_lineList = new LinkedList<Line>();
			m_boxColor		= a_color;
			m_width			= a_width;
			m_height		= a_height;
			m_worldBox		= a_worldBox;
			m_boxTexture.SetData(new[] { a_color });
		}

		public Box(Vector2 a_position, int a_width, int a_height, Color a_color, Color a_lineColor, int a_lineWidth, bool a_worldBox)
			: base(a_position, "", 0.110f)
		{
			m_boxTexture	= new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_boxTexture.SetData(new[] { a_color });
			if (a_worldBox)
			{
				m_position	= new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2);
			}
			else
			{
				m_position	= new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());
			}
			m_boxColor		= a_color;
			m_width			= a_width;
			m_height		= a_height;
			m_worldBox		= a_worldBox;
			
			Vector2 topLeft = a_position;
			Vector2 topRight = a_position;
			topRight.X += a_width;
			Vector2 btmRight = topRight;
			btmRight.Y += a_height;
			Vector2 btmLeft = btmRight;
			btmLeft.X = topLeft.X;

			m_lineList = new LinkedList<Line>();
			if (a_worldBox)
			{
				m_lineList.AddLast(new Line(new CartesianCoordinate(topLeft),	new CartesianCoordinate(topRight), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(new CartesianCoordinate(topRight),	new CartesianCoordinate(btmRight), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(new CartesianCoordinate(btmRight),	new CartesianCoordinate(btmLeft), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(new CartesianCoordinate(btmLeft),	new CartesianCoordinate(topLeft), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth, a_worldBox));
			}
			else
			{
				Position t_camera = Game.getInstance().m_camera.getPosition();
				m_lineList.AddLast(new Line(t_camera, t_camera, topLeft		- Game.getInstance().getResolution() / 2, topRight - Game.getInstance().getResolution() / 2, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(t_camera, t_camera, topRight	- Game.getInstance().getResolution() / 2, btmRight - Game.getInstance().getResolution() / 2, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(t_camera, t_camera, btmRight	- Game.getInstance().getResolution() / 2, btmLeft - Game.getInstance().getResolution() / 2, a_lineColor, a_lineWidth, a_worldBox));
				m_lineList.AddLast(new Line(t_camera, t_camera, btmLeft		- Game.getInstance().getResolution() / 2, topLeft - Game.getInstance().getResolution() / 2, a_lineColor, a_lineWidth, a_worldBox));
			}
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			if (m_timer >= (float)a_gameTime.TotalGameTime.TotalMilliseconds)
			{
				/*m_from.X = m_position.getGlobalX();
				m_to.X = m_position.getGlobalX();
				m_from.Y += m_position.getGlobalY();
				m_to.Y += m_position.getGlobalY();*/
				float t_moveDelta = ((float)a_gameTime.TotalGameTime.TotalMilliseconds - m_timeStart) / (m_timer - m_timeStart);
				Vector2 tLerp = Vector2.Lerp(m_from, m_to, t_moveDelta);
				m_position.setLocalCartesianCoordinates(tLerp);
			}
			else if (m_timer > 0)
			{
				m_position.setLocalCartesianCoordinates(m_to);
				m_timer = 0;
			}
		}

		public override void draw(GameTime a_gameTime)
		{
			if (m_worldBox)
			{
				Game.getInstance().getSpriteBatch().Draw(m_boxTexture, m_position.getGlobalCartesianCoordinates(), null, m_boxColor, 0.0f, Vector2.Zero, new Vector2(m_width, m_height), SpriteEffects.None, m_layer);
				
				if (m_lineList != null && m_lineList.Count > 0)
				{
					foreach (Line t_line in m_lineList)
					{
						t_line.draw();
					}
				}
			}
			else
			{
				float t_zoom = Game.getInstance().m_camera.getZoom();
				Vector2 t_cartCoord;
				t_cartCoord.X = m_position.getLocalX() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalX();
				t_cartCoord.Y = m_position.getLocalY() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalY();
				
				Game.getInstance().getSpriteBatch().Draw(m_boxTexture, t_cartCoord, null, m_boxColor, 0.0f, Vector2.Zero, new Vector2(m_width / t_zoom, m_height / t_zoom), SpriteEffects.None, m_layer);
				
				if (m_lineList != null && m_lineList.Count > 0)
				{
					foreach (Line t_line in m_lineList)
					{
						t_line.draw();
					}
				}
			}
		}
		#endregion

		#region Box Methods
		public bool contains(Vector2 a_position)
		{
			if (a_position.X > m_position.getGlobalX() && a_position.Y > m_position.getGlobalY() && a_position.X < m_position.getGlobalX() + m_width  && a_position.Y < m_position.getGlobalY() + m_height)
			{
				return true;
			}
			return false;
		}

		public void setLineColor(Color a_color)
		{
			foreach (Line t_line in m_lineList)
			{
				t_line.setColor(a_color);
			}
		}

		public int getHeight()
		{
			return m_height;
		}

		public int getWidth()
		{
			return m_width;
		}

		public void setMove(Vector2 a_from, Vector2 a_to, GameTime a_gameTime,float a_timer)
		{
			m_to = a_to;
			m_from = a_from;
			m_timer = (float)a_gameTime.TotalGameTime.TotalMilliseconds + a_timer * 1000;
			m_timeStart = (float)a_gameTime.TotalGameTime.TotalMilliseconds;
		}
		#endregion
	}
}