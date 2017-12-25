using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class TrieNode
    {
        public bool m_end;
        public Dictionary<Char, TrieNode> m_values;

        public TrieNode()
        {
            m_values = new Dictionary<Char, TrieNode>();
        }
    }

    public class BadWordManger : TrieNode
    {
        private const char repaceChar = '*';
        private static BadWordManger _instance;

        public static BadWordManger Instance
        {
            get { return _instance ?? (_instance = new BadWordManger()); }
        }

        public void Init()
        {
            string allWord = Resources.Load<TextAsset>("BadWordBank/BadWordBank").text;
            string[] allKey = allWord.Trim().Split('\n');
            for (int i = 0; i < allKey.Length; i++)
            {
                AddKey(allKey[i].Trim());
            }
        }

        public void InputFeidAddListen(InputField inputField)
        {
            inputField.onValueChanged.AddListener((string inputtext) =>
            {
                inputField.text = Replace(inputtext, repaceChar);
            });
        }

        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            TrieNode node = this;
            for (int i = 0; i < key.Length; i++)
            {
                char c = key[i];
                TrieNode subnode;
                if (!node.m_values.TryGetValue(c, out subnode))
                {
                    subnode = new TrieNode();
                    node.m_values.Add(c, subnode);
                }
                node = subnode;
            }
            node.m_end = true;
        }

        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>找到的第1个非法字符.没有则返回string.Empty</returns>
        public bool HasBadWord(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode node;
                if (m_values.TryGetValue(text[i], out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.m_values.TryGetValue(text[j], out node))
                        {
                            if (node.m_end)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否包含非法字符
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <returns>找到的第1个非法字符.没有则返回string.Empty</returns>
        public string FindOne(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                TrieNode node;
                if (m_values.TryGetValue(c, out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.m_values.TryGetValue(text[j], out node))
                        {
                            if (node.m_end)
                            {
                                return text.Substring(i, j + 1 - i);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return string.Empty;
        }

        //查找所有非法字符
        public IEnumerable<string> FindAll(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode node;
                if (m_values.TryGetValue(text[i], out node))
                {
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (node.m_values.TryGetValue(text[j], out node))
                        {
                            if (node.m_end)
                            {
                                yield return text.Substring(i, (j + 1 - i));
                            }
                        }
                        else
                        {
//                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 替换非法字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="c">用于代替非法字符</param>
        /// <returns>替换后的字符串</returns>
        public string Replace(string text, char c)
        {
            char[] chars = null;
            for (int i = 0; i < text.Length; i++)
            {
                TrieNode subnode;
                TrieNode tempsubnode = new TrieNode();
                CharType type = GetCharType(text[i]);
                if (m_values.TryGetValue(text[i], out subnode))
                {
                    int num = 0;
                    tempsubnode = subnode;
                    if (subnode.m_end)
                    {
                        if (chars == null) chars = text.ToArray();

                        chars[i] = c;
                        continue;
                    }
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (subnode.m_values.TryGetValue(text[j], out subnode))
                        {
                            tempsubnode = subnode;
                            if (subnode.m_end)
                            {
                                if (chars == null) chars = text.ToArray();
                                for (int t = i; t <= j; t++)
                                {
                                    chars[t] = c;
                                }
                                i = j;
                                break;
                            }
                        }
                        else
                        {
                            subnode = tempsubnode;
                            if (GetCharType(text[j]) == type)
                            {
                                break;
                            }
                            else
                            {
                                num++;
                                if (num > 5)
                                {
                                    break;
                                }
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    subnode = tempsubnode;
                }
            }
            return chars == null ? text : new string(chars);
        }

        private CharType GetCharType(char ca)
        {
            CharType type = CharType.Other;
            if (ca >= 0x4e00 && ca <= 0x9fbb)
            {
                type = CharType.Hanzi;
            }
            if (char.IsNumber(ca))
            {
                type = CharType.Number;
            }
            if (char.IsLetter(ca))
            {
                type = CharType.Character;
            }
            return type;
        }

        public enum CharType
        {
            Number,
            Hanzi,
            Character,
            Other
        }
    }
}