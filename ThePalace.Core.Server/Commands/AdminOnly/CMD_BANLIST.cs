using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_BANLIST : ICommand
    {
        public const string Help = @"-- Display a list of the currently banned users.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext == null) return false;

            var xtlk = new Protocols.MSG_XTALK();

            var banrecs = dbContext.Bans.AsNoTracking()
                .ToList();

            if (banrecs.Count > 0)
            {
                banrecs
                    .ForEach(banrec =>
                    {
                        var regRec = new RegistrationRec
                        {
                            counter = (UInt32)banrec.RegCtr,
                            crc = (UInt32)banrec.RegCrc,
                            puidCtr = (UInt32)banrec.Puidctr,
                            puidCRC = (UInt32)banrec.Puidcrc,
                        };

                        var regCode = Cipher.RegRectoSeed(regRec);
                        var puidCode = Cipher.RegRectoSeed(regRec, true);

                        xtlk.text = $"; {{{banrec.Ipaddress}}} {regCode} {puidCode}: {banrec.Note}";

                        if (_sessionState.UserID == 0)
                        {
                            Logger.ConsoleLog(xtlk.text);
                        }
                        else
                        {
                            _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
                        }
                    });
            }
            else
            {
                xtlk.text = "There are currently no bans to list...";

                if (_sessionState.UserID == 0)
                {
                    Logger.ConsoleLog(xtlk.text);
                }
                else
                {
                    _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
                }
            }

            return true;
        }
    }
}
