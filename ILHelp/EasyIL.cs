/*
 * 作者:冰麟轻武
 * 说明:
 * 1....使用简单
 * 2....没有写太多的注释,因为还不完善,不建议单独使用,希望有能力看懂代码的朋友帮助完善
 * 
 * 有建议或者BUG可以联系:fttl_398@126.com,或者QQ21979018,或者http://t.qq.com/jy02305022
 * 所有提供建议或BUG者均加为好友,更新类库后第一时间发送给各位
 * 版本 1.0.0.0 日期:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace ILHelp
{
    //提供一种简单的利用 Microsoft 中间语言 (MSIL) 指令生成动态方法(DynamicMethod)委托的方法
    /// <summary>
    /// 提供一种简单的利用 Microsoft 中间语言 (MSIL) 指令生成动态方法(DynamicMethod)委托的方法
    /// </summary>
    public class EasyIL : IDisposable
    {
        public const BindingFlags ALLATTR = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        //构造函数
        /// <summary>
        /// 创建一个使用简单中间码生成动态方法，指定方法名称、返回类型、参数类型和动态方法逻辑关联的类型。
        /// </summary>
        /// <param name="returnType">一个 System.Type 对象，它指定动态方法的返回类型；如果方法没有返回类型，则为null。</param>
        /// <param name="m">一个System.Reflection.Module，表示动态方法将与之逻辑关联的模块。</param>
        /// <param name="parameterTypes">一个 System.Type 对象数组，它指定动态方法的参数的类型；如果方法没有参数，则不用。</param>
        public EasyIL(Type returnType, Module m, params Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0)
            {
                parameterTypes = null;
            }

            ddmethod = new DynamicMethod("", returnType, parameterTypes, m, true);          //创建动态方法
            il = ddmethod.GetILGenerator();                                                 //创建il中间码生成器
            builders = new Dictionary<string, ILObjectBase>(parameterTypes.Length);      //创建用于储存参数的集合

            for (int i = 0; i < parameterTypes.Length; i++)                                 //循环动态方法参数,并将参数存入变量集合
            {
                LocalBuilder lb = il.DeclareLocal(parameterTypes[i]);

                il.Emit((OpCode)typeof(OpCodes).GetField("Ldarg_" + i, ALLATTR).GetValue(null));
                il.Emit(OpCodes.Stloc, lb);

                builders.Add(i.ToString(), DeclareParameter(lb));
            }
        }

        ILGenerator il;
        DynamicMethod ddmethod;
        Dictionary<string, ILObjectBase> builders;

        public ILObjectBase this[string key]
        {
            get
            {
                return builders[key];
            }
        }

        //中间码生成器
        /// <summary>
        /// 中间码生成器
        /// </summary>
        public ILGenerator IL
        {
            get { return il; }
        }

        //参数的集合
        /// <summary>
        /// 参数的集合
        /// </summary>
        public Dictionary<string, ILObjectBase> Builders
        {
            get { return builders; }
            set { builders = value; }
        }

        //声明参数
        /// <summary>
        /// 声明参数
        /// </summary>
        public ILObjectBase DeclareParameter(LocalBuilder lb)
        {
            return new ILObjectBase(this.IL, ILType.LocalBuilder, lb);
        }

        public ILObjectBase DeclareParameter(ILType ilType, object obj)
        {
            return new ILObjectBase(this.IL, ilType, obj);
        }

        public ILObjectBase DeclareParameter(Type localType)
        {
            LocalBuilder lb = this.IL.DeclareLocal(localType);
            return new ILObjectBase(this.IL, ILType.LocalBuilder, lb);
        }

        public ILProperty CreateProperty(PropertyInfo property, IILValue target)
        {
            return new ILProperty(this.IL, property, target);
        }
        public ILProperty CreateProperty(PropertyInfo property, string targetName)
        {
            return new ILProperty(this.IL, property, Builders[targetName]);
        }
        public ILProperty CreateProperty(string propertyName, IILValue target)
        {
            PropertyInfo property = target.ValueType.GetProperty(propertyName, ALLATTR);
            return new ILProperty(this.IL, property, target);
        }
        public ILProperty CreateProperty(string propertyName, string targetName)
        {
            IILValue target = Builders[targetName];
            PropertyInfo property = target.ValueType.GetProperty(propertyName, ALLATTR);
            return new ILProperty(this.IL, property, target);
        }

        public ILField CreateField(FieldInfo field, IILValue target)
        {
            return new ILField(this.IL, field, target);
        }
        public ILField CreateField(FieldInfo field, string targetName)
        {
            return new ILField(this.IL, field, Builders[targetName]);
        }
        public ILField CreateField(string fieldName, IILValue target)
        {
            FieldInfo field = target.ValueType.GetField(fieldName, ALLATTR);
            return new ILField(this.IL, field, target);
        }
        public ILField CreateField(string fieldName, string targetName)
        {
            IILValue target = Builders[targetName];
            FieldInfo field = target.ValueType.GetField(fieldName, ALLATTR);
            return new ILField(this.IL, field, target);
        }

        public ILMethod CreateMethod(MethodInfo method, IILValue target)
        {
            return new ILMethod(this.IL, method, target);
        }
        public ILMethod CreateMethod(MethodInfo method, string targetName)
        {
            return new ILMethod(this.IL, method, Builders[targetName]);
        }
        public ILMethod CreateMethod(string methodName, IILValue target)
        {
            MethodInfo method = target.ValueType.GetMethod(methodName, ALLATTR);
            return new ILMethod(this.IL, method, target);
        }
        public ILMethod CreateMethod(string methodName, string targetName)
        {
            IILValue target = Builders[targetName];
            MethodInfo method = target.ValueType.GetMethod(methodName, ALLATTR);
            return new ILMethod(this.IL, method, target);
        }

        public ILObjectBase[] CreateArray(string arrayName, IILValue array)
        {
            throw new Exception("无法活动数组长度是个问题...");
            //LocalBuilder args = this.il.DeclareLocal(typeof(object[]));
            //this.il.Emit(OpCodes.Ldc_I4_1);
            //this.il.Emit(OpCodes.Newarr, typeof(object));
            //this.il.Emit(OpCodes.Stloc, args);
            //this.il.Emit(OpCodes.Ldloc, args);
            //this.il.Emit(OpCodes.Ldc_I4_0);
            //this.il.Emit(OpCodes.Ldloc, arg);
            //this.il.Emit(OpCodes.Stelem_Ref);
        }

        public ILObjectBase Convert(Type convType, ILObjectBase obj, params IILValue[] Parameters)
        {
            Type valueType = obj.ValueType;
            LocalBuilder lb = this.IL.DeclareLocal(convType);

            obj.LdValue();
            convert(convType, valueType);

            this.il.Emit(OpCodes.Stloc, lb);

            return DeclareParameter(lb);
        }

        public void Return(IILValue value, params IILValue[] Parameters)
        {
            Type returnType = this.ddmethod.ReturnType;
            Type valueType = value.ValueType;

            value.LdValue();

            convert(returnType, valueType);

            this.IL.Emit(OpCodes.Ret);
        }

        public void Return()
        {
            this.IL.Emit(OpCodes.Ret);
        }

        //创建一个可用于执行该方法的委托
        /// <summary>
        /// 完成动态方法并创建一个可用于执行该方法的委托。
        /// <para>返回结果:
        /// </para>   一个指定类型的委托，可用于执行动态方法。
        /// </summary>
        /// <param name="delegateType">一个签名与动态方法的签名匹配的委托类型。</param>
        public Delegate CreateDelegate(Type delegateType)
        {

            Delegate r = this.ddmethod.CreateDelegate(delegateType);
            Dispose();  //销毁对象引用
            return r;
        }

        //新建对象实例
        /// <summary>
        /// 新建对象实例
        /// </summary>
        /// <param name="objType">对象类型</param>
        /// <param name="Parameters">构造函数参数</param>
        public void New(Type objType, params IILValue[] Parameters)
        {
            Type[] argTypes = new Type[Parameters.Length];
            LocalBuilder[] argBuilder = new LocalBuilder[Parameters.Length];

            for (int i = 0; i < Parameters.Length; i++)
            {
                IILValue v = Parameters[i];
                argTypes[i] = v.ValueType;
                argBuilder[i] = v.StValue();
            }

            foreach (LocalBuilder var in argBuilder)
            {
                this.il.Emit(OpCodes.Ldloc, var);
            }

            this.il.Emit(OpCodes.Newobj, objType.GetConstructor(argTypes));
            //this.il.Emit(OpCodes.Stloc, ((LocalBuilder)(p0._arg)));
        }

        public void Dispose()
        {
            this.builders = null;
            this.ddmethod = null;
            this.il = null;
        }

        private void convert(Type convType, Type oldType)
        {
            if (convType == oldType)
            {
            }
            else if (convType == typeof(object))
            {
                this.IL.Emit(OpCodes.Box, oldType);
            }
            else if (convType.IsValueType && oldType.IsValueType)
            {
                this.IL.Emit(OpCodes.Conv_U);
            }
            else if (convType.IsClass && oldType.IsClass)
            {
                this.IL.Emit(OpCodes.Castclass, oldType);
                //throw new InvalidCastException("转型失败!");
            }
            else if (oldType == typeof(object))
            {
                this.IL.Emit(OpCodes.Unbox_Any, convType);
            }
        }
    }
}
