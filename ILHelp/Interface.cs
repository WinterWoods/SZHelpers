/*
 * 作者:冰麟轻武
 * 
 * 有建议或者BUG可以联系:fttl_398@126.com,或者QQ21979018,或者http://t.qq.com/jy02305022
 * 所有提供建议或BUG者均加为好友,更新类库后第一时间发送给各位
 * 版本 1.0.0.0 日期:2010-4-16
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace ILHelp
{
    //提供一种对 Microsoft 中间语言 (MSIL) 指令中动态对象的值快速操作的机制
    /// <summary>
    /// 提供一种对 Microsoft 中间语言 (MSIL) 指令中动态对象的值快速操作的机制
    /// </summary>
    public interface IILValue
    {
        //获取此对象的类型
        /// <summary>
        /// 获取此对象的类型
        /// </summary>
        Type ValueType { get; }
        //将此对象值推送到堆栈上。
        /// <summary>
        /// 将此对象值推送到堆栈上。
        /// </summary>
        void LdValue();
        //将此对象的值存储到局部变量中并返回。
        /// <summary>
        /// 将此对象的值存储到局部变量中并返回。
        /// </summary>
        LocalBuilder StValue();
        //将此对象的值存储到'特定类型'的局部变量中并返回。
        /// <summary>
        /// 将此对象的值存储到'特定类型'的局部变量中并返回。
        /// </summary>
        LocalBuilder StValue(Type localType);
    }
}
