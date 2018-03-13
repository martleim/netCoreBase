using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GenericAirways.Contracts
{
    public interface IDbContext <T> where T : class
    {

    }


}
