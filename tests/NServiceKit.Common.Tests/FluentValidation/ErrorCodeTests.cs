using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.FluentValidation.Validators;
using NServiceKit.FluentValidation;
using NServiceKit.FluentValidation.Results;

namespace NServiceKit.Common.Tests.FluentValidation
{
    /// <summary>An error code tests.</summary>
    [TestFixture]
    public class ErrorCodeTests
    {
        /// <summary>A person.</summary>
        public class Person
        {
            /// <summary>Gets or sets the firstname.</summary>
            ///
            /// <value>The firstname.</value>
            public string Firstname { get; set; }

            /// <summary>Gets or sets the credit card.</summary>
            ///
            /// <value>The credit card.</value>
            public string CreditCard { get; set; }

            /// <summary>Gets or sets the age.</summary>
            ///
            /// <value>The age.</value>
            public int Age { get; set; }

            /// <summary>Gets or sets the cars.</summary>
            ///
            /// <value>The cars.</value>
            public List<Car> Cars { get; set; }

            /// <summary>Gets or sets the favorites.</summary>
            ///
            /// <value>The favorites.</value>
            public List<Car> Favorites { get; set; }

            /// <summary>Gets or sets the lastname.</summary>
            ///
            /// <value>The lastname.</value>
            public string Lastname { get; set; }
        }

        /// <summary>A car.</summary>
        public class Car
        {
            /// <summary>Gets or sets the manufacturer.</summary>
            ///
            /// <value>The manufacturer.</value>
            public string Manufacturer { get; set; }

            /// <summary>Gets or sets the age.</summary>
            ///
            /// <value>The age.</value>
            public int Age { get; set; }
        }

        /// <summary>A person validator.</summary>
        public class PersonValidator : AbstractValidator<Person>
        {
            /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.FluentValidation.ErrorCodeTests.PersonValidator class.</summary>
            public PersonValidator()
            {
                RuleFor(x => x.Firstname).Matches("asdfj");

                RuleFor(x => x.CreditCard).CreditCard().Length(10).EmailAddress().Equal("537")
                    .ExclusiveBetween("asdlöfjasdf", "asldfjlöakjdfsadf");

                RuleFor(x => x.Age).GreaterThan(100).GreaterThanOrEqualTo(100).InclusiveBetween(100, 200).LessThan(10);

                RuleFor(x => x.Cars).SetCollectionValidator(new CarValidator());
                RuleFor(x => x.Favorites).NotNull().NotEmpty().WithErrorCode("ShouldNotBeEmpty");

                RuleFor(x => x.Lastname).NotEmpty();
            }
        }

        /// <summary>A car validator.</summary>
        public class CarValidator : AbstractValidator<Car>
        {
            /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.FluentValidation.ErrorCodeTests.CarValidator class.</summary>
            public CarValidator()
            {
                RuleFor(x => x.Age).LessThanOrEqualTo(20).NotEqual(100);
                RuleFor(x => x.Manufacturer).Must(m => m == "BMW");
            }
        }

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public ValidationResult Result { get; set; }

        /// <summary>Sets the up.</summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            var person = new Person()
            {
                Firstname = "max",
                CreditCard = "1asdf2",
                Age = 10,
                Cars = new List<Car>()
                {
                    new Car() { Manufacturer = "Audi", Age = 100 }
                }
            };

            var validator = new PersonValidator();
            this.Result = validator.Validate(person);
        }

        /// <summary>Credit card.</summary>
        [Test]
        public void CreditCard()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.CreditCard));
        }

        /// <summary>Emails this object.</summary>
        [Test]
        public void Email()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.Email));
        }

        /// <summary>Equals this object.</summary>
        [Test]
        public void Equal()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.Equal));
        }

        /// <summary>Exclusive between.</summary>
        [Test]
        public void ExclusiveBetween()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.ExclusiveBetween));
        }

        /// <summary>Greater than.</summary>
        [Test]
        public void GreaterThan()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.GreaterThan));
        }

        /// <summary>Greater than or equal.</summary>
        [Test]
        public void GreaterThanOrEqual()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.GreaterThanOrEqual));
        }

        /// <summary>Inclusive between.</summary>
        [Test]
        public void InclusiveBetween()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.InclusiveBetween));
        }

        /// <summary>Lengths this object.</summary>
        [Test]
        public void Length()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.Length));
        }

        /// <summary>Less than.</summary>
        [Test]
        public void LessThan()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.LessThan));
        }

        /// <summary>Less than or equal.</summary>
        [Test]
        public void LessThanOrEqual()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.LessThanOrEqual));
        }

        /// <summary>Not empty.</summary>
        [Test]
        public void NotEmpty()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.NotEmpty));
        }

        /// <summary>Not equal.</summary>
        [Test]
        public void NotEqual()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.NotEqual));
        }

        /// <summary>Not null.</summary>
        [Test]
        public void NotNull()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.NotNull));
        }

        /// <summary>Predicates this object.</summary>
        [Test]
        public void Predicate()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.Predicate));
        }

        /// <summary>Regular expression.</summary>
        [Test]
        public void RegularExpression()
        {
            Assert.IsTrue(Result.Errors.Any(f => f.ErrorCode == ValidationErrors.RegularExpression));
        }

        /// <summary>Customs this object.</summary>
        [Test]
        public void Custom()
        {
            Assert.AreEqual(1, Result.Errors.Where(f => f.ErrorCode == "ShouldNotBeEmpty").Count());
        }
    }
}
