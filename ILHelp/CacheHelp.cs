/*
 * ����:��������
 * ˵��:
 * 1....ʹ�ü�,ʵ�ַ�ʽ��,Ч��Ҳ�Ƚϸ�
 * 2....ȫ����ע��,ע��ȫ��
 * 3....�������û���Ĵ�С����
 * 4....�Զ���ʹ��Ƶ�ʵ���˳��,���ȸ��ǲ�ʹ�õĻ���
 * 5....��ʱû��ʹ��ͬ����,�����п���
 * 
 * �н������BUG������ϵ:fttl_398@126.com,����QQ21979018,����http://t.qq.com/jy02305022
 * �����ṩ�����BUG�߾���Ϊ����,���������һʱ�䷢�͸���λ
 * �汾 1.0.0.0 ����:2010-4-16
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CacheHelp
{
    // �ṩ�򵥻���Ĺ���
    /// <summary>
    /// �ṩ�򵥻���Ĺ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EasyCache<T> : IDisposable
    {
        //�������ֵ,��Ҫ����̫��
        const int MAXSIZE = 50;

        Dictionary<string, T> items;
        List<string> keys;

        public EasyCache()
            : this(20)
        { }

        public EasyCache(int maxLength)
        {
            this.size = maxLength;
            this.items = new Dictionary<string, T>(MAXSIZE + 1);
            this.keys = new List<string>(MAXSIZE + 1);
        }

        int size;

        //��ȡ�����û������ֵ
        /// <summary>
        /// ��ȡ�����û����ֵ
        /// </summary>
        public int Size
        {
            get { return this.size; }
            set
            {
                if (value > MAXSIZE)
                {
                    value = MAXSIZE;
                }
                if (value < this.Count) //���������ֵС�ڵ�ǰ���л���
                {
                    RemoveOverflow();   //ɾ�����໺��
                }
                this.size = value;      //�����������ֵ
            }
        }

        //��ȡ������Ϣ,û�л��淵�� default(T)
        /// <summary>
        /// ��ȡ������Ϣ,û�л��淵�� default(T)
        /// </summary>
        public T this[string key]
        {
            get
            {
                T value;
                TryGetValue(key,out value);
                return value;
            }
        }

        //ɾ���������Ʋ��ֵĻ���
        /// <summary>
        /// ɾ���������Ʋ��ֵĻ���
        /// </summary>
        public void RemoveOverflow()
        {
            int overflow = this.keys.Count - this.Size; //��õ�ǰ�������ֵ
            if (overflow > 0)                           //�����������,��ɾ�����໺��
            {
                for (int i = 0; i < overflow; i++)      //ѭ��ɾ�����໺��
                {
                    this.items.Remove(this.keys[0]);    //������泬�����ֵ,ɾ����Զ��һ��
                    this.keys.RemoveAt(0);              //ɾ����Ӧ��key
                }
            }
        }

        //����һ������,���key�ظ��򸲸�
        /// <summary>
        /// ����һ������,���key�ظ��򸲸�
        /// </summary>
        public void Add(string key, T item)
        {
            this.items[key] = item;                 //д�绺��
            if (this.keys.Count >= this.Size)       //�жϻ����С�Ƿ񳬹�����
            {
                this.items.Remove(this.keys[0]);    //������泬�����ֵ,ɾ����Զ��һ��
                this.keys.RemoveAt(0);              //ɾ����Ӧ��key
            }
            else                                    //�������û������
            {
                this.keys.Remove(key);              //ɾ������ʹ�õ�key
            }
            this.keys.Add(key);                     //����ǰkey���뵽���ʹ��
        }

        //������л���
        /// <summary>
        /// ������л���
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
            this.keys.Clear();
            this.keys.Capacity = 0;
        }

        //ȷ�� ������ �Ƿ����ָ���ļ���
        /// <summary>
        /// ȷ�� ������ �Ƿ����ָ���ļ���
        /// </summary>
        public bool ContainsKey(string key)
        {
            return this.items.ContainsKey(key);
        }

        //��ȡ��ָ���ļ��������ֵ�� 
        /// <summary>
        /// ��ȡ��ָ���ļ��������ֵ�� 
        /// </summary>
        /// <param name="key">ָ���ļ�</param>
        /// <param name="value">������ֵ</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out T value)
        {
            if (this.items.TryGetValue(key, out value))
            {
                if (this.keys[this.items.Count - 1].Equals(key)) { }
                else
                {
                    this.keys.Remove(key);
                    this.keys.Add(key);
                }
                return true;
            }
            return false;
        }

        //��ȡ��ǰ�����С
        /// <summary>
        /// ��ȡ��ǰ�����С
        /// </summary>
        public int Count
        {
            get { return this.items.Count; }
        }

        public void Dispose()
        {
            this.items.Clear();
            this.keys.Clear();
            this.keys.Capacity = 0;
            this.items = null;
            this.keys = null;
        }

        //����key��˳��
        private void adjustKey(string key)
        {
            if (this.keys[0].Equals(key)) { }
            else
            {
                this.items.Remove(this.keys[0]);    //������泬�����ֵ,ɾ����Զ��һ��
                this.keys.RemoveAt(0);              //ɾ����Ӧ��key
            }
        }
    }
}
