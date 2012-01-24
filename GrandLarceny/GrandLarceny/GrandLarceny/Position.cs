using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
    abstract class Position
    {
        protected Position m_parentPosition;

        abstract public Vector2 getLocalCartesianCoordinates();
        abstract public Vector2 getGlobalCartesianCoordinates();
        abstract public Vector2 getLocalPolarCoordinates();
        abstract public Vector2 getGlobalPolarCoordinates();
        abstract public void setCartesianCoordinates(Vector2 a_position);
        abstract public void setPolarCoordinates(float a_radius, float a_radians);

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
    }
}
