using System;

namespace NServiceKit.Razor.Compilation
{
    /// <summary>Additional information for generator error events.</summary>
    public class GeneratorErrorEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.GeneratorErrorEventArgs class.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">A message describing the error.</param>
        /// <param name="lineNumber">  The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        public GeneratorErrorEventArgs(uint errorCode, string errorMessage, uint lineNumber, uint columnNumber)
        {
            ErorrCode = errorCode;
            ErrorMessage = errorMessage;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>Gets the erorr code.</summary>
        ///
        /// <value>The erorr code.</value>
        public uint ErorrCode { get; private set; }

        /// <summary>Gets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
        public string ErrorMessage { get; private set; }

        /// <summary>Gets the line number.</summary>
        ///
        /// <value>The line number.</value>
        public uint LineNumber { get; private set; }

        /// <summary>Gets the column number.</summary>
        ///
        /// <value>The column number.</value>
        public uint ColumnNumber { get; private set; }
    }

    /// <summary>Additional information for progress events.</summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.ProgressEventArgs class.</summary>
        ///
        /// <param name="completed">The completed.</param>
        /// <param name="total">    The total.</param>
        public ProgressEventArgs(uint completed, uint total)
        {
            Completed = completed;
            Total = total;
        }

        /// <summary>Gets the completed.</summary>
        ///
        /// <value>The completed.</value>
        public uint Completed { get; private set; }

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The total.</value>
        public uint Total { get; private set; }
    }
}
