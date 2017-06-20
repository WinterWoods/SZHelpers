/*
 * 作者:冰麟轻武
 * 说明:
 * 1....使用简单
 * 2....没有写太多的注释,因为还不完善,不建议单独使用,希望有能力看懂代码的朋友帮助完善
 * 
 * 有建议或者BUG可以联系:fttl_398@126.com,或者QQ 21979018,或者http://t.qq.com/jy02305022
 * 所有提供建议或BUG者均加为好友,更新类库后第一时间发送给各位
 * 版本 1.0.0.0 日期:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ILHelp
{
    //提供对 Microsoft 中间语言 (MSIL) 指令中属性(Property)对象快速操作的方法
    /// <summary>
    /// 提供对 Microsoft 中间语言 (MSIL) 指令中属性(Property)对象快速操作的方法
    /// </summary>
    public class ILProperty : ILObjectBase
    {
        protected IILValue target;
        protected new PropertyInfo obj;

        public ILProperty(ILGenerator il, PropertyInfo obj, IILValue target)
            : base(il, ILType.Property)
        {
            base.obj = obj;
            this.obj = obj;
            this.target = target;
            this.valueType = obj.PropertyType;

            Ld = delegate()
            {
                this.il.Emit(OpCodes.Call, ((PropertyInfo)obj).GetGetMethod());
            };
        }

        //将当前属性的值,推送到堆栈
        /// <summary>
        /// 将当前属性的值,推送到堆栈
        /// </summary>
        public override void LdValue()
        {
            if (this.obj.GetGetMethod().IsStatic)
                this.il.Emit(OpCodes.Ldnull);
            else
                this.target.LdValue();

            Ld();
        }

        public override void SetValue(LocalBuilder localBuilder)
        {

            //LocalBuilder args = this.il.DeclareLocal(typeof(object[]));
            //this.il.Emit(OpCodes.Ldc_I4_1);
            //this.il.Emit(OpCodes.Newarr, typeof(object));
            //this.il.Emit(OpCodes.Stloc, args);
            //this.il.Emit(OpCodes.Ldloc, args);
            //this.il.Emit(OpCodes.Ldc_I4_0);
            //this.il.Emit(OpCodes.Ldloc, arg);
            //this.il.Emit(OpCodes.Stelem_Ref);
            PropertyInfo propInfo = ((PropertyInfo)obj);
            LocalBuilder arg;
            if (propInfo.PropertyType == localBuilder.LocalType)
            {
                arg = localBuilder;
            }
            else
            {
                arg = this.il.DeclareLocal(propInfo.PropertyType);
                this.il.Emit(OpCodes.Ldloc, localBuilder);
                base.convert(propInfo.PropertyType, localBuilder.LocalType);
                this.il.Emit(OpCodes.Stloc, arg);
            }

            if (this.obj.GetGetMethod().IsStatic)
                this.il.Emit(OpCodes.Ldnull);
            else
                this.target.LdValue();

            this.il.Emit(OpCodes.Ldloc, arg);
            this.il.Emit(OpCodes.Call, propInfo.GetSetMethod());
            //this.il.Emit(OpCodes.Pop);
        }
    }
    //提供对 Microsoft 中间语言 (MSIL) 指令中字段(Field)对象快速操作的方法
    /// <summary>
    /// 提供对 Microsoft 中间语言 (MSIL) 指令中字段(Field)象快速操作的方法
    /// </summary>
    public class ILField : ILObjectBase
    {
        protected IILValue target;
        protected new FieldInfo obj;

        public ILField(ILGenerator il, FieldInfo obj, IILValue target)
            : base(il, ILType.Property)
        {
            base.obj = obj;
            this.obj = obj;
            this.target = target;
            this.valueType = obj.FieldType;

            if (((FieldInfo)obj).IsStatic)
                Ld = delegate() { this.il.Emit(OpCodes.Ldsfld, (FieldInfo)obj); };
            else
                Ld = delegate() { this.il.Emit(OpCodes.Ldfld, (FieldInfo)obj); };
        }

        //将当前字段的值,推送到堆栈
        /// <summary>
        /// 将当前字段的值,推送到堆栈
        /// </summary>
        public override void LdValue()
        {
            if (this.obj.IsStatic) { }
            else
                target.LdValue();

            Ld();
        }

        public override void SetValue(LocalBuilder localBuilder)
        {
            LocalBuilder arg;
            FieldInfo fieldinfo = ((FieldInfo)obj);
            if (fieldinfo.FieldType == localBuilder.LocalType)
            {
                arg = localBuilder;
            }
            else
            {
                arg = this.il.DeclareLocal(fieldinfo.FieldType);
                this.il.Emit(OpCodes.Ldloc, localBuilder);
                this.convert(fieldinfo.FieldType, localBuilder.LocalType);
                this.il.Emit(OpCodes.Stloc, arg);
            }

            if (fieldinfo.IsStatic)
            {
                this.il.Emit(OpCodes.Ldloc, arg);
                this.il.Emit(OpCodes.Stsfld, fieldinfo);
            }
            else
            {
                this.target.LdValue();
                this.il.Emit(OpCodes.Ldloc, arg);
                this.il.Emit(OpCodes.Stfld, fieldinfo);
            }
        }

    }
    //提供对 Microsoft 中间语言 (MSIL) 指令中方法(Method)对象快速操作的方法
    /// <summary>
    /// 提供对 Microsoft 中间语言 (MSIL) 指令中方法(Method)对象快速操作的方法
    /// </summary>
    public class ILMethod : ILObjectBase
    {
        protected IILValue target;
        protected new MethodInfo obj;

        public ILMethod(ILGenerator il, MethodInfo obj, IILValue target)
            : base(il, ILType.Property)
        {
            base.obj = obj;
            this.obj = obj;
            this.target = target;
            this.valueType = obj.ReturnType;

            Ld = delegate() { this.il.Emit(OpCodes.Call, (MethodInfo)obj); };
        }

        //将当前属性的值,推送到堆栈
        /// <summary>
        /// 将当前属性的值,推送到堆栈
        /// </summary>
        public override void LdValue()
        {
            if (this.obj.IsStatic) { }
            else
                target.LdValue();

            Ld();
        }

        public override LocalBuilder Call(IILValue array)
        {
            MethodInfo methodInfo = ((MethodInfo)obj);

            ParameterInfo[] paraInfos = methodInfo.GetParameters();
            LocalBuilder[] paras = new LocalBuilder[paraInfos.Length];

            Type elementType = array.ValueType.GetElementType();

            for (int i = 0; i < paraInfos.Length; i++)
            {

                paras[i] = this.il.DeclareLocal(paraInfos[i].ParameterType);
                array.LdValue();
                this.il.Emit(OpCodes.Ldc_I4, i);
                this.il.Emit(OpCodes.Ldelem_Ref);
                base.convert(paraInfos[i].ParameterType, elementType);
                this.il.Emit(OpCodes.Stloc, paras[i]);

            }

            if (methodInfo.IsStatic)
                this.il.Emit(OpCodes.Ldnull);
            else
                this.target.LdValue();

            for (int i = 0; i < paraInfos.Length; i++)
            {
                this.il.Emit(OpCodes.Ldloc, paras[i]);
            }

            LocalBuilder returnLocal = null;

            this.il.Emit(OpCodes.Call, methodInfo);

            if (methodInfo.ReturnType == typeof(void))
            {
            }
            else
            {
                returnLocal = il.DeclareLocal(methodInfo.ReturnType);
                this.il.Emit(OpCodes.Stloc, returnLocal);
            }
            return returnLocal;
        }
    }
}
