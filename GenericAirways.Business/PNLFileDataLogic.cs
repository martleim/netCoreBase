using GenericAirways.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericAirways.Model;

namespace GenericAirways.Business
{
    public class PNLFileDataLogic : IPNLFileDataLogic
    {
        public IPNLFileRepository PNLFileRepository { get; set; }
        public IRecordLocatorRepository RecordLocatorRepository { get; set; }
        public IPassengerRecordRepository PassengerRecordRepository { get; set; }

        public PNLFileDataLogic(IPNLFileRepository pNLFileRepository,
            IRecordLocatorRepository recordLocatorRepository,
            IPassengerRecordRepository passengerRecordRepository)
        {

            PNLFileRepository = pNLFileRepository;
            /*RecordLocatorRepository = recordLocatorRepository;
            PassengerRecordRepository = passengerRecordRepository;*/
        }

        public void SavePnlFile(PNLFile file)
        {
            PNLFileRepository.Add(file);
            /*var recordLocator = file.RecordLocator;
            file.RecordLocator = null;
            PNLFileRepository.Add(file);
            recordLocator.ToList().ForEach(rl =>
            {
                rl.PNLFileId = file.Id;
                var passengerRecord = rl.PassengerRecord;
                rl.PassengerRecord = null;
                RecordLocatorRepository.Add(rl);
                passengerRecord.ToList().ForEach(pr =>
                {
                    pr.RecordLocator = null;
                    pr.RecordLocatorId = rl.Id;
                    PassengerRecordRepository.Add(pr);
                });
                rl.PassengerRecord = passengerRecord;
            });
            file.RecordLocator = recordLocator;*/
        }

        public IList<PNLFile> GetAll()
        {
            return PNLFileRepository.GetAll();
        }

        public void Update(PNLFile item)
        {
            PNLFileRepository.Update(item);
        }

        public void Remove(PNLFile item)
        {
            PNLFileRepository.Remove(item);
        }
    }
}