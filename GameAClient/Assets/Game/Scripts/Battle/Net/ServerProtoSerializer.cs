using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SoyEngine
{
    /// <summary>
    /// 序列化工具
    /// 借用枚举 既有字符串信息 又有数值value的特性 建立value和字符同名协议类之间的映射
    /// </summary>
    public class ServerProtoSerializer:ProtoSerializer
    {
        public ServerProtoSerializer(Type enumType, string nameSpace, params Assembly[] assemblyAry)
        {
            HashSet<Assembly> set = new HashSet<Assembly>(assemblyAry);
            Type protoType = typeof(global::ProtoBuf.ProtoContractAttribute);
            Dictionary<string, Type> mapping = new Dictionary<string, Type>(65535);
            _enumType = enumType;
            foreach (var assembly in set)
            {
                Type[] typeAry = assembly.GetTypes();
                foreach (var item in typeAry)
                {
                    object[] result = item.GetCustomAttributes(protoType, false);
                    if (result.Length != 0)
                    {
                        if (mapping.ContainsKey(item.FullName))
                        {
                            LogHelper.Error("Proto Msg Name duplication, Name: {0}", item.FullName);
                        }
                        else
                        {
                            mapping.Add(item.FullName, item);
                        }
                    }
                }
            }
            
            var enumAry = Enum.GetValues(enumType);

            for (var i = 0; i < enumAry.Length; i++)
            {
                object emt = enumAry.GetValue(i);
                Type t = null;
                string name = Enum.GetName(enumType, emt);
                string fullName = nameSpace + "." + name;
                if (!string.IsNullOrEmpty(fullName) && mapping.TryGetValue(fullName, out t))
                {
                    _msgType2PacketMapping.Add((ushort)(int)emt, t);
                    _packet2MsgTypeMapping.Add(t, (ushort)(int)emt);
                }
            }
            set.Clear();
            mapping.Clear();
        }

        //public override void Serialize(object msg, Stream stream)
        //{
        //    try
        //    {
        //        ProtoBuf.Serializer.NonGeneric.Serialize(stream, msg);
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", msg.GetType().FullName, e.ToString());
        //    }
        //}


        //public override byte[] Serialize(object msg)
        //{
        //    using (PooledFixedByteBufHolder holder = PoolFactory<PooledFixedByteBufHolder>.Get())
        //    {
        //        Serialize(msg, holder.Content);
        //        return holder.Content.ReadableBytesToArray();
        //    }
        //}

        //public override object Deserialize(Stream stream, uint emt)
        //{
        //    Type t = null;
        //    if (_msgType2PacketMapping.TryGetValue(emt, out t))
        //    {
        //        try
        //        {
        //            return ProtoBuf.Serializer.NonGeneric.Deserialize(t, stream);
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", t.FullName, e.ToString());
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public override object Deserialize(Stream stream, Type type)
        //{
        //    try
        //    {
        //        return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", type.FullName, e.ToString());
        //        return null;
        //    }
        //}

        //public override object Deserialize(ByteBuf stream, uint emt, int len)
        //{
        //    Type t = null;
        //    if (!_msgType2PacketMapping.TryGetValue(emt, out t))
        //    {
        //        LogHelper.Error("Deserialize Type Not Found, TypeInx: {0}", emt);
        //        return null;
        //    }
        //    if (len > stream.ReadableBytes())
        //    {
        //        LogHelper.Error("Deserialize error, data Not enough, Type: {0}", t.FullName);
        //        return null;
        //    }
        //    using (PooledEmptyByteBufHolder holder = PoolFactory<PooledEmptyByteBufHolder>.Get())
        //    {
        //        try
        //        {
        //            holder.Content.SetBufForRead(stream.Buf);
        //            holder.Content.ReaderIndex = stream.ReaderIndex;
        //            holder.Content.WriterIndex = stream.ReaderIndex + len;
        //            stream.IgnoreReadBytes(len);
        //            return ProtoBuf.Serializer.NonGeneric.Deserialize(t, holder.Content);
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", t.FullName, e.ToString());
        //            return null;
        //        }
        //    }
        //}

        //public override object Deserialize(byte[] bytes, Type type)
        //{
        //    using (PooledEmptyByteBufHolder holder = PoolFactory<PooledEmptyByteBufHolder>.Get())
        //    {
        //        holder.Content.SetBufForRead(bytes);
        //        try
        //        {
        //            return ProtoBuf.Serializer.NonGeneric.Deserialize(type, holder.Content);
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", type.FullName, e.ToString());
        //        }
        //        return null;
        //    }
        //}

        //public override T Deserialize<T>(byte[] bytes)
        //{
        //    using (PooledEmptyByteBufHolder holder = PoolFactory<PooledEmptyByteBufHolder>.Get())
        //    {
        //        holder.Content.SetBufForRead(bytes);
        //        T t = default(T);
        //        try
        //        {
        //            t = ProtoBuf.Serializer.Deserialize<T>(holder.Content);
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.Error("ProtoSerializer Error, Type: {0}, Exception: {1}", t.GetType().FullName, e.ToString());
        //        }
        //        return t;
        //    }
        //}
        public override void Serialize(object msg, Stream stream)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(Stream stream, uint emt)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(Stream stream, Type type)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(ByteBuf readBuffer, uint msgType, int size)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(byte[] bytes, Type type)
        {
            throw new NotImplementedException();
        }

        public override T Deserialize<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(object msg)
        {
            throw new NotImplementedException();
        }
    }
}