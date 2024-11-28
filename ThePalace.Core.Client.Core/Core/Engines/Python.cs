using System;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Models;

namespace ThePalace.Core.Client.Core.Languages
{
    public sealed class Python : Disposable, IScriptEngine
    {
        public Python() { }
        ~Python() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }

        public void Initialize(IClientSessionState sessionState, ScriptEvent @event, params object[] args)
        {
        }

        public void PreInvoke(IClientSessionState sessionState, ScriptEvent @event, params object[] args)
        {
            //var t = @event.ScriptState as IptTracking;
            //var p = @event.Packet as Header;

            //var iptVar = null as IptMetaVariable;

            //if (t?.Variables?.ContainsKey("SESSIONSTATE") == false)
            //{
            //    iptVar = new IptMetaVariable
            //    {
            //        IsSpecial = true,
            //        IsGlobal = true,
            //    };
            //    t?.Variables?.TryAdd("SESSIONSTATE", iptVar);

            //    if (iptVar != null)
            //    {
            //        iptVar.IsReadOnly = false;
            //        iptVar.Value = new IptVariable
            //        {
            //            Type = IptVariableTypes.Shadow,
            //            Value = @event.SessionState,
            //        };
            //        iptVar.IsReadOnly = true;
            //    }
            //}

            //if (p != null)
            //{
            //    if (p.eventType == EventTypes.MSG_TALK ||
            //        p.eventType == EventTypes.MSG_XTALK ||
            //        p.eventType == EventTypes.MSG_WHISPER ||
            //        p.eventType == EventTypes.MSG_XWHISPER)
            //    {
            //        if (t?.Variables?.ContainsKey("WHOCHAT") == false)
            //        {
            //            iptVar = new IptMetaVariable
            //            {
            //                IsSpecial = true,
            //            };
            //            t?.Variables?.TryAdd("WHOCHAT", iptVar);
            //        }
            //        else iptVar = t?.Variables["WHOCHAT"];

            //        if (iptVar != null)
            //        {
            //            iptVar.IsReadOnly = false;
            //            iptVar.Value = new IptVariable
            //            {
            //                Type = IptVariableTypes.Integer,
            //                Value = p.refNum,
            //            };
            //            iptVar.IsReadOnly = true;
            //        }
            //    }

            //    if (p.eventType == EventTypes.MSG_GMSG ||
            //        p.eventType == EventTypes.MSG_RMSG ||
            //        p.eventType == EventTypes.MSG_SMSG ||
            //        p.eventType == EventTypes.MSG_WMSG ||
            //        p.eventType == EventTypes.MSG_TALK ||
            //        p.eventType == EventTypes.MSG_XTALK ||
            //        p.eventType == EventTypes.MSG_WHISPER ||
            //        p.eventType == EventTypes.MSG_XWHISPER)
            //    {
            //        if (t?.Variables?.ContainsKey("CHATSTR") == false)
            //        {
            //            iptVar = new IptMetaVariable
            //            {
            //                IsSpecial = true,
            //            };
            //            t?.Variables?.TryAdd("CHATSTR", iptVar);
            //        }
            //        else iptVar = t?.Variables["CHATSTR"];

            //        if (iptVar != null &&
            //            p.protocolReceive is IProtocolCommunications msgComms)
            //        {
            //            iptVar.Value = new IptVariable
            //            {
            //                Type = IptVariableTypes.String,
            //                Value = msgComms.text,
            //            };
            //        }
            //    }
            //}
        }

        public void Invoke(IClientSessionState sessionState, ScriptEvent @event, params object[] args)
        {
        }
    }
}
