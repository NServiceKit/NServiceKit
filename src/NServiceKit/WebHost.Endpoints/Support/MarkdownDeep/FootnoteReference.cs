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
	class FootnoteReference
	{
        /// <summary>Initializes a new instance of the MarkdownDeep.FootnoteReference class.</summary>
        ///
        /// <param name="index">Zero-based index of the.</param>
        /// <param name="id">   The identifier.</param>
		public FootnoteReference(int index, string id)
		{
			this.index = index;
			this.id = id;
		}
        /// <summary>Zero-based index of the.</summary>
		public int index;
        /// <summary>The identifier.</summary>
		public string id;
	}
}
