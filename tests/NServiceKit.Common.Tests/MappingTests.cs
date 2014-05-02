using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests
{
    /// <summary>An user.</summary>
    public class User
    {
        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>Gets or sets the car.</summary>
        ///
        /// <value>The car.</value>
        public Car Car { get; set; }
    }

    /// <summary>A user fields.</summary>
    public class UserFields
    {
        /// <summary>The person's first name.</summary>
        public string FirstName;
        /// <summary>The person's last name.</summary>
        public string LastName;
        /// <summary>The car.</summary>
        public Car Car;
    }

    /// <summary>A sub user.</summary>
    public class SubUser : User { }
    /// <summary>A sub user fields.</summary>
    public class SubUserFields : UserFields { }

    /// <summary>A car.</summary>
    public class Car
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        public int Age { get; set; }
    }

    /// <summary>A user dto.</summary>
    public class UserDto
    {
        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the car.</summary>
        ///
        /// <value>The car.</value>
        public string Car { get; set; }
    }

    /// <summary>Values that represent Color.</summary>
    public enum Color
    {
        /// <summary>An enum constant representing the red option.</summary>
        Red,

        /// <summary>An enum constant representing the green option.</summary>
        Green,

        /// <summary>An enum constant representing the blue option.</summary>
        Blue
    }

    /// <summary>Values that represent OtherColor.</summary>
    public enum OtherColor
    {
        /// <summary>An enum constant representing the red option.</summary>
        Red,

        /// <summary>An enum constant representing the green option.</summary>
        Green,

        /// <summary>An enum constant representing the blue option.</summary>
        Blue
    }


    /// <summary>An int nullable identifier.</summary>
    public class IntNullableId
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int? Id { get; set; }
    }

    /// <summary>An int identifier.</summary>
    public class IntId
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>A bcl types.</summary>
    public class BclTypes
    {
        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The int.</value>
        public int Int { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
        public long Long { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
        public double Double { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The decimal.</value>
        public decimal Decimal { get; set; }
    }

    /// <summary>A bcl type strings.</summary>
    public class BclTypeStrings
    {
        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The int.</value>
        public string Int { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
        public string Long { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
        public string Double { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The decimal.</value>
        public string Decimal { get; set; }
    }

    /// <summary>A nullable conversion.</summary>
    public class NullableConversion
    {
        /// <summary>Gets or sets the amount.</summary>
        ///
        /// <value>The amount.</value>
        public decimal Amount { get; set; }
    }

    /// <summary>A nullable conversion dto.</summary>
    public class NullableConversionDto
    {
        /// <summary>Gets or sets the amount.</summary>
        ///
        /// <value>The amount.</value>
        public decimal? Amount { get; set; }
    }

    /// <summary>A nullable enum conversion.</summary>
    public class NullableEnumConversion
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public Color Color { get; set; }
    }

    /// <summary>An enum conversion.</summary>
    public class EnumConversion
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public Color Color { get; set; }
    }

    /// <summary>A nullable enum conversion dto.</summary>
    public class NullableEnumConversionDto
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public OtherColor? Color { get; set; }
    }

    /// <summary>An enum conversion dto.</summary>
    public class EnumConversionDto
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public OtherColor Color { get; set; }
    }

    /// <summary>An enum conversion string dto.</summary>
    public class EnumConversionStringDto
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public string Color { get; set; }
    }

    /// <summary>An enum conversion int dto.</summary>
    public class EnumConversionIntDto
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        public int Color { get; set; }
    }

    /// <summary>A mapping tests.</summary>
    [TestFixture]
    public class MappingTests
    {
        /// <summary>Does populate.</summary>
        [Test]
        public void Does_populate()
        {
            var user = new User() {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car() { Name = "BMW X6", Age = 3 }
            };

            var userDto = new UserDto().PopulateWith(user);

            Assert.That(userDto.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(userDto.LastName, Is.EqualTo(user.LastName));
            Assert.That(userDto.Car, Is.EqualTo("{Name:BMW X6,Age:3}"));
        }

        /// <summary>Does translate.</summary>
        [Test]
        public void Does_translate()
        {
            var user = new User() {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car() { Name = "BMW X6", Age = 3 }
            };

            var userDto = user.TranslateTo<UserDto>();

            Assert.That(userDto.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(userDto.LastName, Is.EqualTo(user.LastName));
            Assert.That(userDto.Car, Is.EqualTo("{Name:BMW X6,Age:3}"));
        }

        /// <summary>Does enumstringconversion translate.</summary>
        [Test]
        public void Does_enumstringconversion_translate()
        {
            var conversion = new EnumConversion { Color = Color.Blue };
            var conversionDto = conversion.TranslateTo<EnumConversionStringDto>();

            Assert.That(conversionDto.Color, Is.EqualTo("Blue"));
        }

        /// <summary>Does enumintconversion translate.</summary>
        [Test]
        public void Does_enumintconversion_translate()
        {
            var conversion = new EnumConversion { Color = Color.Green };
            var conversionDto = conversion.TranslateTo<EnumConversionIntDto>();

            Assert.That(conversionDto.Color, Is.EqualTo(1));
        }

        /// <summary>Does nullableconversion translate.</summary>
        [Test]
        public void Does_nullableconversion_translate()
        {
            var conversion = new NullableConversion { Amount = 123.45m };
            var conversionDto = conversion.TranslateTo<NullableConversionDto>();

            Assert.That(conversionDto.Amount, Is.EqualTo(123.45m));
        }

        /// <summary>Does enumnullableconversion translate.</summary>
        [Test]
        public void Does_Enumnullableconversion_translate()
        {
            var conversion = new NullableEnumConversion { Color = Color.Green };
            var conversionDto = conversion.TranslateTo<NullableEnumConversionDto>();

            Assert.That(conversionDto.Color, Is.EqualTo(OtherColor.Green));

        }

        /// <summary>Does enumconversion translate.</summary>
        [Test]
        public void Does_Enumconversion_translate()
        {
            var conversion = new NullableEnumConversion { Color = Color.Green };
            var conversionDto = conversion.TranslateTo<EnumConversionDto>();

            Assert.That(conversionDto.Color, Is.EqualTo(OtherColor.Green));

        }

        /// <summary>Does translate nullable int to and from.</summary>
        [Test]
        public void Does_translate_nullableInt_to_and_from()
        {
            var nullable = new IntNullableId();

            var nonNullable = nullable.TranslateTo<IntId>();

            nonNullable.Id = 10;

            var expectedNullable = nonNullable.TranslateTo<IntNullableId>();

            Assert.That(expectedNullable.Id.Value, Is.EqualTo(nonNullable.Id));
        }

        /// <summary>Does translate from properties to fields.</summary>
        [Test]
        public void Does_translate_from_properties_to_fields()
        {
            var user = new User {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car { Name = "BMW X6", Age = 3 }
            };

            var to = user.TranslateTo<UserFields>();
            Assert.That(to.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(to.LastName, Is.EqualTo(user.LastName));
            Assert.That(to.Car.Name, Is.EqualTo(user.Car.Name));
            Assert.That(to.Car.Age, Is.EqualTo(user.Car.Age));
        }

        /// <summary>Does translate from fields to properties.</summary>
        [Test]
        public void Does_translate_from_fields_to_properties()
        {
            var user = new UserFields {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car { Name = "BMW X6", Age = 3 }
            };

            var to = user.TranslateTo<UserFields>();
            Assert.That(to.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(to.LastName, Is.EqualTo(user.LastName));
            Assert.That(to.Car.Name, Is.EqualTo(user.Car.Name));
            Assert.That(to.Car.Age, Is.EqualTo(user.Car.Age));
        }

        /// <summary>Does translate from inherited propeties.</summary>
        [Test]
        public void Does_translate_from_inherited_propeties()
        {
            var user = new SubUser {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car { Name = "BMW X6", Age = 3 }
            };

            var to = user.TranslateTo<UserFields>();
            Assert.That(to.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(to.LastName, Is.EqualTo(user.LastName));
            Assert.That(to.Car.Name, Is.EqualTo(user.Car.Name));
            Assert.That(to.Car.Age, Is.EqualTo(user.Car.Age));
        }

        /// <summary>Does translate to inherited propeties.</summary>
        [Test]
        public void Does_translate_to_inherited_propeties()
        {
            var user = new User {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car { Name = "BMW X6", Age = 3 }
            };

            var to = user.TranslateTo<SubUserFields>();
            Assert.That(to.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(to.LastName, Is.EqualTo(user.LastName));
            Assert.That(to.Car.Name, Is.EqualTo(user.Car.Name));
            Assert.That(to.Car.Age, Is.EqualTo(user.Car.Age));
        }

        /// <summary>Does coerce from bcl types to strings.</summary>
        [Test]
        public void Does_coerce_from_BclTypes_to_strings()
        {
            var from = new BclTypes {
                Int = 1,
                Long = 2,
                Double = 3.3,
                Decimal = 4.4m,                
            };

            var to = from.TranslateTo<BclTypeStrings>();
            Assert.That(to.Int, Is.EqualTo("1"));
            Assert.That(to.Long, Is.EqualTo("2"));
            Assert.That(to.Double, Is.EqualTo("3.3"));
            Assert.That(to.Decimal, Is.EqualTo("4.4"));
        }

        /// <summary>Does coerce from strings to bcl types.</summary>
        [Test]
        public void Does_coerce_from_strings_to_BclTypes()
        {
            var from = new BclTypeStrings {
                Int = "1",
                Long = "2",
                Double = "3.3",
                Decimal = "4.4",
            };

            var to = from.TranslateTo<BclTypes>();
            Assert.That(to.Int, Is.EqualTo(1));
            Assert.That(to.Long, Is.EqualTo(2));
            Assert.That(to.Double, Is.EqualTo(3.3d));
            Assert.That(to.Decimal, Is.EqualTo(4.4m));
        }

        /// <summary>Does map only properties with specified attribute.</summary>
        [Test]
        public void Does_map_only_properties_with_specified_Attribute()
        {
            var user = new User {
                FirstName = "Demis",
                LastName = "Bellot",
                Car = new Car { Name = "BMW X6", Age = 3 }
            };

            var to = new User();
            to.PopulateFromPropertiesWithAttribute(user, typeof(DataMemberAttribute));

            Assert.That(to.LastName, Is.EqualTo(user.LastName));
            Assert.That(to.FirstName, Is.Null);
            Assert.That(to.Car, Is.Null);
        }
    }
}
