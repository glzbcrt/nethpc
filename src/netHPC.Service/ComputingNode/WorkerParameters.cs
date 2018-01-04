using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.Service.ComputingNode
{
    class WorkerParameters
    {
        private Int32 m_algorithmId;
        private Int32 m_executionId;
        private Int32 m_execUnitId;

        internal WorkerParameters(Int32 algorithmId, Int32 executionId, Int32 execUnitId)
        {
            m_algorithmId = algorithmId;
            m_executionId = executionId;
            m_execUnitId = execUnitId;
        }

        internal Int32 AlgorithmId
        {
            get { return m_algorithmId; }
            set { m_algorithmId = value; }
        }

        internal Int32 ExecutionId
        {
            get { return m_executionId; }
            set { m_executionId = value; }
        }

        internal Int32 ExecUnitId
        {
            get { return m_execUnitId; }
            set { m_execUnitId = value; }
        }
    }
}
