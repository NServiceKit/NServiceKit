// 
//   MarkdownDeep - http://www.toptensoftware.com/markdowndeep
//	 Copyright (C) 2010-2011 Topten Software
// 
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this product except in 
//   compliance with the License. You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License is 
//   distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//   See the License for the specific language governing permissions and limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownDeep
{
	class Abbreviation
	{
        /// <summary>Initializes a new instance of the MarkdownDeep.Abbreviation class.</summary>
        ///
        /// <param name="abbr"> The abbr.</param>
        /// <param name="title">The title.</param>
		public Abbreviation(string abbr, string title)
		{
			Abbr = abbr;
			Title = title;
		}
        /// <summary>The abbr.</summary>
		public string Abbr;
        /// <summary>The title.</summary>
		public string Title;
	}
}
