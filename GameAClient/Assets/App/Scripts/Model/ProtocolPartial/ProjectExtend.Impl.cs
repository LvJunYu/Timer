namespace GameA
{
    public partial class ProjectExtend
    {
        private float _score;

        public float Score
        {
            get { return _score; }
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (_likeCount + _unlikeCount == 0)
            {
                _score = 5;
            }
            else
            {
                _score = _likeCount * 10 / (float) (_likeCount + _unlikeCount);
            }
        }
    }
}