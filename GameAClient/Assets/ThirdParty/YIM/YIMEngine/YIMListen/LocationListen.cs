using System.Collections.Generic;

namespace YIMEngine
{
    public interface LocationListen
    {
        // 获取自己位置回调
        void OnUpdateLocation(YIMEngine.ErrorCode errorcode, YIMEngine.GeographyLocation location);
	    // 获取附近目标回调
        void OnGetNearbyObjects(YIMEngine.ErrorCode errorcode, List<YIMEngine.RelativeLocation> neighbourList, uint startDistance, uint endDistance);
    }

}