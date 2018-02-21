using GenericAirways.Contracts;
using GenericAirways.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace GenericAirways.Business
{

    public class PNLFileProcessor : IPNLProcessor<PNLFile, PassengerRecord, string>
    {

        const string NameRegEx = @"1([A-Z])\w+\/([A-Z])\w+-([A-Z])\w+ .([L])/([A-Z])\w+";
        const string CodeRegEx = @".R\/(\w{4}) (\w{3}) (\w{13})\/1";

        const string NameSeparatorRegEx = @"(1)|(\/)|(-)|( .[L]\/)";

        public bool CheckCodeLine(string line)
        {
            return (new Regex(CodeRegEx, RegexOptions.IgnoreCase)).IsMatch(line);
        }

        public bool CheckNameLine(string line)
        {
            return (new Regex(NameRegEx, RegexOptions.IgnoreCase)).IsMatch(line);
        }

        public string ProcessCodeLine(string line)
        {
            return line;
        }

        public PassengerRecord ProcessNameLine(string line)
        {
            var processed = (new Regex(NameSeparatorRegEx, RegexOptions.IgnoreCase)).Split(line);
            return new PassengerRecord() { FirstName = processed[4], LastName = processed[2], RecordLocator = new RecordLocator() { Code = processed[8] } };
        }

        public PNLFile ProcessPNL(StringReader list)
        {
            List<PassengerRecord> PNList = new List<PassengerRecord>();
            PNLFile file = new PNLFile();
            int lineNumber = 1;
            string line = string.Empty;
            line = list.ReadLine();
            while (line != null)
            {
                if (!string.Empty.Equals(line))
                {
                    if (CheckNameLine(line))
                    {
                        PNList.Add( ProcessNameLine(line));
                    }else if (CheckCodeLine(line))
                    {
                        PNList.Last().LineData = ProcessCodeLine(line);
                    }else
                    {
                        throw new Exception(string.Format("The file has an error on line {0}:{1}", lineNumber, line));
                    }
                }
                line = list.ReadLine();
                lineNumber++;
            }

            file.RecordLocator = PNList.Select(pr => pr.RecordLocator)
                .GroupBy(rl=>rl.Code)
                .Select(g=>g.First()).ToList();

            file.RecordLocator.ToList().ForEach(rl => {
                rl.PassengerRecord = PNList.Where(pn => pn.RecordLocator.Code == rl.Code).ToList();
            });

            return file;
        }
        
    }
}
