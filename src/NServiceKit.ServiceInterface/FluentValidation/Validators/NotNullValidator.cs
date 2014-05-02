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
    using Resources;

    /// <summary>A not null validator.</summary>
    public class NotNullValidator : PropertyValidator, INotNullValidator {
        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.NotNullValidator class.</summary>
        public NotNullValidator() : base(() => Messages.notnull_error, ValidationErrors.NotNull) {
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            if (context.PropertyValue == null) {
                return false;
            }
            return true;
        }
    }

    /// <summary>Interface for not null validator.</summary>
    public interface INotNullValidator : IPropertyValidator {
    }
}