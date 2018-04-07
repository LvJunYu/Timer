/********************************************************************
** Filename : MorphUnit
** Author : Dong
** Date : 2016/11/15 星期二 下午 2:02:17
** Summary : MorphUnit
***********************************************************************/

namespace GameA.Game
{
    public class MorphUnit : SpriteUnit
    {
        private byte _morphId;

        public override void OnFree()
        {
            _morphId = 0;
            base.OnFree();
        }

        public override void InitMorphId(byte morphId)
        {
            _morphId = morphId;
            ChangeView();
        }

        public override void OnNeighborDirChanged(ENeighborDir neighborDir, bool add)
        {
            if (add)
            {
                _morphId = (byte)(_morphId | (byte)neighborDir);
            }
            else
            {
                _morphId = (byte)(_morphId & ~(byte)neighborDir);
            }
            ChangeView();
        }

        private void ChangeView()
        {
            var edge = _morphId >> 4;
            Table_Morph tableMorph = TableManager.Instance.GetMorph(edge);
            if (_unit != null && _unit.TableUnit != null)
            {
                var assetPath = string.Format("{0}_{1}_{2}", _unit.TableUnit.Model, tableMorph.Name, UnityEngine.Random.Range(1, tableMorph.Count + 1));
                ChangeView(assetPath);
            }
        }
    }
}
