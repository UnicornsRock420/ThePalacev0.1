using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("ofNs")]
    public sealed class MSG_SPOTINFO : HotspotRec, IProtocolReceive, IProtocolSend
    {
        public List<PictureRec> pictureList;
        public short roomID;

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomID = jsonResponse.roomID;

                id = (short)jsonResponse.spotID;
                type = (HotspotTypes)(short)jsonResponse.type;
                state = (short)jsonResponse.state;
                name = jsonResponse.name;
                script = jsonResponse.script;
                dest = (short)jsonResponse.dest;
                flags = (int)jsonResponse.flags;

                loc = new Point((short)jsonResponse.loc.h, (short)jsonResponse.loc.v);

                States = new List<HotspotStateRec>();

                if ((jsonResponse.states?.Count ?? 0) > 0)
                {
                    foreach (var state in jsonResponse.states)
                    {
                        States.Add(new HotspotStateRec
                        {
                            pictID = (short)state.pictID,
                            picLoc = new Point((short)state.picLoc.h, (short)state.picLoc.v),
                        });
                    }
                }

                Vortexes = new List<Point>();

                if ((jsonResponse.vortexes?.Count ?? 0) > 0)
                {
                    foreach (var vortex in jsonResponse.vortexes)
                    {
                        Vortexes.Add(new Point((short)vortex.h, (short)vortex.v));
                    }
                }

                if ((jsonResponse.pictureList?.Count ?? 0) > 0)
                {
                    pictureList = new List<PictureRec>();

                    foreach (var picture in jsonResponse.pictureList)
                    {
                        pictureList.Add(new PictureRec
                        {
                            picID = (short)picture.picID,
                            name = picture.name,
                            transColor = (short)picture.transColor,
                        });
                    }
                }
                else
                {
                    pictureList = null;
                }
            }
        }

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
