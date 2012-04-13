﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public abstract class Position
	{
		[NonSerialized]
		protected Position m_parentPosition;

		abstract public Vector2 getLocalCartesianCoordinates();
		abstract public Vector2 getGlobalCartesianCoordinates();
		abstract public Vector2 getLocalPolarCoordinates();
		abstract public Vector2 getGlobalPolarCoordinates();
		abstract public void setLocalCartesianCoordinates(Vector2 a_position);
		abstract public void setGlobalCartesianCoordinates(Vector2 a_position);
		abstract public void setLocalPolarCoordinates(float a_radius, float a_radians);
		abstract public void plusWith(Vector2 a_term);
		abstract public void setLength(float length);
		abstract public void rotate(float a_radians);
		abstract public float getLength();
		abstract public float getLocalX();
		abstract public float getLocalY();
		abstract public float getGlobalX();
		abstract public float getGlobalY();
		abstract public void setParentPositionWithoutMoving(Position a_parentPosition);
		abstract public float getSlope();
		abstract public void setLocalY(float a_y);
		abstract public void setLocalX(float a_x);
		abstract public void setGlobalY(float a_y);
		abstract public void setGlobalX(float a_x);
		abstract public void plusYWith(float a_y);
		abstract public void plusXWith(float a_x);
		abstract public void smoothStep(Vector2 a_vec, float a_amount);
		abstract public void setSlope(float a_rotation);

		public void setParentPosition(Position a_parentPosition)
		{
			Position t_parent = a_parentPosition;
			while (t_parent != null)
			{
				if (t_parent == this)
				{
					throw new ArgumentException("This parenting will cause an inheirt paradox");
				}
				else
				{
					t_parent = t_parent.getParentPosition();
				}
			}
			m_parentPosition = a_parentPosition;
		}

		public Position getParentPosition()
		{
			return m_parentPosition;
		}

		public static Vector2 convertCartesianToPolar(Vector2 a_cartesian)
		{
			return new Vector2(a_cartesian.Length(), (float)(Math.Atan2(a_cartesian.Y, a_cartesian.X)));
		}

		public static Vector2 convertPolarToCartesian(Vector2 a_polar)
		{
			return new Vector2((float)(a_polar.X * Math.Cos(a_polar.Y)), (float)(a_polar.X * Math.Sin(a_polar.Y)));
		}

		public float getDistanceTo(Position a_point)
		{
			return (a_point.getGlobalCartesianCoordinates()-getGlobalCartesianCoordinates()).Length();
		}
		public float getDistanceTo(Vector2 a_point)
		{
			return (a_point - getGlobalCartesianCoordinates()).Length();
		}

		public float getAngleTo(Vector2 a_point)
		{
			Vector2 t_ThisPoint = getGlobalCartesianCoordinates();
			return (float)(Math.Atan2((double)(t_ThisPoint.Y - a_point.Y), (double)(t_ThisPoint.X - a_point.X)) + Math.PI);
		}
	}
}
