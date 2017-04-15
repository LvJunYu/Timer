/********************************************************************
** Filename : MathTools  
** Author : ake
** Date : 5/24/2016 2:09:33 PM
** Summary : MathTools  
***********************************************************************/


using UnityEngine;

namespace SoyEngine
{
	public static class MathTools
	{
		public static bool CompareVector2(Vector2 v1, Vector2 v2)
		{
			Vector2 tmp = v1 - v2;
			return (Mathf.Abs(tmp.x) < 0.01f && Mathf.Abs(tmp.y) < 0.01f);
		}
	}
}