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
		private Vector2 m_position;
		private float m_rotation;

		public Camera()
		{
			m_zoom = 1.0f;
			m_rotation = 0.0f;
			m_position = Vector2.Zero;
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

		public Vector2 getPosition()
		{
			return m_position;
		}

		public void setPosition(int a_XPos, int a_YPos)
		{
			m_position.X = a_XPos;
			m_position.Y = a_YPos;
		}

		public void move(Vector2 a_v2)
		{
			m_position += a_v2;
		}
		
		/*
		Använder magi för att förklara hur saker ska ritas ut 
		*/ 
		public Matrix getTransformation(GraphicsDevice a_gd)
		{
			return Matrix.CreateTranslation(
				new Vector3(-m_position.X, -m_position.Y, 0)) 
				* Matrix.CreateRotationZ(m_rotation) 
				* Matrix.CreateScale(new Vector3(m_zoom, m_zoom, 1)) 
				* Matrix.CreateTranslation(new Vector3(1280 * 0.5f, 720 * 0.5f, 0)
			);
		}
	}
}