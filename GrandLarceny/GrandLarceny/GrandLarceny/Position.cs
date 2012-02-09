using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public abstract class Position
	{
		protected Position m_parentPosition;

		abstract public Vector2 getLocalCartesianCoordinates();
		abstract public Vector2 getGlobalCartesianCoordinates();
		abstract public Vector2 getLocalPolarCoordinates();
		abstract public Vector2 getGlobalPolarCoordinates();
		abstract public void setCartesianCoordinates(Vector2 a_position);
		abstract public void setPolarCoordinates(float a_radius, float a_radians);
		abstract public void plusWith(Vector2 a_term);
		abstract public void setLength(float length);
		abstract public void rotate(float a_radians);
		abstract public float getLength();
		abstract public float getX();
		abstract public float getY();
		abstract public void setParentPositionWithoutMoving(Position a_parentPosition);
		abstract public float getSlope();
		abstract public void setY(float y);
		abstract public void setX(float x);
		abstract public void smoothStep(Vector2 a_vec, float a_amount);
		abstract public void setSlope(float m_rotation);

		public void setParentPosition(Position a_parentPosition)
		{
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

		public float getAngleTo(Position a_point)
		{
			Vector2 t_ThisPoint = getGlobalCartesianCoordinates();
			Vector2 t_ArgPoint = a_point.getGlobalCartesianCoordinates();
			return (float) Math.Atan2((double)(t_ThisPoint.Y - t_ArgPoint.Y),(double)(t_ThisPoint.X - t_ArgPoint.X));
		}

	}
}
