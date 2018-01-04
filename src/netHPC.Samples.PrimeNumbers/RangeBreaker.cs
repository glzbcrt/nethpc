
using netHPC.SDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.Samples.PrimeNumbers
{
    public class RangeBreaker : IWorkBreaker<Range>
    {
        #region Fields

        private UInt64 m_startValue;
        private UInt64 m_finishValue;
        private Byte m_workItemSize;
        private UInt32 m_totalSelectedNodes;
        private UInt32 m_totalSelectedExecutionUnits;
        private Boolean m_finish = false;

        private UInt64 m_stepSize;

        #endregion

        #region Load(string executionParameters, uint totalSelectedNodes, uint totalSelectedExecutionUnits)
        public void Load(string executionParameters, uint totalSelectedNodes, uint totalSelectedExecutionUnits)
        {            
            String[] arrTmp = executionParameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            m_startValue = UInt64.Parse(arrTmp[0]) - 1;
            m_finishValue = UInt64.Parse(arrTmp[1]);

            m_workItemSize = (Byte)Math.Pow(3 * (Byte.Parse(arrTmp[2]) + 1), Byte.Parse(arrTmp[2]));

            m_totalSelectedNodes = totalSelectedNodes;
            m_totalSelectedExecutionUnits = totalSelectedExecutionUnits;

            m_stepSize = (m_finishValue - m_startValue) / (m_totalSelectedExecutionUnits * m_workItemSize);
        } 
        #endregion

        #region GetWorkItem(out Range workItem)
        public Boolean GetWorkItem(out Range workItem)
        {
            workItem = null;

            if ((m_finish == true) || (m_startValue >= m_finishValue))
                return false;
            
            if (m_totalSelectedExecutionUnits == 1)
            {
                workItem = new Range(m_startValue, m_finishValue);
                m_finish = true;
                return true;
            }
            else
            {
                workItem = new Range(m_startValue + 1, 0);

                m_startValue = (m_startValue + 1) + m_stepSize;
                workItem.Finish = m_startValue;

                return true;
            }
        } 
        #endregion

        #region Unload()
        public void Unload()
        {
        } 
        #endregion

        #region Abort()
        public void Abort()
        {            
        } 
        #endregion
    }
}
