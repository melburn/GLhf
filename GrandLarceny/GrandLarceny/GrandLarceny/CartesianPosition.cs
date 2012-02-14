using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class CartesianCoordinate : Position
	{
		private Vector2 m_coordinates;

		public CartesianCoordinate(Vector2 a_coordinates)
		{
			m_coordinates = a_coordinates;
		}

		public CartesianCoordinate(Vector2 a_coordinates, Position a_parentPosition)
		{
			m_coordinates = a_coordinates;
			m_parentPosition = a_parentPosition;
		}

		public override Vector2 getLocalCartesianCoordinates()
		{
			return m_coordinates;
		}

		public override Vector2 getGlobalCartesianCoordinates()
		{
			if (m_parentPosition == null)
			{
				return m_coordinates;
			}
			else
			{
				return m_parentPosition.getGlobalCartesianCoordinates() + m_coordinates;
			}
		}

		public override Vector2 getLocalPolarCoordinates()
		{
			return convertCartesianToPolar(m_coordinates);
		}

		public override Vector2 getGlobalPolarCoordinates()
		{
			if (m_parentPosition == null)
			{
				return convertCartesianToPolar(m_coordinates);
			}
			else
			{
				return convertCartesianToPolar(m_parentPosition.getGlobalCartesianCoordinates() + m_coordinates);
			}
		}

		public override void setCartesianCoordinates(Vector2 a_position)
		{
			m_coordinates = a_position;
		}

		public override void setPolarCoordinates(float a_radius, float a_radians)
		{
			m_coordinates = convertPolarToCartesian(new Vector2(a_radius, a_radians));
		}

		public override void plusWith(Vector2 a_term)
		{
			m_coordinates += a_term;
		}

		public override void setLength(float length)
		{
			m_coordinates = Vector2.Normalize(m_coordinates) * length;
		}

		public override void rotate(float a_radians)
		{
			Vector2 t_polar = convertCartesianToPolar(m_coordinates);
			t_polar.Y += a_radians;
			m_coordinates = convertPolarToCartesian(t_polar);
		}

		public override float getLength()
		{
			return m_coordinates.Length();
		}

		public override float getX()
		{
			return m_coordinates.X;
		}

		public override float getY()
		{
			return m_coordinates.Y;
		}

		public override void setParentPositionWithoutMoving(Position a_parentPosition)
		{
			//kanske ska vara tvärtom
			m_coordinates = getGlobalCartesianCoordinates() - a_parentPosition.getGlobalCartesianCoordinates();

			m_parentPosition = a_parentPosition;
		}

		public override float getSlope()
		{
			return (float)Math.Atan2(m_coordinates.Y, m_coordinates.X);
		}

		public PolarCoordinate convertToPolarCoordinate()
		{
			return new PolarCoordinate(convertCartesianToPolar(m_coordinates));
		}

		public override void setY(float y)
		{
			m_coordinates.Y = y;
		}

		public override void setX(float x)
		{
			m_coordinates.X = x;
		}

		public override void smoothStep(Vector2 a_vec, float a_amount)
		{
			m_coordinates = Vector2.SmoothStep(m_coordinates, a_vec, a_amount);
		}

		public override void setSlope(float m_rotation)
		{
			Vector2 t_polarCoordinate = convertCartesianToPolar(m_coordinates);
			t_polarCoordinate.Y = m_rotation;
			m_coordinates = convertPolarToCartesian(t_polarCoordinate);
		}

		public override Position getProductWith(float p)
		{
			return new CartesianCoordinate(m_coordinates * p);
		}
	}
}
