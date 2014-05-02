namespace NServiceKit.Razor.Compilation
{
    /// <summary>Interface for razor host.</summary>
    public interface IRazorHost
    {
        /// <summary>Gets or sets the default namespace.</summary>
        ///
        /// <value>The default namespace.</value>
        string DefaultNamespace { get; set; }

        /// <summary>Gets or sets a value indicating whether the line pragmas is enabled.</summary>
        ///
        /// <value>true if enable line pragmas, false if not.</value>
        bool EnableLinePragmas { get; set; }

        //event EventHandler<GeneratorErrorEventArgs> Error;

        //event EventHandler<ProgressEventArgs> Progress;

        //string GenerateCode();
    }
}
