namespace NeoSharp.Application.Client
{
    public enum ConsoleOutputStyle : byte
    {
        /// <summary>
        /// Regular output
        /// </summary>
        Output,
        /// <summary>
        /// Prompt
        /// </summary>
        Prompt,
        /// <summary>
        /// Input
        /// </summary>
        Input,
        /// <summary>
        /// Information
        /// </summary>
        Information,
        /// <summary>
        /// Autocomplete
        /// </summary>
        Autocomplete,
        /// <summary>
        /// Autocomplete match
        /// </summary>
        AutocompleteMatch,
        /// <summary>
        /// Error
        /// </summary>
        Error
    }
}