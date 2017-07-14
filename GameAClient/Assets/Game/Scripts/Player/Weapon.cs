namespace GameA.Game
{
    public class Weapon
    {
        protected Table_Equipment _tableEquipment;

        public bool ChangeWeapon(int id)
        {
            if (id == 0)
            {
                //扔掉装备
                return true;
            }
            _tableEquipment = TableManager.Instance.GetEquipment(id);
            if (_tableEquipment == null)
            {
                return false;
            }
            return true;
        }
    }
}