namespace LaundrySystem.Domain.Model.Responses
{
    /// <summary>
    /// This class wraps the object in a property called Data. it has Success and Message as other properties, that is used to send back if the Data property has data in it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Placeholder type intended to be populated by a object or list of objects.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Set to false if property 'Data' has no data.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Error message set when Success is false, to give context of the problem.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}