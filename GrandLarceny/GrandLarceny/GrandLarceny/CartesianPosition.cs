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

		public CartesianCoordinate(Vector2 a_offset, Position a_parentPosition)
		{
			m_coordinates = a_offset;
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

		public override void setLocalCartesianCoordinates(Vector2 a_position)
		{
			m_coordinates = a_position;
		}

		public override void setLocalPolarCoordinates(float a_radius, float a_radians)
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

		public override float getLocalX()
		{
			return m_coordinates.X;
		}

		public override float getLocalY()
		{
			return m_coordinates.Y;
		}
		public override float getGlobalX()
		{
			if (m_parentPosition == null)
			{
				return m_coordinates.X;
			}
			else
			{
				return m_coordinates.X + m_parentPosition.getGlobalX();
			}
		}
		public override float getGlobalY()
		{
			if (m_parentPosition == null)
			{
				return m_coordinates.Y;
			}
			else
			{
				return m_coordinates.Y + m_parentPosition.getGlobalY();
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
				m_coordinates = getGlobalCartesianCoordinates();
			}
			else
			{
				m_coordinates = getGlobalCartesianCoordinates() - a_parentPosition.getGlobalCartesianCoordinates();
			}
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

		public override void setLocalY(float y)
		{
			m_coordinates.Y = y;
		}

		public override void setLocalX(float x)
		{
			m_coordinates.X = x;
		}

		public override void smoothStep(Vector2 a_vec, float a_amount)
		{
			m_coordinates = Vector2.SmoothStep(m_coordinates, a_vec, a_amount);
		}

		public override void setSlope(float a_rotation)
		{
			Vector2 t_polarCoordinate = convertCartesianToPolar(m_coordinates);
			t_polarCoordinate.Y = a_rotation;
			m_coordinates = convertPolarToCartesian(t_polarCoordinate);
		}

		public override void plusYWith(float a_y)
		{
			m_coordinates.Y += a_y;
		}

		public override void plusXWith(float a_x)
		{
			m_coordinates.X += a_x;
		}

		public override void setGlobalY(float a_y)
		{
			if (m_parentPosition == null)
			{
				m_coordinates.Y = a_y;
			}
			else
			{
				m_coordinates.Y = a_y - m_parentPosition.getGlobalY();
			}
		}

		public override void setGlobalX(float a_x)
		{
			if (m_parentPosition == null)
			{
				m_coordinates.X = a_x;
			}
			else
			{
				m_coordinates.X = a_x - m_parentPosition.getGlobalX();
			}
		}

		public override void setGlobalCartesianCoordinates(Vector2 a_position)
		{
			if (m_parentPosition == null)
			{
				m_coordinates = a_position;
			}
			else
			{
				m_coordinates = a_position - m_parentPosition.getGlobalCartesianCoordinates();
			}
		}

		public override string ToString()
		{
			if (m_parentPosition == null)
			{
				return "(X:" + m_coordinates.X + ",Y:" + m_coordinates.Y + ")";
			}
			else
			{
				return "(X:" + m_coordinates.X + ",Y:" + m_coordinates.Y + ") + P";
			}
		}
	}
}
