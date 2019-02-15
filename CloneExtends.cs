using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class TransExpV2<TIn, TOut>
    {

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                var tmpProperty = typeof(TIn).GetProperty(item.Name);
                if(tmpProperty!=null)
                {
                    MemberExpression property = Expression.Property(parameterExpression, tmpProperty);
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
               
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }

    }
    public static class CloneExtends
    {
        public static T DeepCloneObject<T>(this T t) where T : class
        {
            return TransExpV2<T, T>.Trans(t);
        }
        public static IList<T> DeepCloneList<T>(this IList<T> tList) where T : class
        {
            IList<T> listNew = new List<T>();
            foreach (var item in tList)
            {
                listNew.Add(TransExpV2<T, T>.Trans(item));
            }
            return listNew;
        }
        /// <summary>  
        /// Copy Propertys and Fileds   
        /// 拷贝属性和公共字段  
        /// </summary>  
        /// <typeparam name="T"> </typeparam>  
        /// <param name="source"></param>  
        /// <param name="target"></param>  
        public static T1 CopyToAll<T, T1>(this T source) where T : class
        {
            return TransExpV2<T, T1>.Trans(source);
        }
    }
}
