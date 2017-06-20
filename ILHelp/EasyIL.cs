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
    //�ṩһ�ּ򵥵����� Microsoft �м����� (MSIL) ָ�����ɶ�̬����(DynamicMethod)ί�еķ���
    /// <summary>
    /// �ṩһ�ּ򵥵����� Microsoft �м����� (MSIL) ָ�����ɶ�̬����(DynamicMethod)ί�еķ���
    /// </summary>
    public class EasyIL : IDisposable
    {
        public const BindingFlags ALLATTR = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        //���캯��
        /// <summary>
        /// ����һ��ʹ�ü��м������ɶ�̬������ָ���������ơ��������͡��������ͺͶ�̬�����߼����������͡�
        /// </summary>
        /// <param name="returnType">һ�� System.Type ������ָ����̬�����ķ������ͣ��������û�з������ͣ���Ϊnull��</param>
        /// <param name="m">һ��System.Reflection.Module����ʾ��̬��������֮�߼�������ģ�顣</param>
        /// <param name="parameterTypes">һ�� System.Type �������飬��ָ����̬�����Ĳ��������ͣ��������û�в��������á�</param>
        public EasyIL(Type returnType, Module m, params Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0)
            {
                parameterTypes = null;
            }

            ddmethod = new DynamicMethod("", returnType, parameterTypes, m, true);          //������̬����
            il = ddmethod.GetILGenerator();                                                 //����il�м���������
            builders = new Dictionary<string, ILObjectBase>(parameterTypes.Length);      //�������ڴ�������ļ���

            for (int i = 0; i < parameterTypes.Length; i++)                                 //ѭ����̬��������,�������������������
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

        //�м���������
        /// <summary>
        /// �м���������
        /// </summary>
        public ILGenerator IL
        {
            get { return il; }
        }

        //�����ļ���
        /// <summary>
        /// �����ļ���
        /// </summary>
        public Dictionary<string, ILObjectBase> Builders
        {
            get { return builders; }
            set { builders = value; }
        }

        //��������
        /// <summary>
        /// ��������
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
            throw new Exception("�޷�����鳤���Ǹ�����...");
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

        //����һ��������ִ�и÷�����ί��
        /// <summary>
        /// ��ɶ�̬����������һ��������ִ�и÷�����ί�С�
        /// <para>���ؽ��:
        /// </para>   һ��ָ�����͵�ί�У�������ִ�ж�̬������
        /// </summary>
        /// <param name="delegateType">һ��ǩ���붯̬������ǩ��ƥ���ί�����͡�</param>
        public Delegate CreateDelegate(Type delegateType)
        {

            Delegate r = this.ddmethod.CreateDelegate(delegateType);
            Dispose();  //���ٶ�������
            return r;
        }

        //�½�����ʵ��
        /// <summary>
        /// �½�����ʵ��
        /// </summary>
        /// <param name="objType">��������</param>
        /// <param name="Parameters">���캯������</param>
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
                //throw new InvalidCastException("ת��ʧ��!");
            }
            else if (oldType == typeof(object))
            {
                this.IL.Emit(OpCodes.Unbox_Any, convType);
            }
        }
    }
}
