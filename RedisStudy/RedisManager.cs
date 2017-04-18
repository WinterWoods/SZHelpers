using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using System.Configuration;

namespace RedisStudy
{
    /// <summary>
    /// RedisManager类主要是创建链接池管理对象的
    /// </summary>
    public class RedisManager
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static string RedisPath = ConfigurationManager.AppSettings["redisIP"].ToString()+":"+ ConfigurationManager.AppSettings["redisProt"].ToString();
        private static PooledRedisClientManager _prcm;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static void CreateManager(string ip, string port, string password)
        {
            _prcm = CreateManager(new string[] { password+"@" +ip+":"+port }, new string[] { password + "@" + ip + ":" + port });
        }

        
        private static PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            //WriteServerList：可写的Redis链接地址。
            //ReadServerList：可读的Redis链接地址。
            //MaxWritePoolSize：最大写链接数。
            //MaxReadPoolSize：最大读链接数。
            //AutoStart：自动重启。
            //LocalCacheTime：本地缓存到期时间，单位:秒。
            //RecordeLog：是否记录日志,该设置仅用于排查redis运行时出现的问题,如redis工作正常,请关闭该项。
            //RedisConfigInfo类是记录redis连接信息，此信息和配置文件中的RedisConfig相呼应

            // 支持读写分离，均衡负载 
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = 500, // “写”链接池链接数 
                MaxReadPoolSize = 500, // “读”链接池链接数 
                AutoStart = true,
            });
        }

        private static IEnumerable<string> SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IRedisClient GetClient(string ip,string port,string password)
        {
            if (_prcm == null)
            {
                CreateManager(ip,port,password);
            }
            return _prcm.GetClient();
        }

    }
}
