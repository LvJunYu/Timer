namespace SoyEngine
{
    public class CSProtoSerializer : ClientProtoSerializer
    {
        public static readonly CSProtoSerializer Instance = new CSProtoSerializer();

        public CSProtoSerializer() : base(typeof (ECSMsgType), ProtoNameSpace, new GeneratedClientSerializer())
        {
        }
    }
}