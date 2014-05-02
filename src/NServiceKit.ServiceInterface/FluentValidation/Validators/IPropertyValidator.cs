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

namespace NServiceKit.FluentValidation.Validators
{
    using System;
    using System.Collections.Generic;
    using Resources;
    using Results;

    /// <summary>
    /// A custom property validator.
    /// This interface should not be implemented directly in your code as it is subject to change.
    /// Please inherit from <see cref="PropertyValidator">PropertyValidator</see> instead.
    /// </summary>
    public interface IPropertyValidator
    {
        /// <summary>Enumerates validate in this collection.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process validate in this collection.</returns>
        IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context);

        /// <summary>Gets the custom message format arguments.</summary>
        ///
        /// <value>The custom message format arguments.</value>
        ICollection<Func<object, object>> CustomMessageFormatArguments { get; }

        /// <summary>Gets or sets the custom state provider.</summary>
        ///
        /// <value>The custom state provider.</value>
        Func<object, object> CustomStateProvider { get; set; }

        /// <summary>Gets or sets the error message source.</summary>
        ///
        /// <value>The error message source.</value>
        IStringSource ErrorMessageSource { get; set; }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
        string ErrorCode { get; set; }
    }
}