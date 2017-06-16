/********************************************************************
** Filename : UICtrlGameItemAddMessage  
** Author : ake
** Date : 9/23/2016 3:20:38 PM
** Summary : UICtrlGameItemAddMessage  
***********************************************************************/


using System;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	public class UICtrlGameItemAddMessage: UICtrlInGameBase<UIViewGameItemAddMessage>
	{
		private UnitDesc _selectItem;
		private string _pristineValue;
		public const int MessageStringCountMax = 45;

		protected override void InitGroupId()
		{
			_groupId = (int)EUIGroupType.InGamePopup;
		}

		public override void Open(object parameter)
		{
			base.Open(parameter);
			if (parameter == null)
			{
				LogHelper.Error("UICtrlGameItemAddMessage.Open called ,but parameter is null!");
				return;
			}
			_selectItem = (UnitDesc) parameter;
			InspectSelectItem();
			UpdateShow();
		}


		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitUI();
		}

		#region private

		private void UpdateShow()
		{
		    _pristineValue = DataScene2D.Instance.GetUnitExtra(_selectItem.Guid).Msg;
			_cachedView.AddMessageContentValue.text = _pristineValue==null?"":_pristineValue;
		}

		private void InspectSelectItem()
		{
			if (_selectItem.Id == 0)
			{
				LogHelper.Error("cur _selectItem is invalid id is 0.");
				return;
			}
			Table_Unit item = UnitManager.Instance.GetTableUnit(_selectItem.Id);
			if (item == null)
			{
				LogHelper.Error("_selectItem.id is invalid! {0}", _selectItem.Id);
				return;
			}
			if (item.Id==7001)
			{
				LogHelper.Error("_selectItem {0} .EScriptType is not BulletinBoard!", _selectItem.Id);
				return;
			}
		}

		private void InitUI()
		{
			_cachedView.Title.text = GM2DUIConstDefine.GameBulletinBoardMessageEditorTitle;
			_cachedView.AddMessageContentValue.characterLimit = MessageStringCountMax;

			_cachedView.ButtonEnsure.onClick.AddListener(OnClickEnsureButton);
			_cachedView.ButtonCancle.onClick.AddListener(OnClickCancleButton);
		}

	    private void SetData(string value)
	    {
	        EditMode.Instance.MapStatistics.NeedSave = true;
	        UnitExtra unitExtra = DataScene2D.Instance.GetUnitExtra(_selectItem.Guid);
	        unitExtra.Msg = value;
	        DataScene2D.Instance.ProcessUnitExtra(_selectItem.Guid, unitExtra);
	    }

		#endregion

		#region uievent


		private void OnClickEnsureButton()
		{
			string value = _cachedView.AddMessageContentValue.text;
			if (String.CompareOrdinal(_pristineValue, value) != 0)
			{
				SetData(value);
			}

            SocialGUIManager.Instance.CloseUI<UICtrlGameItemAddMessage>();
		}

		private void OnClickCancleButton()
		{
            SocialGUIManager.Instance.CloseUI<UICtrlGameItemAddMessage>();
		}

		#endregion

	}
}