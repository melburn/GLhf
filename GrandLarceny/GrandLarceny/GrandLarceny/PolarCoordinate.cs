using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class PolarCoordinate : Position
	{
		//X är radien Y är vinkeln i form a radianer
		private Vector2 m_coordinates;

		public PolarCoordinate(Vector2 a_coordinates)
		{
			m_coordinates = a_coordinates;
		}
		public PolarCoordinate(Vector2 a_coordinates, Position a_parentPosition)
		{
			m_coordinates = a_coordinates;
			m_parentPosition = a_parentPosition;
		}
		public PolarCoordinate(float a_radie, float a_slope)
		{
			m_coordinates = new Vector2(a_radie, a_slope);
		}

		public PolarCoordinate(float a_radie, float a_slope, Position a_parent)
		{
			m_coordinates = new Vector2(a_radie, a_slope);
			m_parentPosition = a_parent;
		}

		public override Vector2 getLocalCartesianCoordinates()
		{
			return convertPolarToCartesian(m_coordinates);
		}

		public override Vector2 getGlobalCartesianCoordinates()
		{
			if (m_parentPosition == null)
			{
				return getLocalCartesianCoordinates();
			}
			else
			{
				return m_parentPosition.getGlobalCartesianCoordinates() + getLocalCartesianCoordinates();
			}
		}

		public override Vector2 getLocalPolarCoordinates()
		{
			return m_coordinates;
		}

		public override Vector2 getGlobalPolarCoordinates()
		{
			if (m_parentPosition == null)
			{
				return m_coordinates;
			}
			else
			{
				return convertCartesianToPolar(getGlobalCartesianCoordinates());
			}
		}

		public override void setLocalCartesianCoordinates(Vector2 a_position)
		{
			m_coordinates = convertCartesianToPolar(a_position);
		}

		public override void setLocalPolarCoordinates(float a_radius, float a_radians)
		{
			m_coordinates.X = a_radius;
			m_coordinates.Y = a_radians;
		}

		public override void plusWith(Vector2 a_term)
		{
			if (a_term.Length() != 0)
			{
				m_coordinates = convertCartesianToPolar(convertPolarToCartesian(m_coordinates) + a_term);
			}
		}

		public override void setLength(float length)
		{
			m_coordinates.X = length;
		}

		public override void rotate(float a_radians)
		{
			m_coordinates.Y += a_radians;
		}

		public override float getLength()
		{
			return m_coordinates.X;
		}

		public override float getLocalX()
		{
			return (float)(m_coordinates.X * Math.Cos(m_coordinates.Y));
		}

		public override float getLocalY()
		{
			return (float)(m_coordinates.X * Math.Sin(m_coordinates.Y));
		}
		public override float getGlobalX()
		{
			if (m_parentPosition == null)
			{
				return (float)(m_coordinates.X * Math.Cos(m_coordinates.Y));
			}
			else
			{
				return (float)(m_coordinates.X * Math.Cos(m_coordinates.Y) + m_parentPosition.getGlobalX());
			}
		}
		public override float getGlobalY()
		{
			if (m_parentPosition == null)
			{
				return (float)(m_coordinates.X * Math.Sin(m_coordinates.Y));
			}
			else
			{
				return (float)(m_coordinates.X * Math.Sin(m_coordinates.Y) + m_parentPosition.getGlobalY());
			}
		}
		public override void setParentPositionWithoutMoving(Position a_parentPosition)
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
			if (a_parentPosition == null)
			{
				m_coordinates = getGlobalPolarCoordinates();
			}
			else{
				m_coordinates = convertCartesianToPolar(getGlobalCartesianCoordinates() - a_parentPosition.getGlobalCartesianCoordinates());
			}
			m_parentPosition = a_parentPosition;
		}

		public override float getSlope()
		{
			return m_coordinates.Y;
		}

		public CartesianCoordinate convertToCartesian()
		{
			return new CartesianCoordinate(convertPolarToCartesian(m_coordinates));
		}

		public override void setLocalY(float y)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			t_cartesian.Y = y;
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}
		public override void setLocalX(float x)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			t_cartesian.X = x;
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}

		public override void smoothStep(Vector2 a_vec, float a_amount)
		{
			m_coordinates = convertCartesianToPolar(Vector2.SmoothStep(convertPolarToCartesian(m_coordinates),a_vec,a_amount));
		}

		public override void setSlope(float a_rotation)
		{
			m_coordinates.Y = a_rotation;
		}

		public override void plusYWith(float a_y)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			t_cartesian.Y += a_y;
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}

		public override void plusXWith(float a_x)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			t_cartesian.X += a_x;
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}

		public override void setGlobalY(float a_y)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			if (m_parentPosition == null)
			{
				m_coordinates.Y = a_y;
			}
			else
			{
				m_coordinates.Y = a_y - m_parentPosition.getGlobalY();
			}
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}

		public override void setGlobalX(float a_x)
		{
			Vector2 t_cartesian = convertPolarToCartesian(m_coordinates);
			if (m_parentPosition == null)
			{
				m_coordinates.X = a_x;
			}
			else
			{
				m_coordinates.X = a_x - m_parentPosition.getGlobalX();
			}
			m_coordinates = convertCartesianToPolar(t_cartesian);
		}

		public override void setGlobalCartesianCoordinates(Vector2 a_position)
		{
			if (m_parentPosition == null)
			{
				m_coordinates = convertCartesianToPolar(a_position);
			}
			else
			{
				m_coordinates = convertCartesianToPolar(a_position - m_parentPosition.getGlobalCartesianCoordinates());
			}
		}
	}
}
