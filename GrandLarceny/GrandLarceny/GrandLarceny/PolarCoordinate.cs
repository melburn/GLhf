using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GL
{
    class PolarCoordinate : Position
    {
        //X är radien Y är vinkeln i form a radianer
        private Vector2 m_coordinates;

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

        public override void setCartesianCoordinates(Vector2 a_position)
        {
            m_coordinates = convertCartesianToPolar(a_position);
        }

        public override void setPolarCoordinates(float a_radius, float a_radians)
        {
            m_coordinates.X = a_radius;
            m_coordinates.Y = a_radians;
        }
    }
}
