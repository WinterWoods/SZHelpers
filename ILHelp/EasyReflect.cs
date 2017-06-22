/*
 * ����:��������
 * ˵��:
 * 1....ʹ�ü�
 * 2....�����־�̬��ʵ�������Է���
 * 3....������public��private�����Է���,��С��ʹ��
 * 4....��ʱ��֧�ַ��ͷ���,in��out�����ķ���,�кõĽ��������ϵ��
 * 5....����ע���Ѿ��Ƚ�ȫ��
 * 
 * �н������BUG������ϵ:fttl_398@126.com,����QQ 21979018,����http://t.qq.com/jy02305022
 * �����ṩ�����BUG�߾���Ϊ����,���������һʱ�䷢�͸���λ
 * �汾 1.0.0.0 ����:2010-4-16
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
    //�ṩ�򵥵ķ�����������,�ֶ�,�����ķ�ʽ
    /// <summary>
    /// �ṩ�򵥵ķ�����������,�ֶ�,�����ķ�ʽ
    /// </summary>
    public static class EasyReflect
    {
        static EasyCache<ReflectGet> cacheGet;
        static EasyCache<ReflectSet> cacheSet;
        static EasyCache<ReflectCall> cacheCall;

        const string SPACE = "!";       //��ò�Ҫ���ñ���������ʹ�õ��ַ�
        static EasyReflect()            //��̬���췽��,���û�������С
        {
            cacheGet = new EasyCache<ReflectGet>(25);
            cacheSet = new EasyCache<ReflectSet>(25);
            cacheCall = new EasyCache<ReflectCall>(25);
        }

        //��ȡһ����ȡ�ض����Ե�ֵ��ί��
        /// <summary>
        /// ��ȡһ����ȡ�ض����Ե�ֵ��ί��
        /// <para>���ؽ��:
        /// </para>һ��object ReflectGet(object obj)���͵�ί��
        /// </summary>
        /// <param name="classType">��Ҫ��ȡ���Ե���</param>
        /// <param name="propertyName">��������</param>
        public static ReflectGet GetPropertyValue(Type classType, string propertyName)
        {
            string cacheKey = classType.FullName + SPACE + propertyName;//�����ʶ��key

            ReflectGet returnDelegate = null;                                   //��������ί�ж���
            if (cacheGet.TryGetValue(cacheKey, out returnDelegate))             //ȡ�������е�ί��,���û�з���false
            {
                return returnDelegate;                                          //�����,ֱ�ӷ��ظ�ί��
            }
            ReflectGet reflectGet = null;
            returnDelegate = delegate(object obj) { return reflectGet(obj); };  //������ί�и�ֵ
            cacheGet.Add(cacheKey, returnDelegate);                             //��ӵ�����

            PropertyInfo propertyInfo =
                    classType.GetProperty(propertyName, EasyIL.ALLATTR);        //������Զ���PropertyInfo

            reflectGet = delegate(object obj)                                   //ʹ��ϵͳ���䷽��
            {
                return propertyInfo.GetGetMethod().Invoke(obj, null);
            };

            //�������̳߳�ʼ����̬����
            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                //Type returnType = classType.GetProperty(propertyName, EasyIL.ALLATTR).PropertyType;
                Type returnType = typeof(object);                               //���÷���ֵ����
                Type argType = typeof(object);                                  //���ò�������

                EasyIL il = new EasyIL(returnType, classType.Module, argType);  //������EasyIL����
                //EasyIL:�Զ�����,���ڼ򵥲���IL���������ɶ�̬����

                //ILParameterBase obj = il.Convert(classType, il["0"]);         //������0ת��Ϊ
                ILProperty objProperty =
                    il.CreateProperty(propertyInfo, il["0"]);                   //���ݶ�������Զ��� ����IL����
                //il[0]��ʾ��0��������ILProperty��ʽ,ILProperty:�Զ�����,������EasyIL�д���һ������
                il.Return(objProperty);                                         //return ���Ե�ֵ
                //Type delegateType = typeof(delegateGeneric<,>).MakeGenericType(returnType, classType);
                reflectGet = (ReflectGet)il.CreateDelegate(typeof(ReflectGet)); //����һ��ί��

            });


            return returnDelegate;
        }
        //��ȡһ����ȡ�ض��ֶε�ֵ��ί��
        /// <summary>
        /// ��ȡһ����ȡ�ض��ֶε�ֵ��ί��
        /// <para>���ؽ��:
        /// </para>һ��object ReflectGet(object obj)���͵�ί��
        /// </summary>
        /// <param name="classType">��Ҫ��ȡ�ֶε���</param>
        /// <param name="fieldName">�ֶ�����</param>
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
        //��ȡһ�������ض����Ե�ֵ��ί��
        /// <summary>
        /// ��ȡһ�������ض����Ե�ֵ��ί��
        /// <para>���ؽ��:
        /// </para>һ��void ReflectSet(object obj, object value)���͵�ί��
        /// </summary>
        /// <param name="classType">��Ҫ�������Ե���</param>
        /// <param name="propertyName">��������</param>
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
                    classType.GetProperty(propertyName, EasyIL.ALLATTR);            //������Զ���PropertyInfo

            reflectSet = delegate(object obj, object value)
            {
                propertyInfo.GetSetMethod().Invoke(obj, new object[] { value });
            };

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                Type argType = typeof(object);
                EasyIL il = new EasyIL(null, classType.Module, argType, argType);

                ILProperty objProperty = il.CreateProperty(propertyInfo, il["0"]);  //���ݶ�������Զ��� ����IL����


                objProperty.SetValue(il["1"].StValue());

                il.Return();
                reflectSet = (ReflectSet)il.CreateDelegate(typeof(ReflectSet));
            });


            return returnDelegate;
        }
        //��ȡһ�������ض��ֶε�ֵ��ί��
        /// <summary>
        /// ��ȡһ�������ض��ֶε�ֵ��ί��
        /// <para>���ؽ��:
        /// </para>һ��void ReflectSet(object obj, object value)���͵�ί��
        /// </summary>
        /// <param name="classType">��Ҫ�����ֶε���</param>
        /// <param name="fieldName">�ֶ�����</param>
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
                    classType.GetField(fieldName, EasyIL.ALLATTR);        //������Զ���FieldInfo

            reflectSet = delegate(object obj, object value)
            {
                fieldInfo.SetValue(obj, value);
            };

            ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                Type argType = typeof(object);
                EasyIL il = new EasyIL(null, classType.Module, argType, argType);

                ILField objField = il.CreateField(fieldInfo, il["0"]);//���ݶ�������Զ��� ����IL����
                objField.SetValue(il["1"].StValue());
                il.Return();
                reflectSet = (ReflectSet)il.CreateDelegate(typeof(ReflectSet));
            });


            return returnDelegate;
        }
        //��ȡһ���ض�������ί��
        /// <summary>
        /// ��ȡһ���ض�������ί��
        /// <para>���ؽ��:
        /// </para>һ��object ReflectCall(object obj, params object[] args)���͵�ί��
        /// <para>����������ֵΪvoidʱ,����һ��null</para>
        /// </summary>
        /// <param name="classType">������������</param>
        /// <param name="methodName">������</param>
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
