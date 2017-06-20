/*
 * ����:��������
 * ˵��:
 * 1....ʹ�ü�
 * 2....û��д̫���ע��,��Ϊ��������,�����鵥��ʹ��,ϣ��������������������Ѱ�������
 * 
 * �н������BUG������ϵ:fttl_398@126.com,����QQ 21979018,����http://t.qq.com/jy02305022
 * �����ṩ�����BUG�߾���Ϊ����,���������һʱ�䷢�͸���λ
 * �汾 1.0.0.0 ����:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ILHelp
{
    //�ṩ�� Microsoft �м����� (MSIL) ָ��������(Property)������ٲ����ķ���
    /// <summary>
    /// �ṩ�� Microsoft �м����� (MSIL) ָ��������(Property)������ٲ����ķ���
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

        //����ǰ���Ե�ֵ,���͵���ջ
        /// <summary>
        /// ����ǰ���Ե�ֵ,���͵���ջ
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
    //�ṩ�� Microsoft �м����� (MSIL) ָ�����ֶ�(Field)������ٲ����ķ���
    /// <summary>
    /// �ṩ�� Microsoft �м����� (MSIL) ָ�����ֶ�(Field)����ٲ����ķ���
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

        //����ǰ�ֶε�ֵ,���͵���ջ
        /// <summary>
        /// ����ǰ�ֶε�ֵ,���͵���ջ
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
    //�ṩ�� Microsoft �м����� (MSIL) ָ���з���(Method)������ٲ����ķ���
    /// <summary>
    /// �ṩ�� Microsoft �м����� (MSIL) ָ���з���(Method)������ٲ����ķ���
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

        //����ǰ���Ե�ֵ,���͵���ջ
        /// <summary>
        /// ����ǰ���Ե�ֵ,���͵���ջ
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
