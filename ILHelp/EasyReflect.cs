/*
 * 作者:冰麟轻武
 * 说明:
 * 1....使用简单
 * 2....不区分静态和实例都可以反射
 * 3....不区分public和private都可以反射,请小心使用
 * 4....暂时不支持泛型方法,in和out参数的方法,有好的建议可以联系我
 * 5....中文注释已经比较全面
 * 
 * 有建议或者BUG可以联系:fttl_398@126.com,或者QQ 21979018,或者http://t.qq.com/jy02305022
 * 所有提供建议或BUG者均加为好友,更新类库后第一时间发送给各位
 * 版本 1.0.0.0 日期:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CacheHelp;
using System.Reflection;
using System.Reflection.Emit;

namespace ILHelp
{
    //public delegate R delegateGeneric<R, T>(T t);
    public delegate object ReflectGet(object obj);
    public delegate void ReflectSet(object obj, object value);
    public delegate object ReflectCall(object obj, params object[] args);
    //提供简单的反射对象的属性,字段,方法的方式
    /// <summary>
    /// 提供简单的反射对象的属性,字段,方法的方式
    /// </summary>
    public static class EasyReflect
    {
        static EasyCache<ReflectGet> cacheGet;
        static EasyCache<ReflectSet> cacheSet;
        static EasyCache<ReflectCall> cacheCall;

        const string SPACE = "!";       //最好不要设置变量名可以使用的字符
        static EasyReflect()            //静态构造方法,设置缓存区大小
        {
            cacheGet = new EasyCache<ReflectGet>(25);
            cacheSet = new EasyCache<ReflectSet>(25);
            cacheCall = new EasyCache<ReflectCall>(25);
        }

        //获取一个获取特定属性的值的委托
        /// <summary>
        /// 获取一个获取特定属性的值的委托
        /// <para>返回结果:
        /// </para>一个object ReflectGet(object obj)类型的委托
        /// </summary>
        /// <param name="classType">需要获取属性的类</param>
        /// <param name="propertyName">属性名称</param>
        public static ReflectGet GetPropertyValue(Type classType, string propertyName)
        {
            string cacheKey = classType.FullName + SPACE + propertyName;//缓存标识符key

            ReflectGet returnDelegate = null;                                   //声明返回委托对象
            if (cacheGet.TryGetValue(cacheKey, out returnDelegate))             //取出缓存中的委托,如果没有返回false
            {
                return returnDelegate;                                          //如果有,直接返回该委托
            }
            ReflectGet reflectGet = null;
            returnDelegate = delegate(object obj) { return reflectGet(obj); };  //给返回委托赋值
            cacheGet.Add(cacheKey, returnDelegate);                             //添加到缓存

            PropertyInfo propertyInfo =
                    classType.GetProperty(propertyName, EasyIL.ALLATTR);        //获得属性对象PropertyInfo

            reflectGet = delegate(object obj)                                   //使用系统反射方法
            {
                return propertyInfo.GetGetMethod().Invoke(obj, null);
            };

            //开启新线程初始化动态方法
            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                //Type returnType = classType.GetProperty(propertyName, EasyIL.ALLATTR).PropertyType;
                Type returnType = typeof(object);                               //设置返回值类型
                Type argType = typeof(object);                                  //设置参数类型

                EasyIL il = new EasyIL(returnType, classType.Module, argType);  //建立简单EasyIL对象
                //EasyIL:自定义类,用于简单操作IL生成器生成动态方法

                //ILParameterBase obj = il.Convert(classType, il["0"]);         //将参数0转换为
                ILProperty objProperty =
                    il.CreateProperty(propertyInfo, il["0"]);                   //根据对象和属性对象 创建IL参数
                //il[0]表示第0个参数的ILProperty形式,ILProperty:自定义类,用于在EasyIL中代表一个参数
                il.Return(objProperty);                                         //return 属性的值
                //Type delegateType = typeof(delegateGeneric<,>).MakeGenericType(returnType, classType);
                reflectGet = (ReflectGet)il.CreateDelegate(typeof(ReflectGet)); //返回一个委托

            });


            return returnDelegate;
        }
        //获取一个获取特定字段的值的委托
        /// <summary>
        /// 获取一个获取特定字段的值的委托
        /// <para>返回结果:
        /// </para>一个object ReflectGet(object obj)类型的委托
        /// </summary>
        /// <param name="classType">需要获取字段的类</param>
        /// <param name="fieldName">字段名称</param>
        public static ReflectGet GetFieldValue(Type classType, string fieldName)
        {
            ReflectGet returnDelegate;

            string cacheKey = classType.FullName + SPACE + fieldName;
            if (cacheGet.TryGetValue(cacheKey, out returnDelegate))
            {
                return returnDelegate;
            }
            cacheGet.Add(cacheKey, returnDelegate);

            FieldInfo fieldInfo = classType.GetField(fieldName, EasyIL.ALLATTR);

            ReflectGet reflectGet = delegate(object obj)
            {
                return fieldInfo.GetValue(obj);
            };

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                Type returnType = typeof(object);
                Type argType = typeof(object);

                EasyIL il = new EasyIL(returnType, classType.Module, argType);

                ILField objfield = il.CreateField(fieldInfo, il["0"]);

                il.Return(objfield);

                reflectGet = (ReflectGet)il.CreateDelegate(typeof(ReflectGet));
            });

            returnDelegate = delegate(object obj) { return reflectGet(obj); };
            return returnDelegate;
        }
        //获取一个设置特定属性的值的委托
        /// <summary>
        /// 获取一个设置特定属性的值的委托
        /// <para>返回结果:
        /// </para>一个void ReflectSet(object obj, object value)类型的委托
        /// </summary>
        /// <param name="classType">需要设置属性的类</param>
        /// <param name="propertyName">属性名称</param>
        public static ReflectSet SetPropertyValue(Type classType, string propertyName)
        {
            ReflectSet returnDelegate;

            string cacheKey = classType.FullName + SPACE + propertyName;
            if (cacheSet.TryGetValue(cacheKey, out returnDelegate))
            {
                return returnDelegate;
            }
            ReflectSet reflectSet = null;
            returnDelegate = delegate(object obj, object value) { reflectSet(obj, value); };
            cacheSet.Add(cacheKey, returnDelegate);

            PropertyInfo propertyInfo =
                    classType.GetProperty(propertyName, EasyIL.ALLATTR);            //获得属性对象PropertyInfo

            reflectSet = delegate(object obj, object value)
            {
                propertyInfo.GetSetMethod().Invoke(obj, new object[] { value });
            };

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                Type argType = typeof(object);
                EasyIL il = new EasyIL(null, classType.Module, argType, argType);

                ILProperty objProperty = il.CreateProperty(propertyInfo, il["0"]);  //根据对象和属性对象 创建IL参数


                objProperty.SetValue(il["1"].StValue());

                il.Return();
                reflectSet = (ReflectSet)il.CreateDelegate(typeof(ReflectSet));
            });


            return returnDelegate;
        }
        //获取一个设置特定字段的值的委托
        /// <summary>
        /// 获取一个设置特定字段的值的委托
        /// <para>返回结果:
        /// </para>一个void ReflectSet(object obj, object value)类型的委托
        /// </summary>
        /// <param name="classType">需要设置字段的类</param>
        /// <param name="fieldName">字段名称</param>
        public static ReflectSet SetFieldValue(Type classType, string fieldName)
        {
            ReflectSet returnDelegate;

            string cacheKey = classType.FullName + SPACE + fieldName;
            if (cacheSet.TryGetValue(cacheKey, out returnDelegate))
            {
                return returnDelegate;
            }
            ReflectSet reflectSet = null;
            returnDelegate = delegate(object obj, object value) { reflectSet(obj, value); };
            cacheSet.Add(cacheKey, returnDelegate);

            FieldInfo fieldInfo =
                    classType.GetField(fieldName, EasyIL.ALLATTR);        //获得属性对象FieldInfo

            reflectSet = delegate(object obj, object value)
            {
                fieldInfo.SetValue(obj, value);
            };

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                Type argType = typeof(object);
                EasyIL il = new EasyIL(null, classType.Module, argType, argType);

                ILField objField = il.CreateField(fieldInfo, il["0"]);//根据对象和属性对象 创建IL参数
                objField.SetValue(il["1"].StValue());
                il.Return();
                reflectSet = (ReflectSet)il.CreateDelegate(typeof(ReflectSet));
            });


            return returnDelegate;
        }
        //获取一个特定方法的委托
        /// <summary>
        /// 获取一个特定方法的委托
        /// <para>返回结果:
        /// </para>一个object ReflectCall(object obj, params object[] args)类型的委托
        /// <para>当方法返回值为void时,返回一个null</para>
        /// </summary>
        /// <param name="classType">方法所属的类</param>
        /// <param name="methodName">方法名</param>
        public static ReflectCall CallMethod(Type classType, string methodName)
        {
            ReflectCall returnDelegate;
            string cacheKey = classType.FullName + SPACE + methodName;
            if (cacheCall.TryGetValue(cacheKey, out returnDelegate))
            {
                return returnDelegate;
            }
            ReflectCall reflectCall = null;
            returnDelegate = delegate(object obj, object[] args) { return reflectCall(obj, args); };
            cacheCall.Add(cacheKey, returnDelegate);

            MethodInfo method = classType.GetMethod(methodName, EasyIL.ALLATTR);

            reflectCall = delegate(object obj, object[] args)
            {
                return method.Invoke(obj, args);
            };

            ThreadPool.QueueUserWorkItem(delegate(object obj)
            {
                Type returnType = typeof(object);
                Type objType = typeof(object);
                Type argsType = typeof(object[]);
                EasyIL il = new EasyIL(returnType, classType.Module, objType, argsType);

                ILMethod objMethod = il.CreateMethod(method, il["0"]);

                LocalBuilder lb = objMethod.Call(il["1"]);
                if (lb != null)
                {
                    il.IL.Emit(OpCodes.Ldloc, lb);
                }
                else
                {
                    il.IL.Emit(OpCodes.Ldnull);
                }

                il.Return();

                reflectCall = (ReflectCall)il.CreateDelegate(typeof(ReflectCall));
            });

            return returnDelegate;
        }

    }
}
