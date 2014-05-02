//
// NServiceKit
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack
//
// Licensed under the new BSD license.
//

using System.Runtime.Serialization;

namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>
    /// Error information pertaining to a particular named field.
    /// Used for returning multiple field validation errors.s
    /// </summary>
    [DataContract]
    public class ResponseError
    {
        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public string ErrorCode { get; set; }

        /// <summary>Gets or sets the name of the field.</summary>
        ///
        /// <value>The name of the field.</value>
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string FieldName { get; set; }

        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string Message { get; set; }
    }
}
