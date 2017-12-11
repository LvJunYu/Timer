using System;
using System.Collections.Generic;

namespace GameA
{
    public partial class UnitPreinstallList
    {
        private Dictionary<int, List<UnitPreinstall>> _cache = new Dictionary<int, List<UnitPreinstall>>();

        private List<UnitPreinstall> _preinstallsCache;

        public List<UnitPreinstall> PreinstallsCache
        {
            get { return _preinstallsCache; }
        }

        public void CheckLocalOrRequest(int unitId, Action successAction, Action failAction = null)
        {
            _preinstallsCache = null;
            if (_cache.TryGetValue(unitId, out _preinstallsCache))
            {
                if (successAction != null)
                {
                    successAction.Invoke();
                }
            }
            else
            {
                Request(unitId, () =>
                {
                    _preinstallsCache = _preinstallList;
                    _preinstallsCache.Sort((p, q) => p.CreateTime.CompareTo(q.CreateTime));
                    _cache.Add(unitId, _preinstallsCache);
                    if (successAction != null)
                    {
                        successAction.Invoke();
                    }
                }, res =>
                {
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                });
            }
        }
    }
}