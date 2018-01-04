using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.SDK
{
    /// <summary>
    /// The interface that the configuration dialog must implement.
    /// </summary>
    public interface IConfigurationDialog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Boolean ValidateFieldsOnScreen();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        String GetParameters();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        String GetSummaryText();
    }
}
