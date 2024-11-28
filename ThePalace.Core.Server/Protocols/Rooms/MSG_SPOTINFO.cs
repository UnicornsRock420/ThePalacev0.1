using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Server.Protocols
{
    [Description("ofNs")]
    public class MSG_SPOTINFO : IProtocolReceive
    {
        public List<PictureRec> pictureList;
        public HotspotRec spot;
        public Int16 roomID;

        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                roomID = jsonResponse.roomID;

                spot = new HotspotRec();
                spot.id = (Int16)jsonResponse.spotID;
                spot.type = (HotspotTypes)(short)jsonResponse.type;
                spot.state = (Int16)jsonResponse.state;
                spot.name = jsonResponse.name;
                spot.script = jsonResponse.script;
                spot.dest = (Int16)jsonResponse.dest;
                spot.flags = (Int32)jsonResponse.flags;

                spot.loc = new Point((Int16)jsonResponse.loc.h, (Int16)jsonResponse.loc.v);

                spot.States = new List<HotspotStateRec>();

                if (jsonResponse.states != null && jsonResponse.states.Count > 0)
                {
                    foreach (var state in jsonResponse.states)
                    {
                        spot.States.Add(new HotspotStateRec
                        {
                            pictID = (Int16)state.pictID,
                            picLoc = new Point((Int16)state.picLoc.h, (Int16)state.picLoc.v),
                        });
                    }
                }

                spot.Vortexes = new List<Point>();

                if (jsonResponse.vortexes != null && jsonResponse.vortexes.Count > 0)
                {
                    foreach (var vortex in jsonResponse.vortexes)
                    {
                        spot.Vortexes.Add(new Point((Int16)vortex.h, (Int16)vortex.v));
                    }
                }

                if (jsonResponse.pictureList == null || jsonResponse.pictureList.Count < 1)
                {
                    pictureList = null;
                }
                else
                {
                    pictureList = new List<PictureRec>();

                    foreach (var picture in jsonResponse.pictureList)
                    {
                        pictureList.Add(new PictureRec
                        {
                            picID = (Int16)picture.picID,
                            name = picture.name,
                            transColor = (Int16)picture.transColor,
                        });
                    }
                }
            }
            catch
            {
            }
        }
    }
}
