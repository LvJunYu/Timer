/********************************************************************
** Filename : UMViewRecordLoading  
** Author : ake
** Date : 12/27/2016 3:38:43 PM
** Summary : UMViewRecordLoading  
***********************************************************************/


using System;
using System.Diagnostics;
using SoyEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GameA
{
	public class UMViewRecordLoading: UMViewBase
	{
		public Text ProcessLabel;

		public const string ShowProcessFormat = "{0}%";


		private float _process = -1;

		public void SetProcess(float value)
		{
			if (Math.Abs(value - _process) < 0.001)
			{
				return;
			}
			_process = value;
			float showValue = ((int)(value * 1000)) / 10f;
			
			ProcessLabel.text = string.Format(ShowProcessFormat, showValue);
		}
	}
}