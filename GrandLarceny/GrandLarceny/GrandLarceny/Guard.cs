using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Guard : NPE
	{
        private int m_leftPatrollPoint;
        private int m_rightPatrollPoint;
        private Boolean m_patrolling;
        private Boolean m_hasFlashLight;

        //flashlight addicted guard always has their flashlight up
        private Boolean m_FlashLightAddicted;

		public Guard(Vector2 a_posV2, String a_sprite, int a_leftPatrollPoint, int a_rightPatrollPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted)
			: base(a_posV2, a_sprite)
		{
            m_leftPatrollPoint = a_leftPatrollPoint;
            m_rightPatrollPoint = a_rightPatrollPoint;
            m_patrolling = true;
            m_hasFlashLight = a_hasFlashLight;
            m_hasFlashLight = a_flashLightAddicted;
		}
        public Guard(Vector2 a_posV2, String a_sprite, Boolean a_hasFlashLight, Boolean a_flashLightAddicted)
			: base(a_posV2, a_sprite)
		{
            m_patrolling = false;
            m_hasFlashLight = a_hasFlashLight;
            m_hasFlashLight = a_flashLightAddicted;
		}
	}
}
