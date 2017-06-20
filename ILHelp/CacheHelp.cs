/*
 * 作者:冰麟轻武
 * 说明:
 * 1....使用简单,实现方式简单,效率也比较高
 * 2....全中文注释,注释全面
 * 3....可以设置缓存的大小数量
 * 4....自动按使用频率调整顺序,优先覆盖不使用的缓存
 * 5....暂时没有使用同步锁,请自行控制
 * 
 * 有建议或者BUG可以联系:fttl_398@126.com,或者QQ21979018,或者http://t.qq.com/jy02305022
 * 所有提供建议或BUG者均加为好友,更新类库后第一时间发送给各位
 * 版本 1.0.0.0 日期:2010-4-16
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CacheHelp
{
    // 提供简单缓存的功能
    /// <summary>
    /// 提供简单缓存的功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EasyCache<T> : IDisposable
    {
        //缓存最大值,不要设置太大
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

        //获取或设置缓存最大值
        /// <summary>
        /// 获取或设置缓最大值
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
                if (value < this.Count) //如果社设置值小于当前已有缓存
                {
                    RemoveOverflow();   //删除多余缓存
                }
                this.size = value;      //重新设置最大值
            }
        }

        //获取缓存信息,没有缓存返回 default(T)
        /// <summary>
        /// 获取缓存信息,没有缓存返回 default(T)
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

        //删除超过限制部分的缓存
        /// <summary>
        /// 删除超过限制部分的缓存
        /// </summary>
        public void RemoveOverflow()
        {
            int overflow = this.keys.Count - this.Size; //获得当前缓存溢出值
            if (overflow > 0)                           //如果超过限制,则删除多余缓存
            {
                for (int i = 0; i < overflow; i++)      //循环删除多余缓存
                {
                    this.items.Remove(this.keys[0]);    //如果缓存超过最大值,删除最远的一个
                    this.keys.RemoveAt(0);              //删除对应的key
                }
            }
        }

        //加入一个缓存,如果key重复则覆盖
        /// <summary>
        /// 加入一个缓存,如果key重复则覆盖
        /// </summary>
        public void Add(string key, T item)
        {
            this.items[key] = item;                 //写如缓存
            if (this.keys.Count >= this.Size)       //判断缓存大小是否超过限制
            {
                this.items.Remove(this.keys[0]);    //如果缓存超过最大值,删除最远的一个
                this.keys.RemoveAt(0);              //删除对应的key
            }
            else                                    //如果缓存没到限制
            {
                this.keys.Remove(key);              //删除当先使用的key
            }
            this.keys.Add(key);                     //将当前key加入到最近使用
        }

        //清空所有缓存
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
            this.keys.Clear();
            this.keys.Capacity = 0;
        }

        //确定 缓存中 是否包含指定的键。
        /// <summary>
        /// 确定 缓存中 是否包含指定的键。
        /// </summary>
        public bool ContainsKey(string key)
        {
            return this.items.ContainsKey(key);
        }

        //获取与指定的键相关联的值。 
        /// <summary>
        /// 获取与指定的键相关联的值。 
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="value">关联的值</param>
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

        //获取当前缓存大小
        /// <summary>
        /// 获取当前缓存大小
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

        //调整key的顺序
        private void adjustKey(string key)
        {
            if (this.keys[0].Equals(key)) { }
            else
            {
                this.items.Remove(this.keys[0]);    //如果缓存超过最大值,删除最远的一个
                this.keys.RemoveAt(0);              //删除对应的key
            }
        }
    }
}
