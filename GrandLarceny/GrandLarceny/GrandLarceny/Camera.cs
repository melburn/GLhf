﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class Camera
	{
		private float m_zoom;
		//private Vector2 m_position;
		private float m_rotation;
		private Position m_position;

		public Camera()
		{
			m_zoom = 1.0f;
			m_rotation = 0.0f;
			m_position = new CartesianCoordinate(Vector2.Zero);
		}

		public float getZoom()
		{
			return m_zoom;
		}

		public void setZoom(float a_zoom)
		{
			m_zoom = Math.Max(a_zoom, 0.1f);
		}

		public float getRotation()
		{
			return m_rotation;
		}

		public void setRotation(float a_rotation)
		{
			m_rotation = a_rotation;
		}

		public Position getPosition()
		{
			return m_position;
		}

		public void setPosition(Vector2 a_posV2)
		{
			m_position.setCartesianCoordinates(a_posV2);
		}

		public void move(Vector2 a_posV2)
		{
			m_position.plusWith(a_posV2);
		}
		
		/*
		Använder magi för att förklara hur saker ska ritas ut 
		*/ 
		public Matrix getTransformation(GraphicsDevice a_gd)
		{
			Vector2 t_posV2 = m_position.getGlobalCartesianCoordinates();
			return Matrix.CreateTranslation(
				new Vector3(-t_posV2.X, -t_posV2.Y, 0)) 
				* Matrix.CreateRotationZ(m_rotation) 
				* Matrix.CreateScale(new Vector3(m_zoom, m_zoom, 1)) 
				* Matrix.CreateTranslation(new Vector3(Game.getInstance().m_graphics.PreferredBackBufferWidth * 0.5f, Game.getInstance().m_graphics.PreferredBackBufferHeight * 0.5f, 0)
			);
		}
	}
}