using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
	/// <summary>
	/// Gets the type of the direction.
	/// </summary>
	/// <returns>The direction type.</returns>
	/// <param name="diff">Diff.</param>
	public static Direction8Way GetDirectionType(Vector3 diff)
	{
		Direction8Way retType = Direction8Way.se;

		float atanAngleVal = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		if (atanAngleVal >= -22.5f && atanAngleVal <= 22.5f)
		{
			retType = Direction8Way.e;
		}
		else if (atanAngleVal >= 22.5f && atanAngleVal <= 67.5f)
		{
			retType = Direction8Way.ne;
		}
		else if (atanAngleVal >= 67.5f && atanAngleVal <= 112.5f)
		{
			retType = Direction8Way.n;
		}
		else if (atanAngleVal >= 112.5f && atanAngleVal <= 157.5f)
		{
			retType = Direction8Way.nw;
		}
		else if (atanAngleVal >= 157.5f)
		{
			retType = Direction8Way.w;
		}
		else if (atanAngleVal <= -157.5f)
		{
			retType = Direction8Way.w;
		}
		else if (atanAngleVal <= -112.5f && atanAngleVal >= -157.5f)
		{
			retType = Direction8Way.sw;
		}
		else if (atanAngleVal <= -67.5f && atanAngleVal >= -112.5f)
		{
			retType = Direction8Way.s;
		}
		else if (atanAngleVal <= -22.5f && atanAngleVal >= -67.5f)
		{
			retType = Direction8Way.se;
		}

		return retType;
	}


}
