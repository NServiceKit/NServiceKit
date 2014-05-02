using System;
using NUnit.Framework;
using NServiceKit.Common.Tests.Models;
using NServiceKit.Common.Utils;
using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Common.Tests
{
    /// <summary>An identifier utilities tests.</summary>
    [TestFixture]
    public class IdUtilsTests
    {
        private const int IntValue = 1;
        private const string StringValue = "A";

        /// <summary>The has int identifier.</summary>
        public class HasIntId : IHasIntId
        {
            /// <summary>Gets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public int Id
            {
                get { return IntValue; }
            }
        }

        /// <summary>The has generic identifier int.</summary>
        public class HasGenericIdInt : IHasId<int>
        {
            /// <summary>Gets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public int Id
            {
                get { return IntValue; }
            }
        }

        /// <summary>The has generic identifier string.</summary>
        public class HasGenericIdString : IHasId<string>
        {
            /// <summary>Gets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public string Id
            {
                get { return StringValue; }
            }
        }

        /// <summary>The has identifier property.</summary>
        public class HasIdProperty
        {
            /// <summary>Gets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public int Id
            {
                get { return IntValue; }
            }
        }

        /// <summary>The has identifier string property.</summary>
        public class HasIdStringProperty
        {
            /// <summary>Gets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public string Id
            {
                get { return StringValue; }
            }
        }

        /// <summary>The has identifier custom string property.</summary>
        public class HasIdCustomStringProperty
        {
            /// <summary>Gets the identifier of the custom.</summary>
            ///
            /// <value>The identifier of the custom.</value>
            public string CustomId
            {
                get { return StringValue; }
            }
        }

        /// <summary>The has identifier custom int property.</summary>
        public class HasIdCustomIntProperty
        {
            /// <summary>Gets the identifier of the custom.</summary>
            ///
            /// <value>The identifier of the custom.</value>
            public int CustomId
            {
                get { return IntValue; }
            }
        }

        /// <summary>Attribute for has primary key.</summary>
        public class HasPrimaryKeyAttribute
        {
            /// <summary>Gets the identifier of the custom.</summary>
            ///
            /// <value>The identifier of the custom.</value>
            [PrimaryKey]
            public int CustomId
            {
                get { return IntValue; }
            }
        }

        /// <summary>Can get if has int identifier.</summary>
        [Test]
        public void Can_get_if_HasIntId()
        {
            Assert.That(new HasIntId().GetId(), Is.EqualTo(IntValue));
        }

        /// <summary>Can get if has generic identifier int.</summary>
        [Test]
        public void Can_get_if_HasGenericIdInt()
        {
            Assert.That(new HasGenericIdInt().GetId(), Is.EqualTo(IntValue));
        }

        /// <summary>Can get if has generic identifier string.</summary>
        [Test]
        public void Can_get_if_HasGenericIdString()
        {
            Assert.That(new HasGenericIdString().GetId(), Is.EqualTo(StringValue));
        }

        /// <summary>Can get if has identifier property.</summary>
        [Test]
        public void Can_get_if_HasIdProperty()
        {
            Assert.That(new HasIdProperty().GetId(), Is.EqualTo(IntValue));
        }

        /// <summary>Can get if has identifier string property.</summary>
        [Test]
        public void Can_get_if_HasIdStringProperty()
        {
            Assert.That(new HasIdStringProperty().GetId(), Is.EqualTo(StringValue));
        }

        /// <summary>Can get if has identifier custom string property.</summary>
        [Test]
        public void Can_get_if_HasIdCustomStringProperty()
        {
            ModelConfig<HasIdCustomStringProperty>.Id(x => x.CustomId);

            Assert.That(new HasIdCustomStringProperty().GetId(), Is.EqualTo(StringValue));
        }

        /// <summary>Can get if has identifier custom int property.</summary>
        [Test]
        public void Can_get_if_HasIdCustomIntProperty()
        {
            ModelConfig<HasIdCustomIntProperty>.Id(x => x.CustomId);

            Assert.That(new HasIdCustomIntProperty().GetId(), Is.EqualTo(IntValue));
        }

        /// <summary>Can get if has primary key attribute.</summary>
        [Test]
        public void Can_get_if_HasPrimaryKeyAttribute()
        {
            Assert.That(new HasPrimaryKeyAttribute().GetId(), Is.EqualTo(IntValue));
        }

    }
}