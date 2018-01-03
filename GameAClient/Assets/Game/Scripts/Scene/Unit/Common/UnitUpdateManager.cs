/********************************************************************
** Filename : UnitUpdateManager
** Author : Dong
** Date : 2016/10/20 星期四 下午 10:14:44
** Summary : UnitUpdateManager
***********************************************************************/

namespace GameA.Game
{
    public class UnitUpdateManager
    {
        public void UpdateLogic(float deltaTime)
        {
            var playerList = PlayerManager.Instance.PlayerList;
            var allSwitchUnits = ColliderScene2D.CurScene.AllSwitchUnits;
            var allMagicUnits = ColliderScene2D.CurScene.AllMagicUnits;
            var allBulletUnits = ColliderScene2D.CurScene.AllBulletUnits;
            var allOtherUnits = ColliderScene2D.CurScene.AllOtherUnits;

            for (int i = 0; i < allSwitchUnits.Count; i++)
            {
                if (allSwitchUnits[i].IsInterest)
                {
                    allSwitchUnits[i].UpdateLogic();
                }
            }
            for (int i = 0; i < allMagicUnits.Count; i++)
            {
                if (allMagicUnits[i].IsInterest)
                {
                    allMagicUnits[i].UpdateLogic();
                }
            }

            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                if (allOtherUnits[i].IsInterest)
                {
                    allOtherUnits[i].CheckStart();
                }
            }
            //人先执行 AI怪物后执行
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] != null)
                {
                    playerList[i].UpdateLogic();
                }
            }
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                if (allOtherUnits[i].IsPlayer)
                {
                    continue;
                }
                if (allOtherUnits[i].IsInterest)
                {
                    allOtherUnits[i].UpdateLogic();
                }
            }
            for (int i = 0; i < allBulletUnits.Count; i++)
            {
                if (allBulletUnits[i].IsInterest)
                {
                    allBulletUnits[i].UpdateLogic();
                }
            }

            for (int i = 0; i < allMagicUnits.Count; i++)
            {
                if (allMagicUnits[i].IsInterest)
                {
                    allMagicUnits[i].UpdateView(deltaTime);
                }
            }
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                if (allOtherUnits[i].IsInterest)
                {
                    allOtherUnits[i].CalculateExtraDeltaPos();
                }
            }
            //拉箱子的先移动，推箱子的最后
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] != null && playerList[i].GetBoxOperateType() == EBoxOperateType.Pull)
                {
                    playerList[i].UpdateView(deltaTime);
                }
            }
            for (int i = 0; i < allOtherUnits.Count; i++)
            {
                if (allOtherUnits[i].IsPlayer || allOtherUnits[i].Id == UnitDefine.RopeJointId)
                {
                    continue;
                }
                if (allOtherUnits[i].IsInterest)
                {
                    allOtherUnits[i].UpdateView(deltaTime);
                }
            }
            //控制绳子每个节点的逻辑顺序
            RopeManager.Instance.UpdateView(deltaTime);
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] != null && playerList[i].GetBoxOperateType() != EBoxOperateType.Pull)
                {
                    playerList[i].UpdateView(deltaTime);
                }
            }

            for (int i = 0; i < allBulletUnits.Count; i++)
            {
                if (allBulletUnits[i].IsInterest)
                {
                    allBulletUnits[i].UpdateView(deltaTime);
                }
            }
        }
    }
}