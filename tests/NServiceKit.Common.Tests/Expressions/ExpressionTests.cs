using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using System.Diagnostics;

namespace NServiceKit.Common.Tests.Expressions
{
    /// <summary>An expression tests.</summary>
	[TestFixture]
	public class ExpressionTests
	{
        /// <summary>Adds a method.</summary>
        ///
        /// <param name="a">The int to process.</param>
        ///
        /// <returns>An int.</returns>
		public int AddMethod(int a)
		{
			return a + 4;
		}

        /// <summary>Simple function and equivalent expression tests.</summary>
		[Test]
		public void Simple_func_and_equivalent_expression_tests()
		{
			Func<int, int> add = x => x + x;

			Assert.That(add(4), Is.EqualTo(4 + 4));

			Expression<Func<int, int>> addExpr = x => x + 4;

			Func<int, int> addFromExpr = addExpr.Compile();

			Assert.That(addFromExpr(4), Is.EqualTo(add(4)));

			Func<int, int> addMethod = AddMethod;

			Assert.That(addMethod(4), Is.EqualTo(add(4)));

			Expression<Func<int, int>> callAddMethodExpr = x => AddMethod(x);
			var addMethodCall = (MethodCallExpression) callAddMethodExpr.Body;
			Assert.That(addMethodCall.Method.Name, Is.EqualTo("AddMethod"));
		}

        /// <summary>Simple function timing tests.</summary>
		[Test]
		public void Simple_func_timing_tests()
		{
                // 1/5 as expensive as expression
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			Func<int, int> add = x => x + x;

			Assert.That(add(4), Is.EqualTo(4 + 4));
			stopWatch.Stop();
			Console.WriteLine("Delegate took: {0}ms", stopWatch.ElapsedMilliseconds);
		}

        /// <summary>Simple expression timing tests.</summary>
		[Test]
		public void Simple_expression_timing_tests()
		{
             	//5 times more expensive than Func
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			Expression<Func<int, int>> addExpr = x => x + 4;

			Func<int, int> addFromExpr = addExpr.Compile();

			Assert.That(addFromExpr(4), Is.EqualTo(4 + 4));
			stopWatch.Stop();
			Console.WriteLine("Delegate took: {0}ms", stopWatch.ElapsedMilliseconds);
		}

        /// <summary>Static add.</summary>
        ///
        /// <param name="a">The int to process.</param>
        ///
        /// <returns>An int.</returns>
		public static int StaticAdd(int a)
		{
			return a + 4;
		}

        /// <summary>Method call expression to call a static method.</summary>
		[Test]
		public void MethodCallExpression_to_call_a_static_method()
		{
			//Expression<Func<int, int>> callAddMethodExpr = Expression.Lambda<Func<int, int>>(Expression.Call(null, (MethodInfo) methodof(ExpressionTests.StaticAdd), 
			//new Expression[] { CS$0$0000 = Expression.Parameter(typeof(int), "x") }), new ParameterExpression[] { CS$0$0000 });

			Expression<Func<int, int>> callAddMethodExpr = x => StaticAdd(x);
			var addMethodCall = (MethodCallExpression)callAddMethodExpr.Body;
			Assert.That(addMethodCall.Method.Name, Is.EqualTo("StaticAdd"));
		}

        /// <summary>Dynamic method call expression to call a static method.</summary>
		[Test]
		public void Dynamic_MethodCallExpression_to_call_a_static_method()
		{

			//MethodCallExpression.Call(Expression.Call(GetType().GetMethod("StaticAdd", BindingFlags.Static | BindingFlags.Public));
			
			//Assert.That(addMethodCall.Method.Name, Is.EqualTo("StaticAdd"));
		}
	}
}
