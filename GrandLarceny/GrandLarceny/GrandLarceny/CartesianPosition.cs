using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GL
{
    class CartesianPosition : Position
    {
        private Vector2 m_coordinates;

        public CartesianPosition(Vector2 a_coordinates)
        {
            m_coordinates = a_coordinates;
        }

        public CartesianPosition(Vector2 a_coordinates, Position a_parentPosition)
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
    }
}
