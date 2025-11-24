using FMELibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMEConsole
{
    public class SwitchNation
    {
        public async Task SwitchNationAsync()
        {
            // load nations.dat
            var file = "../../../db/db_archive_2603/nations.dat";
            var parser = await NationParser.Load(file);

            // switch nation id of 2 teams

            // load clubs.dat
            var clubFile = "../../../db/db_archive_2603/clubs.dat";
            var clubParser = await ClubParser.Load(clubFile);
        }
    }
}
