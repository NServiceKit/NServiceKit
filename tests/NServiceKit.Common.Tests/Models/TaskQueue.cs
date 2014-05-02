using System;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>Queue of tasks.</summary>
	public class TaskQueue
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(TaskQueue));

        /// <summary>The task load.</summary>
		public const string TaskLoad = "Load";
        /// <summary>Zero-based index of the task.</summary>
		public const string TaskIndex = "Index";

        /// <summary>The status pending.</summary>
		public const string StatusPending = "Pending";
        /// <summary>The status started.</summary>
		public const string StatusStarted = "Started";
        /// <summary>The status completed.</summary>
		public const string StatusCompleted = "Completed";
        /// <summary>The status failed.</summary>
		public const string StatusFailed = "Failed";

        /// <summary>The priority low.</summary>
		public const int PriorityLow = 0;
        /// <summary>The priority medium.</summary>
		public const int PriorityMedium = 1;
        /// <summary>The priority high.</summary>
		public const int PriorityHigh = 2;

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
		public Guid? UserId { get; set; }

        /// <summary>Gets or sets the task.</summary>
        ///
        /// <value>The task.</value>
		public string Task { get; set; }

        /// <summary>Gets or sets the content URN.</summary>
        ///
        /// <value>The content URN.</value>
		public string ContentUrn { get; set; }

        /// <summary>Gets or sets the status.</summary>
        ///
        /// <value>The status.</value>
		public string Status { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the priority.</summary>
        ///
        /// <value>The priority.</value>
		public int Priority { get; set; }

        /// <summary>Gets or sets the no of attempts.</summary>
        ///
        /// <value>The no of attempts.</value>
		public int NoOfAttempts { get; set; }

        /// <summary>Gets or sets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
		public string ErrorMessage { get; set; }

        /// <summary>Creates a new TaskQueue.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>A TaskQueue.</returns>
		public static TaskQueue Create(int id)
		{
			return new TaskQueue {
				ContentUrn = "urn:track:" + id,
				CreatedDate = DateTime.Now,
				Task = TaskLoad,
				Status = StatusPending,
				NoOfAttempts = 0,
			};
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(TaskQueue actual, TaskQueue expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.UserId, Is.EqualTo(expected.UserId));
			Assert.That(actual.ContentUrn, Is.EqualTo(expected.ContentUrn));
			Assert.That(actual.Status, Is.EqualTo(expected.Status));
			try
			{
				Assert.That(actual.CreatedDate, Is.EqualTo(expected.CreatedDate));
			}
			catch (Exception ex)
			{
				Log.Error("Trouble with DateTime precisions, trying Assert again with rounding to seconds", ex);
				Assert.That(actual.CreatedDate.RoundToSecond(), Is.EqualTo(expected.CreatedDate.RoundToSecond()));
			}
			Assert.That(actual.Priority, Is.EqualTo(expected.Priority));
			Assert.That(actual.NoOfAttempts, Is.EqualTo(expected.NoOfAttempts));
			Assert.That(actual.ErrorMessage, Is.EqualTo(expected.ErrorMessage));
		}

	}
}