using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.Samples.PrimeNumbers
{
    [Serializable]
    public class Range
    {
        private UInt64 m_start;
        private UInt64 m_finish;

        public Range()
        {
        }

        public Range(UInt64 start, UInt64 finish)
        {
            m_start = start;
            m_finish = finish;
        }

        public UInt64 Start
        {
            get { return m_start; }
            set { m_start = value; }
        }

        public UInt64 Finish
        {
            get { return m_finish; }
            set { m_finish = value; }
        }
    }
}
