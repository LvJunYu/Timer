using System;
using System.Collections.Generic;
using System.Text;
using GameA;

namespace SoyEngine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DictionaryObjectBase
    {
        protected static readonly Dictionary<Type, Dictionary<int, FieldDefineData>> DefineDict =
            new Dictionary<Type, Dictionary<int, FieldDefineData>>();

        protected class FieldDefineData
        {
            public String FieldName;
            public EType EType;
            public Type FieldType;
            public int Dimension;
            public Type ChildType;
        }

        protected enum EType
        {
            Atomic,
            Dictionary,
            List,
        }
        
        protected Dictionary<int, object> _dict;

//        protected static bool IsGenericList(object o)
//        {
//            var oType = o.GetType();
//            return oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>);
//        }

        public abstract void Clear();

        public abstract bool IsEmpty { get; }
        public abstract int Count { get; }

        public abstract TK Get<TK>(params int[] keySeq);

        protected internal abstract TK InternalGet<TK>(int startInx, params int[] keySeq);

        public abstract bool Has(params int[] keySeq);

        protected internal abstract bool InternalHas(int startInx, params int[] keySeq);

        public abstract void Set<TK>(TK val, params int[] keySeq);

        protected internal abstract void InternalSet<TK>(TK val, int startInx, params int[] keySeq);

        public abstract void ForceSet<TK>(TK val, params int[] keySeq);

        protected internal abstract void InternalForceSet<TK>(TK val, int startInx, params int[] keySeq);

        public abstract void Remove(params int[] keySeq);

        protected internal abstract void InternalRemove(int startInx, params int[] keySeq);

        protected internal abstract DictionaryObjectBase InternalClone();

        protected internal abstract DictionaryObjectBase InternalDiff(DictionaryObjectBase dictionaryObjectBase);
        protected internal abstract void InternalPatch(DictionaryObjectBase dictionaryObjectBase);

        protected internal virtual void ToString(int level, StringBuilder stringBuilder)
        {
        }

        protected static bool IsAtomic(object o)
        {
            if (o == null)
            {
                return false;
            }

            return IsAtomic(o.GetType());
        }

        protected static bool IsAtomic(Type t)
        {
            return t == typeof(string)
                   || t.IsSubclassOf(typeof(ValueType));
        }

        protected static bool IsDefaultValue<TK>(TK v)
        {
            if (v == null)
            {
                return true;
            }
            return v.Equals(default(TK));
        }

        protected static bool IsDefaultValue(Type t, object v)
        {
            if (v == null)
            {
                return true;
            }

            if (t == typeof(string))
            {
                return false;
            }
            
            if (t == typeof(bool))
            {
                return default(bool).Equals(v);
            }

            if (t == typeof(byte))
            {
                return default(byte).Equals(v);
            }

            if (t == typeof(sbyte))
            {
                return default(sbyte).Equals(v);
            }

            if (t == typeof(ushort))
            {
                return default(ushort).Equals(v);
            }

            if (t == typeof(short))
            {
                return default(short).Equals(v);
            }

            if (t == typeof(uint))
            {
                return default(uint).Equals(v);
            }

            if (t == typeof(int))
            {
                return default(int).Equals(v);
            }

            if (t == typeof(ulong))
            {
                return default(ulong).Equals(v);
            }

            if (t == typeof(long))
            {
                return default(long).Equals(v);
            }

            if (t == typeof(float))
            {
                return default(float).Equals(v);
            }

            if (t == typeof(double))
            {
                return default(double).Equals(v);
            }

            return v.Equals(Activator.CreateInstance(t));
        }
    }

    public class DictionaryObject<TExtend> : DictionaryObjectBase where TExtend : DictionaryObject<TExtend>, new()
    {
        protected static readonly Dictionary<int, String> FieldNameDict =
            new Dictionary<int, String>();

        public override void Clear()
        {
            if (_dict == null)
            {
                return;
            }
            _dict.Clear();
        }

        public override bool IsEmpty
        {
            get
            {
                return _dict == null || _dict.Count == 0;
            }
        }

        public override int Count
        {
            get { return _dict == null ? 0 : _dict.Count; }
        }

        public override TK Get<TK>(params int[] keySeq)
        {
            return InternalGet<TK>(0, keySeq);
        }

        protected internal override TK InternalGet<TK>(int startInx, params int[] keySeq)
        {
            if (startInx >= keySeq.Length)
            {
                throw new ArgumentException();
            }
            
            if (_dict == null)
            {
                _dict = new Dictionary<int, object>();
            }

            bool needReturnRet = startInx == keySeq.Length - 1;
            var curKey = keySeq[startInx];
            bool needAdd = false;
            object fieldVal;
            if (!_dict.TryGetValue(curKey, out fieldVal))
            {
                fieldVal = GetFieldDefaultValue(curKey);
                needAdd = true;
            }
            
            if (fieldVal == null)
            {
                if (needReturnRet)
                {
                    return default(TK);
                }

                throw new ArgumentException();
            }

            if (IsAtomic(fieldVal.GetType()))
            {
                if (needReturnRet)
                {
                    return (TK) fieldVal;
                }
                
                throw new ArgumentException();
            }

            if (needAdd)
            {
                if (!(this is DictionaryListObject))
                {
                    _dict.Add(curKey, fieldVal);
                }
            }

            if (needReturnRet)
            {
                return (TK) fieldVal;
            }

            return ((DictionaryObjectBase) fieldVal).InternalGet<TK>(startInx + 1, keySeq);
        }

        public override bool Has(params int[] keySeq)
        {
            return InternalHas(0, keySeq);
        }

        protected internal override bool InternalHas(int startInx, params int[] keySeq)
        {
            if (startInx >= keySeq.Length)
            {
                throw new ArgumentException();
            }

            if (_dict == null)
            {
                return false;
            }

            var curKey = keySeq[startInx];
            if (startInx == keySeq.Length - 1)
            {
                return _dict.ContainsKey(curKey);
            }

            object o;
            if (!_dict.TryGetValue(curKey, out o))
            {
                return false;
            }

            var dictionaryObject = o as DictionaryObjectBase;
            if (dictionaryObject != null)
            {
                return dictionaryObject.InternalHas(startInx + 1, keySeq);
            }

            throw new ArgumentException();
        }

        public override void Set<TK>(TK val, params int[] keySeq)
        {
            InternalSet(val, 0, keySeq);
        }

        protected internal override void InternalSet<TK>(TK val, int startInx, params int[] keySeq)
        {
            if (startInx >= keySeq.Length)
            {
                throw new ArgumentException();
            }

            if (CheckIsDefault(val))
            {
                InternalRemove(startInx, keySeq);
                return;
            }

            if (_dict == null)
            {
                _dict = new Dictionary<int, object>();
            }

            var key = keySeq[startInx];
            if (startInx == keySeq.Length - 1)
            {
                _dict[key] = val;
                return;
            }

            DictionaryObjectBase dictionaryObjectBase;
            object o;
            if (_dict.TryGetValue(key, out o))
            {
                dictionaryObjectBase = o as DictionaryObjectBase;
                if (dictionaryObjectBase == null)
                {
                    throw new ArgumentException();
                }

                dictionaryObjectBase.InternalSet(val, startInx + 1, keySeq);
                return;
            }

            dictionaryObjectBase = GetNewFieldObject(key);
            _dict[key] = dictionaryObjectBase;
            dictionaryObjectBase.InternalSet(val, startInx + 1, keySeq);
        }

        public override void ForceSet<TK>(TK val, params int[] keySeq)
        {
            InternalForceSet(val, 0, keySeq);
        }

        protected internal override void InternalForceSet<TK>(TK val, int startInx, params int[] keySeq)
        {
            if (startInx >= keySeq.Length)
            {
                throw new ArgumentException();
            }

            if (_dict == null)
            {
                _dict = new Dictionary<int, object>();
            }

            var key = keySeq[startInx];
            if (startInx == keySeq.Length - 1)
            {
                _dict[key] = val;
                return;
            }

            DictionaryObjectBase dictionaryObjectBase;
            object o;
            if (_dict.TryGetValue(key, out o))
            {
                dictionaryObjectBase = o as DictionaryObjectBase;
                if (dictionaryObjectBase == null)
                {
                    throw new ArgumentException();
                }

                dictionaryObjectBase.InternalForceSet(val, startInx + 1, keySeq);
                return;
            }

            dictionaryObjectBase = GetNewFieldObject(key);
            _dict[key] = dictionaryObjectBase;
            dictionaryObjectBase.InternalForceSet(val, startInx + 1, keySeq);
        }

        public override void Remove(params int[] keySeq)
        {
            InternalRemove(0, keySeq);
        }

        protected internal override void InternalRemove(int startInx, params int[] keySeq)
        {
            if (startInx >= keySeq.Length)
            {
                throw new ArgumentException();
            }

            var key = keySeq[startInx];
            if (_dict != null)
            {
                if (startInx == keySeq.Length - 1)
                {
                    if (_dict.ContainsKey(key))
                    {
                        _dict.Remove(key);
                    }
                }
                else
                {
                    object obj;
                    if (_dict.TryGetValue(key, out obj))
                    {
                        var dictionaryObject = obj as DictionaryObjectBase;
                        if (dictionaryObject == null)
                        {
                            throw new ArgumentException();
                        }

                        dictionaryObject.InternalRemove(startInx + 1, keySeq);
                    }
                }
            }
        }

        protected internal override DictionaryObjectBase InternalClone()
        {
            TExtend o = new TExtend();
            if (_dict == null)
            {
                return o;
            }

            o._dict = new Dictionary<int, object>(_dict.Count);
            foreach (var entry in _dict)
            {
                if (entry.Value == null)
                {
                    o._dict.Add(entry.Key, null);
                }
                else if (entry.Value is DictionaryObjectBase)
                {
                    o._dict.Add(entry.Key, ((DictionaryObjectBase) entry.Value).InternalClone());
                }
                else
                {
                    o._dict.Add(entry.Key, entry.Value);
                }
            }

            return o;
        }

        public TExtend Clone()
        {
            return (TExtend) InternalClone();
        }

        /// <summary>
        /// other.patch(result) = me
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TExtend Diff(TExtend other)
        {
            return (TExtend) InternalDiff(other);
        }

        public void Patch(TExtend patch)
        {
            InternalPatch(patch);
        }

        protected internal override DictionaryObjectBase InternalDiff(DictionaryObjectBase otherObj)
        {
            TExtend diffObj = Clone();
            TExtend other = otherObj as TExtend;
            if (other == null || other.IsEmpty)
            {
                return diffObj;
            }

            foreach (var entry in other._dict)
            {
                if (entry.Value == null)
                {
                    //没值 保持原状
                    continue;
                }

                var dictionaryObjectBase = entry.Value as DictionaryObjectBase;
                if (dictionaryObjectBase != null)
                {
                    if (diffObj.Has(entry.Key))
                    {
                        var subDiff = diffObj.Get<DictionaryObjectBase>(entry.Key).InternalDiff(dictionaryObjectBase);
                        if (!subDiff.IsEmpty)
                        {
                            //子节点不相同 使用子节点diff
                            diffObj.ForceSet(subDiff, entry.Key);
                        }
                        else
                        {
                            //子节点完全相同 diff移除这一项
                            diffObj.Remove(entry.Key);
                        }
                    }
                    else
                    {
                        //我没有这个值 添加删除标记
                        diffObj.ForceSet<object>(null, entry.Key);
                    }
                }
                else
                {
                    //值类型
                    if (diffObj.Has(entry.Key))
                    {
                        if (diffObj.Get<object>(entry.Key) == entry.Value)
                        {
                            //相等 diff移除
                            diffObj.Remove(entry.Key);
                        }

                        //不相等 使用我的值
                    }
                    else
                    {
                        //我没有这个值 添加删除标记
                        diffObj.ForceSet<object>(null, entry.Key);
                    }
                }
            }

            return diffObj;
        }

        protected internal override void InternalPatch(DictionaryObjectBase patchObj)
        {
            TExtend patch = patchObj as TExtend;
            if (patch == null || patch.IsEmpty)
            {
                return;
            }

            foreach (var entry in patch._dict)
            {
                if (entry.Value == null)
                {
                    //删除标记
                    Remove(entry.Key);
                }

                var dictionaryObjectBase = entry.Value as DictionaryObjectBase;
                if (dictionaryObjectBase != null)
                {
                    DictionaryObjectBase myDictionaryObjectBase = Get<DictionaryObjectBase>(entry.Key);
                    if (myDictionaryObjectBase == null)
                    {
                        myDictionaryObjectBase = GetNewFieldObject(entry.Key);
                        ForceSet(myDictionaryObjectBase, entry.Key);
                    }

                    myDictionaryObjectBase.InternalPatch((DictionaryObjectBase) entry.Value);
                    if (myDictionaryObjectBase.IsEmpty)
                    {
                        Remove(entry.Key);
                    }
                }
                else
                {
                    //值类型
                    Set(entry.Value, entry.Key);
                }
            }
        }

        protected virtual DictionaryObjectBase GetNewFieldObject(int fieldTag)
        {
            var fieldDefine = GetDefine(GetType(), fieldTag);
            if (fieldDefine.EType == EType.Atomic)
            {
                throw new ArgumentException();
            }

            if (fieldDefine.EType == EType.List)
            {
                return new DictionaryListObject(fieldDefine.ChildType, fieldDefine.Dimension - 1);
            }

            return (DictionaryObjectBase) Activator.CreateInstance(fieldDefine.FieldType);
        }
        
        protected virtual object GetFieldDefaultValue(int fieldTag)
        {
            var fieldDefine = GetDefine(GetType(), fieldTag);
            if (fieldDefine.EType == EType.List)
            {
                return new DictionaryListObject(fieldDefine.ChildType, fieldDefine.Dimension - 1);
            }

            if (fieldDefine.FieldType == typeof(string))
            {
                return null;
            }

            return Activator.CreateInstance(fieldDefine.FieldType);
        }

        public override string ToString()
        {
            string str;
            using (var sbHolder = PoolFactory<PooledStringBuilderHolder>.Get())
            {
                ToString(0, sbHolder.S);
                str = sbHolder.ToString();
            }

            return str;
        }

        protected internal override void ToString(int level, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("{");
            InternalToString(level + 1, stringBuilder);
            StringBuildTab(level, stringBuilder);
            stringBuilder.Append("}");
        }

        protected void InternalToString(int level, StringBuilder stringBuilder)
        {
            if (_dict == null)
            {
                StringBuildTab(level, stringBuilder);
                stringBuilder.AppendLine();
                return;
            }
            foreach (var entry in _dict)
            {
                StringBuildTab(level, stringBuilder);
                stringBuilder.Append(GetFieldName(entry.Key));
                stringBuilder.Append(": ");
                if (entry.Value == null)
                {
                    stringBuilder.Append("null");
                }
                else if (entry.Value is string)
                {
                    stringBuilder.Append("\"");
                    stringBuilder.Append((string) entry.Value);
                    stringBuilder.Append("\"");
                }
                else if (entry.Value is DictionaryObjectBase)
                {
                    ((DictionaryObjectBase) entry.Value).ToString(level, stringBuilder);
                }
                else
                {
                    stringBuilder.Append(entry.Value);
                }

                stringBuilder.AppendLine(",");
            }
        }

        protected virtual string GetFieldName(int fieldTag)
        {
            return FieldNameDict[fieldTag];
        }

        protected void StringBuildTab(int count, StringBuilder stringBuilder)
        {
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append("\t");
            }
        }

        protected static bool CheckIsDefault<TK>(TK val)
        {
            if (val == null)
            {
                return true;
            }

            Type type = val.GetType();
            var isAtomic = IsAtomic(type);
            if (isAtomic)
            {
                if (typeof(TK) != typeof(object))
                {
                    if (IsDefaultValue(val))
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsDefaultValue(type, val))
                    {
                        return true;
                    }
                }
            }
            else
            {
                DictionaryObjectBase dictionaryObjectBase = val as DictionaryObjectBase;
                if (dictionaryObjectBase == null)
                {
                    throw new ArgumentException("Type not support, Only Support DictionaryObject and Value Type");
                }

                if (dictionaryObjectBase.IsEmpty)
                {
                    return false;
                }
            }

            return false;
        }

        protected static void DefineField<T>(int fieldTag, string fieldName = null)
        {
            Type type = typeof(T);
            if (IsAtomic(type))
            {
                DefineField(fieldTag, fieldName, EType.Atomic, type);
            }
            else if (type.IsSubclassOf(typeof(DictionaryObjectBase)))
            {
                DefineField(fieldTag, fieldName, EType.Dictionary, type);
            }
            else
            {
                throw new ArgumentException("Type not support, Only Support DictionaryObject and ValueType");
            }
        }

        protected static void DefineFieldList<T>(int fieldTag, string fieldName = null, int dimension = 1)
        {
            if (dimension <= 0 || dimension > 3)
            {
                throw new ArgumentException();
            }
            Type type = typeof(T);
            if (IsAtomic(type) || type.IsSubclassOf(typeof(DictionaryObjectBase)))
            {
                DefineField(fieldTag, fieldName, EType.List, null, typeof(T), dimension);
            }
            else
            {
                throw new ArgumentException("Type not support, Only Support DictionaryObject and ValueType");
            }

        }

        private static void DefineField(int fieldTag, string fieldName, EType eType, Type fieldType,
            Type childType = null,
            int dimension = 1)
        {
            Dictionary<int, FieldDefineData> dict;
            var key = typeof(TExtend);
            if (!DefineDict.TryGetValue(key, out dict))
            {
                dict = new Dictionary<int, FieldDefineData>();
                DefineDict.Add(key, dict);
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = "" + fieldTag;
            }

            FieldDefineData fieldDefineData;
            if (dict.TryGetValue(fieldTag, out fieldDefineData))
            {
                LogHelper.Error("FieldDefine duplication, Type: {0}, FieldName: {1}, NewFieldName: {2}", key.Name,
                    fieldDefineData.FieldName, fieldName);
                throw new Exception();
            }

            dict.Add(fieldTag, new FieldDefineData()
            {
                FieldName = fieldName,
                EType = eType,
                FieldType = fieldType,
                ChildType = childType,
                Dimension = dimension
            });
            FieldNameDict.Add(fieldTag, fieldName);
        }

        protected static FieldDefineData GetDefine(Type type, int fieldTag)
        {
            Dictionary<int, FieldDefineData> dict;
            if (!DefineDict.TryGetValue(type, out dict))
            {
                throw new ArgumentException("Type not defined, Please define field in class static block, Type: " +
                                            type.FullName);
            }

            FieldDefineData fieldDefineData;
            if (!dict.TryGetValue(fieldTag, out fieldDefineData))
            {
                throw new ArgumentException("FieldTag not defined, Please define field in class static block, Type: " +
                                            type.FullName + ", TagField: " + fieldTag);
            }

            return fieldDefineData;
        }
    }

    public class DictionaryListObject : DictionaryObject<DictionaryListObject>
    {
        private int _leftDimension;
        private Type _childType;

        public DictionaryListObject()
        {
        }

        public override int Count
        {
            get
            {
                if (_dict == null || _dict.Count == 0)
                {
                    return 0;
                }

                int maxInx = 0;
                foreach (var entry in _dict)
                {
                    if (entry.Key > maxInx)
                    {
                        maxInx = entry.Key;
                    }
                }
                return maxInx+1;
            }
        }

        public DictionaryListObject(Type childType, int leftDimension)
        {
            _childType = childType;
            _leftDimension = leftDimension;
        }

        public void Add<TK>(TK val)
        {
            Set(val, Count);
        }

        public void RemoveAt(int inx)
        {
            if (IsEmpty)
            {
                return;
            }
            List<int> idList = new List<int>(_dict.Count);
            foreach (var entry in _dict)
            {
                if (entry.Key > inx)
                {
                    idList.Add(entry.Key);
                }
            }

            if (_dict.ContainsKey(inx))
            {
                _dict.Remove(inx);
            }
            idList.Sort();
            for (int i = 0; i < idList.Count; i++)
            {
                var id = idList[i];
                var obj = _dict[id];
                _dict.Remove(id);
                _dict.Add(id-1, obj);
            }
        }

        public List<T> ToList<T>()
        {
            List<T> list = new List<T>(Count);
            if (Count == 0)
            {
                return list;
            }
            for (int i = 0, len = list.Capacity; i < len; i++)
            {
                list.Add(default(T));
            }

            foreach (var entry in _dict)
            {
                list[entry.Key] = (T) entry.Value;
            }

            return list;
        }

        protected internal override DictionaryObjectBase InternalClone()
        {
            var obj = base.InternalClone();
            DictionaryListObject dictionaryListObject = obj as DictionaryListObject;
            if (dictionaryListObject == null)
            {
                throw new Exception();
            }

            dictionaryListObject._childType = _childType;
            dictionaryListObject._leftDimension = _leftDimension;
            return obj;
        }

        protected internal override void ToString(int level, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("[");
            InternalToString(level + 1, stringBuilder);
            StringBuildTab(level, stringBuilder);
            stringBuilder.Append("]");
        }

        protected override string GetFieldName(int fieldTag)
        {
            return "" + fieldTag;
        }

        protected override DictionaryObjectBase GetNewFieldObject(int fieldTag)
        {
            if (_leftDimension == 0)
            {
                if (IsAtomic(_childType))
                {
                    throw new ArgumentException();
                }

                return (DictionaryObjectBase) Activator.CreateInstance(_childType);
            }

            return new DictionaryListObject(_childType, _leftDimension - 1);
        }

        protected override object GetFieldDefaultValue(int fieldTag)
        {
            if (_leftDimension == 0)
            {
                if (IsAtomic(_childType))
                {
                    return Activator.CreateInstance(_childType);
                }

                return null;
            }

            return new DictionaryListObject(_childType, _leftDimension - 1);
        }
    }
}
/*
private void Start()
{
    MyClass myClass = new MyClass();
    myClass.Set(1, MyClass.Field1);
    myClass.Set((byte) 2, MyClass.Field2);
    myClass.Set("lalala", MyClass.Field3);
    myClass.Set(4, MyClass.Field4, MyClass2.Field1);
    myClass.Set(5, MyClass.Field5, 10, MyClass2.Field1);
    var myClass2 = myClass.Clone();
    myClass2.Set("hahaha", MyClass.Field3);
    Debug.Log(myClass.ToString());
    myClass.Remove(MyClass.Field5, 1, MyClass2.Field1);
    myClass.Remove(MyClass.Field5, 10, MyClass2.Field1);
    Debug.Log(myClass.ToString());
    Debug.Log(myClass2.ToString());
    Debug.Log(myClass.Get<MyClass2>(MyClass.Field5, 1));
    Debug.Log(myClass.Has(MyClass.Field5, 1));
    Debug.Log(myClass.Has(MyClass.Field3));
    Debug.Log(myClass.Has(MyClass.Field5));
}

public class MyClass : DictionaryObject<MyClass>
{
    private static int _nextId = 1;
    public static readonly int Field1 = _nextId++;
    public static readonly int Field2 = _nextId++;
    public static readonly int Field3 = _nextId++;
    public static readonly int Field4 = _nextId++;
    public static readonly int Field5 = _nextId++;

    static MyClass()
    {
        DefineField<int>(Field1, "F1");
        DefineField<byte>(Field2, "F2");
        DefineField<string>(Field3, "F3");
        DefineField<MyClass2>(Field4, "F4");
        DefineFieldList<MyClass2>(Field5, "F5");
    }
}

public class MyClass2 : DictionaryObject<MyClass2>
{
    private static int _nextId = 1;
    public static readonly int Field1 = _nextId++;

    static MyClass2()
    {
        DefineField<string>(Field1, "2F1");
    }
}
*/