using DurableFunction.FunctionChaining.Client.Constants;

namespace DurableFunction.FunctionChaining.Client.Models
{
    /// <summary>
    /// EmployeeDetails
    /// </summary>
    /// <seealso cref="DurableFunction.FunctionChaining.Client.Models.Employee" />
    public class EmployeeDetails : Employee
    {
        /// <summary>
        /// Gets or sets a value indicating whether [on boarded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [on boarded]; otherwise, <c>false</c>.
        /// </value>
        public bool OnBoarded { get; set; }

        /// <summary>
        /// Gets or sets the domain allocated.
        /// </summary>
        /// <value>
        /// The domain allocated.
        /// </value>
        public string DomainAllocated { get; set; }

        /// <summary>
        /// Gets or sets the manager allocated.
        /// </summary>
        /// <value>
        /// The manager allocated.
        /// </value>
        public string ManagerAllocated { get; set; }

        /// <summary>
        /// Gets or sets the project allocated.
        /// </summary>
        /// <value>
        /// The project allocated.
        /// </value>
        public string ProjectAllocated { get; set; }
    }
}
