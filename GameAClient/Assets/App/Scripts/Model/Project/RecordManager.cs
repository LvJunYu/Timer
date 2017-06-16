/********************************************************************
** Filename : RecordManager.cs
** Author : quan
** Date : 1/4/2017 8:02 PM
** Summary : RecordManager.cs
***********************************************************************/

using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class RecordManager : ICacheDataManager<Record>
    {
        public readonly static RecordManager Instance = new RecordManager();

        private readonly LRUCache<long, Record> _caches = new LRUCache<long, Record>(ConstDefine.MaxLRUProjectCount);

        public override bool TryGetData(long guid, out Record t)
        {
            if (_caches.TryGetItem(guid, out t))
            {
                return true;
            }
            return false;
        }
//
//        public Record OnSync(Msg_SC_DAT_Record msg, Project project, bool full = true)
//        {
//            Record data = null;
//            if (!_caches.TryGetItem(msg.RecordId, out data))
//            {
//                data = new Record();
//                _caches.Insert(msg.RecordId, data);
//            }
//            data.Set(msg, project, full);
//            return data;
//        }
    }
}