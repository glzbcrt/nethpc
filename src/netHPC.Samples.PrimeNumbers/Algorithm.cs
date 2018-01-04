
using netHPC.SDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netHPC.Samples.PrimeNumbers
{
    public class Algorithm : AlgorithmBase<Range>
    {
        protected override void Load()
        {
            base.Load();
        }

        protected override void Process()
        {
            StringBuilder stringBuilder;
            Range range = null;
            Boolean prime = true;

            while (GetWorkItem(out range))
            {
                stringBuilder = new StringBuilder(2000);
                stringBuilder.AppendLine(" Start: " + range.Start.ToString());
                stringBuilder.AppendLine("Finish: " + range.Finish.ToString());

                for (UInt64 tmpCounter = range.Start; tmpCounter <= range.Finish; tmpCounter++)
                {
                    if ((tmpCounter % 2 == 0) || (tmpCounter % 5 == 0))
                        continue;

                    prime = true;

                    for (UInt64 i = 3; i < tmpCounter; i++)
                    {
                        if (tmpCounter % i == 0)
                        {
                            prime = false;
                            break;
                        }
                    }

                    if (prime)
                        stringBuilder.AppendLine(tmpCounter.ToString());
                }

                ReportWorkItemResult(stringBuilder.ToString());
            }
        }

        protected override void Unload()
        {
            base.Unload();
        }
    }
}

