using System;

namespace DurableFunction.FunctionChaining.Client.Models
{
    /// <summary>
    /// Employee
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the doj.
        /// </summary>
        /// <value>
        /// The doj.
        /// </value>
        public DateTime DOJ { get; set; }
    }
}
