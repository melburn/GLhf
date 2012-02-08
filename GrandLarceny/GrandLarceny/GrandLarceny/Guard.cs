﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Guard : NPE
	{
        private float m_leftPatrollPoint;
        private float m_rightPatrollPoint;
        private Boolean m_hasPatroll;
        private Boolean m_hasFlashLight;
        public Boolean m_inALightArea;
        private Boolean m_isCarryingFlashLight;
        private float MOVEMENTSPEED = 250;

        //flashlight addicted guard always has their flashlight up
        private Boolean m_FlashLightAddicted;

		public Guard(Vector2 a_posV2, String a_sprite, float a_leftPatrollPoint, float a_rightPatrollPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted)
			: base(a_posV2, a_sprite)
		{
            m_leftPatrollPoint = a_leftPatrollPoint;
            m_rightPatrollPoint = a_rightPatrollPoint;
            m_hasPatroll = true;
            m_hasFlashLight = a_hasFlashLight;
            m_hasFlashLight = a_flashLightAddicted;
            m_aiState = AIStatePatrolling.getInstance();
		}
        public Guard(Vector2 a_posV2, String a_sprite, Boolean a_hasFlashLight, Boolean a_flashLightAddicted)
			: base(a_posV2, a_sprite)
		{
            m_hasPatroll = false;
            m_hasFlashLight = a_hasFlashLight;
            m_hasFlashLight = a_flashLightAddicted;
		}
        internal void goRight()
        {
            m_speed.X = MOVEMENTSPEED;
            if(m_isCarryingFlashLight)
            {
                //m_img.setSprite("goRightWithFlashLight");
            }
            else
            {
                //m_img.setSprite("goRight");
            }
        }
        internal void goLeft()
        {
            m_speed.X = -MOVEMENTSPEED;
            if(m_isCarryingFlashLight)
            {
                //m_img.setSprite("goLeftWithFlashLight");
            }
            else
            {
                //m_img.setSprite("goLeft");
            }
        }
        internal void stop()
        {
            m_speed.X = 0;
            if(m_isCarryingFlashLight)
            {
                //m_img.setSprite("idleing");
            }
            else
            {
                //m_img.setSprite("idleing");
            }
        }
        internal void toogleFlashLight()
        {
            if(m_isCarryingFlashLight)
            {
                m_isCarryingFlashLight = false;
                //m_img.setSprite("putthataway");
            }
            else
            {
                m_isCarryingFlashLight = true;
                //m_img.setSprite("getthatup");
            }
            m_speed.X = 0;
        }

        internal float getLeftPatrollPoint()
        {
            return m_leftPatrollPoint;
        }


        internal bool hasPatroll()
        {
            return m_hasPatroll;
        }

        internal float getRightPatrollPoint()
        {
            return m_rightPatrollPoint;
        }
    }
}