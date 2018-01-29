namespace GameA
{
    public partial class Reward
    {
        public void AddToLocal()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].AddToLocal();
            }
        }
    }
}