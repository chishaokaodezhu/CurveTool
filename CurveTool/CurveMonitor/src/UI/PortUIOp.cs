using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurveMonitor.src.UI.PortPannel;

namespace CurveMonitor.src.UI
{
    public interface PortUIOp
    {
        bool UIOp(Object obj, OpType type);
    }
}
