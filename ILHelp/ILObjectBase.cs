/*
 * ����:��������
 * ˵��:
 * 1....ʹ�ü�
 * 2....û��д̫���ע��,��Ϊ��������,�����鵥��ʹ��,ϣ��������������������Ѱ�������
 * 
 * �н������BUG������ϵ:fttl_398@126.com,����QQ21979018,����http://t.qq.com/jy02305022
 * �����ṩ�����BUG�߾���Ϊ����,���������һʱ�䷢�͸���λ
 * �汾 1.0.0.0 ����:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace ILHelp
{
    //�ṩ�� Microsoft �м����� (MSIL) ָ���ж�̬������ٲ����ķ���
    /// <summary>
    /// �ṩ�� Microsoft �м����� (MSIL) ָ���ж�̬������ٲ����ķ���
    /// </summary>
    public class ILObjectBase : IILValue
    {
        protected ILType ilType;
        protected ILGenerator il;
        protected object obj;
        protected Type valueType;
        //��ǰ���������ֵ������
        /// <summary>
        /// ��ǰ���������ֵ������
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
                throw new ArgumentException("ILParameterBase���޷���������͵�IL����ʵ��");
            }

            this.InitLdManner();
        }
        //ֱ������
        /// <summary>
        /// ֱ������
        /// </summary>
        protected DeleLd Ld;
        //��ʼ�����ͷ�ʽ
        /// <summary>
        /// ��ʼ�����ͷ�ʽ
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
        //����ǰ������ֵ,���͵���ջ
        /// <summary>
        /// ����ǰ������ֵ,���͵���ջ
        /// </summary>
        public virtual void LdValue()
        {
            if ((int)ilType <= 6)
            {
                Ld();
            }
            else
            {
                throw new InvalidOperationException("�޷��ɻ��෽�����͵�ǰ���͵Ĳ���ֵ");
            }
        }
        //����ǰ������ֵ,�洢��һ���µ�localBuilder�в�����
        /// <summary>
        /// ����ǰ������ֵ,�洢��һ���µ�localBuilder�в�����
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
                //throw new InvalidCastException("ת��ʧ��!");
            }
            else if (oldType == typeof(object))
            {
                this.il.Emit(OpCodes.Unbox_Any, convType);
            }
        }

        //����ǰ������ֵ,�洢��ָ�����͵�localBuilder�в�����(���ת��ʧ���׳��쳣)
        /// <summary>
        /// ����ǰ������ֵ,�洢��ָ�����͵�localBuilder�в�����(���ת��ʧ���׳��쳣)
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
            throw new Exception("��ǰ�಻��ʹ�ô˷���");
        }

        public virtual LocalBuilder Call(IILValue Parameters)
        {
            throw new Exception("��ǰ�಻��ʹ�ô˷���");
        }
    }
}
