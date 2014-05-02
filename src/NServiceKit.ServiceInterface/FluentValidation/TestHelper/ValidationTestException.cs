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

namespace NServiceKit.FluentValidation.TestHelper
{
    using System;

    /// <summary>Exception for signalling validation test errors.</summary>
    public class ValidationTestException : Exception {

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.TestHelper.ValidationTestException class.</summary>
        ///
        /// <param name="message">The message.</param>
        public ValidationTestException(string message) : base(message) {
        }
    }
}