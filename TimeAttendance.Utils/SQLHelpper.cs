using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttendance.Utils
{
    public static class SQLHelpper
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, bool ordertype)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp;
            if (ordertype)
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            }
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> OrderBy2<T>(this IQueryable<T> source, string ordering, string ordering2, bool ordertype)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
         //   var property2 = type.GetProperty(ordering2);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
           // var propertyAccess2 = Expression.MakeMemberAccess(parameter, property2);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
          //  var orderByExp2 = Expression.Lambda(propertyAccess2, parameter);
            MethodCallExpression resultExp;
            if (ordertype)
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            }
            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
