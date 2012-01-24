﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
    class PolarCoordinate : Position
    {
        //X är radien Y är vinkeln i form a radianer
        private Vector2 m_coordinates;

        public PolarCoordinate(Vector2 a_coordinates)
        {
            m_coordinates = a_coordinates;
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

        public override void setCartesianCoordinates(Vector2 a_position)
        {
            m_coordinates = convertCartesianToPolar(a_position);
        }

        public override void setPolarCoordinates(float a_radius, float a_radians)
        {
            m_coordinates.X = a_radius;
            m_coordinates.Y = a_radians;
        }

        public override void plusWith(Vector2 a_term)
        {
            m_coordinates = convertCartesianToPolar(convertPolarToCartesian(m_coordinates)+a_term);
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
            return m_coordinates.Y;
        }

        public override float getX()
        {
            return (float)(m_coordinates.X * Math.Cos(m_coordinates.Y));
        }

        public override float getY()
        {
            return (float)(m_coordinates.X * Math.Sin(m_coordinates.Y));
        }

        public override void setParentPositionWithoutMoving(Position a_parentPosition)
        {
            //kanske ska vara tvärtom
            m_coordinates = convertCartesianToPolar(getGlobalCartesianCoordinates() - a_parentPosition.getGlobalCartesianCoordinates());

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
    }
}
