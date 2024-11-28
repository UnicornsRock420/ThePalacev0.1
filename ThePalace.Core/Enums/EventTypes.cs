using System.ComponentModel;

namespace ThePalace.Core.Enums
{
    public enum EventTypes : uint
    {
        #region Assets
        [Description("aAst")]
        MSG_ASSETNEW = 0x61417374,
        [Description("qAst")]
        MSG_ASSETQUERY = 0x71417374,
        [Description("rAst")]
        MSG_ASSETREGI = 0x72417374,
        [Description("sAst")]
        MSG_ASSETSEND = 0x73417374,
        [Description("dPrp")]
        MSG_PROPDEL = 0x64507270,
        [Description("mPrp")]
        MSG_PROPMOVE = 0x6D507270,
        [Description("nPrp")]
        MSG_PROPNEW = 0x6E507270,
        [Description("sPrp")]
        MSG_PROPSETDESC = 0x73507270,
        #endregion
        #region Auth
        [Description("susr")]
        MSG_SUPERUSER = 0x73757372,
        [Description("auth")]
        MSG_AUTHENTICATE = 0x61757468,
        [Description("autr")]
        MSG_AUTHRESPONSE = 0x61757472,
        #endregion
        #region Communications
        [Description("gmsg")]
        MSG_GMSG = 0x676D7367,
        [Description("rmsg")]
        MSG_RMSG = 0x726D7367,
        [Description("smsg")]
        MSG_SMSG = 0x736D7367,
        [Description("talk")]
        MSG_TALK = 0x74616C6B,
        [Description("whis")]
        MSG_WHISPER = 0x77686973,
        [Description("wmsg")]
        MSG_WMSG = 0x776D7367,
        [Description("xwis")]
        MSG_XWHISPER = 0x78776973,
        [Description("xtlk")]
        MSG_XTALK = 0x78746C6B,
        #endregion
        #region Media
        [Description("fnfe")]
        MSG_FILENOTFND = 0x666e6665,
        [Description("qFil")]
        MSG_FILEQUERY = 0x7146696C,
        [Description("sFil")]
        MSG_FILESEND = 0x7346696C,
        #endregion
        #region Network
        [Description("rep2")]
        MSG_ALTLOGONREPLY = 0x72657032,
        [Description("blow")]
        MSG_BLOWTHRU = 0x626C6F77,
        [Description("durl")]
        MSG_DISPLAYURL = 0x6475726C,
        [Description("ryit")]
        MSG_DIYIT = 0x72796974,
        [Description("HTTP")]
        MSG_HTTPSERVER = 0x48545450,
        [Description("cLog")]
        MSG_INITCONNECTION = 0x634C6F67,
        [Description("regi")]
        MSG_LOGON = 0x72656769,
        [Description("bye ")]
        MSG_LOGOFF = 0x62796520,
        [Description("ping")]
        MSG_PING = 0x70696E67,
        [Description("pong")]
        MSG_PONG = 0x706F6E67,
        [Description("resp")]
        MSG_RESPORT = 0x72657370,
        [Description("navR")]
        MSG_ROOMGOTO = 0x6E617652,
        [Description("tiyr")]
        MSG_TIYID = 0x74697972,
        #endregion
        #region Room
        [Description("lock")]
        MSG_DOORLOCK = 0x6C6F636B,
        [Description("unlo")]
        MSG_DOORUNLOCK = 0x756E6C6F,
        [Description("draw")]
        MSG_DRAW = 0x64726177,
        [Description("dPct")]
        MSG_PICTDEL = 0x46505371,
        [Description("pLoc")]
        MSG_PICTMOVE = 0x704C6F63,
        [Description("nPct")]
        MSG_PICTNEW = 0x6E506374,
        [Description("sPct")]
        MSG_PICTSETDESC = 0x73506374,
        [Description("nRom")]
        MSG_ROOMNEW = 0x6E526F6D,
        [Description("ofNr")]
        MSG_ROOMINFO = 0x6F664E72,
        [Description("sRom")]
        MSG_ROOMSETDESC = 0x73526F6D,
        [Description("room")]
        MSG_ROOMDESC = 0x726F6F6D,
        [Description("endr")]
        MSG_ROOMDESCEND = 0x656E6472,
        [Description("dSpo")]
        MSG_SPOTDEL = 0x6F705364,
        [Description("ofNs")]
        MSG_SPOTINFO = 0x6F664E73,
        [Description("sLoc")]
        MSG_SPOTMOVE = 0x636F4C73,
        [Description("nSpo")]
        MSG_SPOTNEW = 0x6F70536E,
        [Description("sSpo")]
        MSG_SPOTSETDESC = 0x6F705373,
        [Description("sSta")]
        MSG_SPOTSTATE = 0x73537461,
        #endregion
        #region Server
        [Description("sInf")]
        MSG_EXTENDEDINFO = 0x73496e66,
        [Description("kill")]
        MSG_KILLUSER = 0x6B696C6C,
        [Description("rLst")]
        MSG_LISTOFALLROOMS = 0x724C7374,
        [Description("uLst")]
        MSG_LISTOFALLUSERS = 0x754C7374,
        [Description("sErr")]
        MSG_NAVERROR = 0x73457272,
        [Description("NOOP")]
        MSG_NOOP = 0x4E4F4F50,
        [Description("down")]
        MSG_SERVERDOWN = 0x646F776E,
        [Description("sinf")]
        MSG_SERVERINFO = 0x73696E66,
        [Description("init")]
        MSG_SERVERUP = 0x696E6974,
        [Description("vers")]
        MSG_VERSION = 0x76657273,
        #endregion
        #region Users
        [Description("usrC")]
        MSG_USERCOLOR = 0x75737243,
        [Description("usrD")]
        MSG_USERDESC = 0x75737244,
        [Description("wprs")]
        MSG_USERENTER = 0x77707273,
        [Description("eprs")]
        MSG_USEREXIT = 0x65707273,
        [Description("usrF")]
        MSG_USERFACE = 0x75737246,
        [Description("rprs")]
        MSG_USERLIST = 0x72707273,
        [Description("log ")]
        MSG_USERLOG = 0x6C6F6720,
        [Description("uLoc")]
        MSG_USERMOVE = 0x754C6F63,
        [Description("nprs")]
        MSG_USERNEW = 0x6E707273,
        [Description("usrN")]
        MSG_USERNAME = 0x7573724E,
        [Description("usrP")]
        MSG_USERPROP = 0x75737250,
        [Description("uSta")]
        MSG_USERSTATUS = 0x75537461,
        #endregion
    };
}
