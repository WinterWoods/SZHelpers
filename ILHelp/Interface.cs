/*
 * ����:��������
 * 
 * �н������BUG������ϵ:fttl_398@126.com,����QQ21979018,����http://t.qq.com/jy02305022
 * �����ṩ�����BUG�߾���Ϊ����,���������һʱ�䷢�͸���λ
 * �汾 1.0.0.0 ����:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace ILHelp
{
    //�ṩһ�ֶ� Microsoft �м����� (MSIL) ָ���ж�̬�����ֵ���ٲ����Ļ���
    /// <summary>
    /// �ṩһ�ֶ� Microsoft �м����� (MSIL) ָ���ж�̬�����ֵ���ٲ����Ļ���
    /// </summary>
    public interface IILValue
    {
        //��ȡ�˶��������
        /// <summary>
        /// ��ȡ�˶��������
        /// </summary>
        Type ValueType { get; }
        //���˶���ֵ���͵���ջ�ϡ�
        /// <summary>
        /// ���˶���ֵ���͵���ջ�ϡ�
        /// </summary>
        void LdValue();
        //���˶����ֵ�洢���ֲ������в����ء�
        /// <summary>
        /// ���˶����ֵ�洢���ֲ������в����ء�
        /// </summary>
        LocalBuilder StValue();
        //���˶����ֵ�洢��'�ض�����'�ľֲ������в����ء�
        /// <summary>
        /// ���˶����ֵ�洢��'�ض�����'�ľֲ������в����ء�
        /// </summary>
        LocalBuilder StValue(Type localType);
    }
}
