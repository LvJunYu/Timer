using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameA
{
    public class SpecialInputField : InputField
    {
        public int SelectionBeginPosition;
        public int SelectionEndPosition;

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            base.OnUpdateSelected(eventData);
            SelectionBeginPosition = selectionAnchorPosition <= selectionFocusPosition
                ? selectionAnchorPosition
                : selectionFocusPosition;
            SelectionEndPosition = selectionAnchorPosition >= selectionFocusPosition
                ? selectionAnchorPosition
                : selectionFocusPosition;
        }

        public void SetDefaultValue()
        {
            SelectionBeginPosition = 0;
            SelectionEndPosition = 0;
        }
    }
}