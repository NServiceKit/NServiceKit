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

namespace NServiceKit.FluentValidation.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if !SILVERLIGHT
    [Serializable]
#endif
    public class ValidationResult {
        private readonly List<ValidationFailure> errors = new List<ValidationFailure>();

        /// <summary>Gets a value indicating whether this object is valid.</summary>
        ///
        /// <value>true if this object is valid, false if not.</value>
        public bool IsValid {
            get { return Errors.Count == 0; }
        }

        /// <summary>Gets the errors.</summary>
        ///
        /// <value>The errors.</value>
        public IList<ValidationFailure> Errors {
            get { return errors; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Results.ValidationResult class.</summary>
        public ValidationResult() {
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Results.ValidationResult class.</summary>
        ///
        /// <param name="failures">The failures.</param>
        public ValidationResult(IEnumerable<ValidationFailure> failures) {
            errors.AddRange(failures.Where(failure => failure != null));
        }
    }
}