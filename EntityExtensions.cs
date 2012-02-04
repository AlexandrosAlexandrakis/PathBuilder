// property path builder, fluent style using expression trees
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Project.Domain
{

    public static class EntityExtensions
    {
        public static IQueryable<T> PrintQueryable<T>(this IQueryable<T> objQuery)
        {
            var query = objQuery as ObjectQuery;
            if(query != null)
            {
                Debug.WriteLine(query.ToTraceString());
            }
            return objQuery;
        }

    }

    public class PathBuilder<T>
    {
        private readonly IList<String> pathElements;

        private PathBuilder()
            : this(new List<String>())
        {
        }
        private PathBuilder(IList<String> pathElements)
        {
            this.pathElements = pathElements;
        }
        private String ParseExpression(Expression expression)
        {
            var body = (expression as LambdaExpression).Body;

            var memberExpr = body as MemberExpression;
            if(memberExpr != null)
            {
                return memberExpr.Member.Name;
            }
            else
            {
                throw new ArgumentException("expression");
            }
        }
        public String BuildPath()
        {
            return String.Join(".", pathElements);
        }

        public static PathBuilder<TProperty> Create<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return new PathBuilder<T>().Load(expression);
        }
        public static PathBuilder<TCollection> Create<TCollection>(Expression<Func<T, ICollection<TCollection>>> expression)
        {
            return new PathBuilder<T>().Load(expression);
        }


        public PathBuilder<TProperty> Load<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            pathElements.Add(ParseExpression(expression));
            return new PathBuilder<TProperty>(pathElements);
        }
        public PathBuilder<TCollection> Load<TCollection>(Expression<Func<T, ICollection<TCollection>>> expression)
        {
            pathElements.Add(ParseExpression(expression));
            return new PathBuilder<TCollection>(pathElements);
        }


        
    }    
}
