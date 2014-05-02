#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace NServiceKit.FluentValidation.Internal
{
    using System;
    using System.ComponentModel;

    //From Kzu's blog: http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluentInterface {

        /// <summary>Gets the type.</summary>
        ///
        /// <returns>The type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>Returns a hash code for this object.</summary>
        ///
        /// <returns>A hash code for this object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>Convert this object into a string representation.</summary>
        ///
        /// <returns>A string that represents this object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>Tests if this object is considered equal to another.</summary>
        ///
        /// <param name="obj">The object to compare to this object.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
    }
}