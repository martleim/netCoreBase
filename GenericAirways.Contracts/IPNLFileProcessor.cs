using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAirways.Contracts
{
    public interface IPNLProcessor<L, NL, CL>
    {
        L ProcessPNL(StringReader list);
        NL ProcessNameLine(string line);
        CL ProcessCodeLine(string line);
        bool CheckNameLine(string line);
        bool CheckCodeLine(string line);

    }

}
