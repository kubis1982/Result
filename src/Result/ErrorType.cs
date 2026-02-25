namespace Kubis1982.Result
{
    /// <summary>
    /// Defines the types of errors that can be represented by an <see cref="Error"/>.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Indicates no error (success state).
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates a resource was not found.
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// Indicates a conflict with the current state of a resource.
        /// </summary>
        Conflict = 2,

        /// <summary>
        /// Indicates access to a resource is forbidden.
        /// </summary>
        Forbidden = 3,

        /// <summary>
        /// Indicates authentication is required or has failed.
        /// </summary>
        Unauthorized = 4,

        /// <summary>
        /// Indicates validation of input data has failed.
        /// </summary>
        Validation = 5,

        /// <summary>
        /// Indicates an unexpected error has occurred.
        /// </summary>
        Unexpected = 6,
    }
}