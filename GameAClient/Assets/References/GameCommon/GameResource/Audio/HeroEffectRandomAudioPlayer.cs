/********************************************************************
** Filename : HeroEffectRandomAudioPlayer  
** Author : ake
** Date : 5/31/2016 2:42:47 PM
** Summary : HeroEffectRandomAudioPlayer  
***********************************************************************/


using GameA.Game;

namespace SoyEngine
{
	public class HeroEffectRandomAudioPlayer:RandomAudioPlayer
	{
		private int _lastDownId = NoneValue;

        public HeroEffectRandomAudioPlayer (string name) : base (name)
        {
        }

		public void SetCurDownUnitId(int id)
		{
			if (_lastDownId == id)
			{
				return;
			}
		    var tableUnit = UnitManager.Instance.GetTableUnit(id);
//		    SetRandomAudioNameArray(tableUnit == null ? null : tableUnit.WalkAudioNames);
		    _lastDownId = id;
		}
	}
}