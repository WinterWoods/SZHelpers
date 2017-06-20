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
    //提供对 Microsoft 中间语言 (MSIL) 指令中动态对象快速操作的方法
    /// <summary>
    /// 提供对 Microsoft 中间语言 (MSIL) 指令中动态对象快速操作的方法
    /// </summary>
    public class ILObjectBase : IILValue
    {
        protected ILType ilType;
        protected ILGenerator il;
        protected object obj;
        protected Type valueType;
        //当前参数对象的值的类型
        /// <summary>
        /// 当前参数对象的值的类型
        /// </summary>
        public Type ValueType
        {
            get { return valueType; }
        }

        protected ILObjectBase(ILGenerator il, ILType ilType)
        {
            this.il = il;
            this.ilType = ilType;
        }

        public ILObjectBase(ILGenerator il, ILType ilType, object obj)
        {
            this.obj = obj;
            this.ilType = ilType;
            this.il = il;
            if ((int)ilType <= 4)
            {
                this.valueType = obj.GetType();
            }
            else if (ilType == ILType.LocalBuilder)
            {
                this.valueType = ((LocalBuilder)obj).LocalType;
            }
            else if (ilType == ILType.Null)
            {
                this.valueType = typeof(void);
            }
            else
            {
                throw new ArgumentException("ILParameterBase类无法构造该类型的IL参数实例");
            }

            this.InitLdManner();
        }
        //直接推送
        /// <summary>
        /// 直接推送
        /// </summary>
        protected DeleLd Ld;
        //初始化推送方式
        /// <summary>
        /// 初始化推送方式
        /// </summary>
        protected virtual void InitLdManner()
        {
            switch (this.ilType)
            {
                case ILType.String:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldstr, (string)obj); };
                    break;
                case ILType.Int:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldind_I4, (int)obj); };
                    break;
                case ILType.Long:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldind_I8, (long)obj); };
                    break;
                case ILType.Double:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldind_R8, (double)obj); };
                    break;
                //case ILType.Object:
                //    break;
                case ILType.LocalBuilder:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldloc, (LocalBuilder)obj); };
                    break;
                case ILType.Null:
                    Ld = delegate() { this.il.Emit(OpCodes.Ldnull); };
                    break;
                case ILType.Method:
                    Ld = delegate() { this.il.Emit(OpCodes.Call, (MethodInfo)obj); };
                    break;
                case ILType.Field:
                    if (((FieldInfo)obj).IsStatic)
                        Ld = delegate() { this.il.Emit(OpCodes.Ldsfld, (FieldInfo)obj); };
                    else
                        Ld = delegate() { this.il.Emit(OpCodes.Ldfld, (FieldInfo)obj); };
                    break;
                case ILType.Property:
                    Ld = delegate() { il.Emit(OpCodes.Ldnull); this.il.Emit(OpCodes.Call, ((PropertyInfo)obj).GetGetMethod()); };
                    break;
            }

        }
        //将当前参数的值,推送到堆栈
        /// <summary>
        /// 将当前参数的值,推送到堆栈
        /// </summary>
        public virtual void LdValue()
        {
            if ((int)ilType <= 6)
            {
                Ld();
            }
            else
            {
                throw new InvalidOperationException("无法由基类方法推送当前类型的参数值");
            }
        }
        //将当前参数的值,存储到一个新的localBuilder中并返回
        /// <summary>
        /// 将当前参数的值,存储到一个新的localBuilder中并返回
        /// </summary>
        public LocalBuilder StValue()
        {
            if (ilType == ILType.LocalBuilder)
            {
                return (LocalBuilder)obj;
            }
            else
            {
                LdValue();
                LocalBuilder lb = this.il.DeclareLocal(this.valueType);
                this.il.Emit(OpCodes.Stloc, lb);
                return lb;
            }
        }

        protected void convert(Type convType, Type oldType)
        {
            if (convType == oldType)
            {
            }
            else if (convType == typeof(object))
            {
                this.il.Emit(OpCodes.Box, oldType);
            }
            else if (convType.IsValueType && oldType.IsValueType)
            {
                this.il.Emit(OpCodes.Conv_U);
            }
            else if (convType.IsClass && oldType.IsClass)
            {
                this.il.Emit(OpCodes.Castclass, oldType);
                //throw new InvalidCastException("转型失败!");
            }
            else if (oldType == typeof(object))
            {
                this.il.Emit(OpCodes.Unbox_Any, convType);
            }
        }

        //将当前参数的值,存储到指定类型的localBuilder中并返回(如果转型失败抛出异常)
        /// <summary>
        /// 将当前参数的值,存储到指定类型的localBuilder中并返回(如果转型失败抛出异常)
        /// </summary>
        public LocalBuilder StValue(Type localType)
        {
            LocalBuilder lb = this.il.DeclareLocal(localType);
            LdValue();
            this.convert(localType, this.ValueType);
            this.il.Emit(OpCodes.Stloc, lb);
            return lb;
        }

        public virtual void SetValue(LocalBuilder localBuilder)
        {
            throw new Exception("当前类不能使用此方法");
        }

        public virtual LocalBuilder Call(IILValue Parameters)
        {
            throw new Exception("当前类不能使用此方法");
        }
    }
}
