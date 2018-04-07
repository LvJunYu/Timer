namespace GameA.Game
{
    public abstract class ComponentBase
    {
        private static int _value;
        protected int _componentId;
        protected EntityBase _entity;

        public EntityBase Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        public int ComponentId
        {
            get { return _componentId; }
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnGet()
        {
            _componentId = GenerateId();
        }

        public virtual void OnFree()
        {
        }

        public virtual void OnDestroy()
        {
        }

        private int GenerateId()
        {
            return ++_value;
        }
    }
}