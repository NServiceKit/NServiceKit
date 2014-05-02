using System;
using System.Collections.Generic;
using NServiceKit.DesignPatterns.Translator;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A translator extensions.</summary>
    [Obsolete("Use ConvertAll")]
    public static class TranslatorExtensions
    {
        /// <summary>Methods.</summary>
        ///
        /// <typeparam name="To">  Type of to.</typeparam>
        /// <typeparam name="From">Type of from.</typeparam>
        /// <param name="translator">The translator to act on.</param>
        /// <param name="from">      Source for the.</param>
        ///
        /// <returns>A List&lt;To&gt;</returns>
        public static List<To> ParseAll<To, From>(this ITranslator<To, From> translator, IEnumerable<From> from)
        {
            var list = new List<To>();
            if (from != null)
            {
                foreach (var local in from)
                {
                    list.Add(translator.Parse(local));
                }
            }
            return list;
        }
    }


}