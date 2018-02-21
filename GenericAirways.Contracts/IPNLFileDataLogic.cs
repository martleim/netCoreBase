using GenericAirways.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAirways.Contracts
{
    public interface IPNLFileDataLogic
    {
        IPNLFileRepository PNLFileRepository { get; set; }
        IRecordLocatorRepository RecordLocatorRepository { get; set; }
        IPassengerRecordRepository PassengerRecordRepository { get; set; }

        void SavePnlFile(PNLFile file);
        IList<PNLFile> GetAll();
        void Update(PNLFile item);
        void Remove(PNLFile item);
    }
}
