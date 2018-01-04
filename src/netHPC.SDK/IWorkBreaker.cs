using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.SDK
{
    /// <summary>
    /// Defines the methods that a work breaker must implement to break the total work in work items to be distributed.
    /// </summary>
    /// <typeparam name="WorkItemType"></typeparam>
    public interface IWorkBreaker<WorkItemType>
    {
        /// <summary>
        /// Called to initialize the work breaker. This method is called only once on the beginning.
        /// </summary>
        /// <param name="executionParameters">A String representing the algorithm parameters provided on the screen.</param>
        /// <param name="totalSelectedNodes">The total number of selected nodes.</param>
        /// <param name="totalSelectedExecutionUnits">The total number of selected cores.</param>
        void Load(String executionParameters, UInt32 totalSelectedNodes, UInt32 totalSelectedExecutionUnits);

        /// <summary>
        /// Called to get an work item to send to the requesting node.
        /// </summary>
        /// <param name="workItem">An out object representing the work item to be processed.</param>
        /// <returns>True if there is an work item to process, otherwise false.</returns>
        Boolean GetWorkItem(out WorkItemType workItem);

        /// <summary>
        /// Called to unload the work breaker, freeing any resouce allocated to it.
        /// </summary>
        void Unload();

        /// <summary>
        /// Called when the execution is aborted from the netHPC console.
        /// </summary>
        void Abort();
    }
}