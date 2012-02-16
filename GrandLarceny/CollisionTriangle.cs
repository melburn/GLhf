using System;

public class CollisionTriangle : CollisionShape
{
	public Vector2 m_point1;
	public Vector2 m_point2;
	public Vector2 m_point3;

	public CollisionTriangle(Vector2 a_point1, Vector2 a_point2, Vector2 a_point3)
	{
		m_poiont1 = a_point1;
		m_point2 = a_point2;
		m_point3 = a_point3;
	}
	public bool Collides(CollisionShape a_cs)
	{
		if (a_cs is CollisionRectangle)
		{
			CollisionRectangle a_cr = (CollisionRectangle) a_cs;
			if (a_cr.contains(m_point1) || contains(new Vector2 (a_cr.m_x, a_cr.my)))
			{
				return true;
			}
		}
		else
		{
			//throw new NotImplementedException("Triangles cannot collide with triangles");
			return false;
		}
	}
	public bool contains(Vector2 a_point)
	{

	}
}
