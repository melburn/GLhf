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
		private int m_layer = 0;
		private float m_zoom;
		private float m_rotation;
		private Position m_position;
		private CollisionRectangle m_cameraBox;

		public Camera()
		{
			m_zoom = 1.0f;
			m_rotation = 0.0f;
			m_position = new CartesianCoordinate(Vector2.Zero);
		}

		public void load() {
			m_cameraBox = new CollisionRectangle(-Game.getInstance().getResolution().X, -Game.getInstance().getResolution().Y, Game.getInstance().getResolution().X * 2, Game.getInstance().getResolution().Y * 2, m_position);			
		}

		public float getZoom()
		{
			return m_zoom;
		}

		public void setZoom(float a_zoom)
		{
			m_zoom = Math.Max(a_zoom, 0.1f);
		}

		public void zoomIn(float a_zoom)
		{
			m_zoom = Math.Min(m_zoom + a_zoom, 2.0f);
		}

		public void zoomOut(float a_zoom)
		{
			m_zoom = Math.Max(m_zoom - a_zoom, 0.1f);
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
			m_position.setLocalCartesianCoordinates(a_posV2);
		}

		public void move(Vector2 a_posV2)
		{
			m_position.plusWith(a_posV2);
		}

		public void setParentPosition(Position a_parent)
		{
			m_position.setParentPosition(a_parent);
		}

		public bool isInCamera(GameObject a_gameObject)
		{
			return CollisionManager.Contains(m_cameraBox, a_gameObject.getPosition().getGlobalCartesianCoordinates());
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
				* Matrix.CreateTranslation(new Vector3(Game.getInstance().getResolution().X * 0.5f, Game.getInstance().getResolution().Y * 0.5f, 0)
			);
		}

		internal int getLayer()
		{
			return m_layer;
		}
		internal void setLayer(int a_layer)
		{
			m_layer = a_layer;
		}

		public void printInfo() {
			System.Console.WriteLine(m_cameraBox.ToString());
		}
	}
}