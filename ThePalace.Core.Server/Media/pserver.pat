;
; Room layout
;
ENTRANCE 86

ROOM
    ID 86
    DROPZONE
    NAME "Lobby"
    PICT "lobby.gif"
    DOOR
        ID 3
        DEST 91
        OUTLINE  13,120  91,125  102,208  27,230
        LOC 24,0
    ENDDOOR
    DOOR
        ID 4
        DEST 93
        OUTLINE  219,121  286,117  282,251  223,236
        LOC 84,108
    ENDDOOR
ENDROOM

ROOM
    ID 71
    DROPZONE
    NAME "The Loading Program..."
    PICT "clouds.gif"
    ARTIST "Sinistral Janus"
    PICTURE ID 1
        NAME "clouds.gif"
    ENDPICTURE
    DOOR
        LOCKABLE
        ID 1
        NAME "New Spot 1"
        DRAGGABLE
        FORBIDDEN
        MANDATORY
        LANDINGPAD
        DONTMOVEHERE
        INVISIBLE
        SHOWNAME
        SHOWFRAME
        SHADOW
        FILL
        DEST 91
        OUTLINE  135,116  391,116  391,308  135,308
        LOC 263,212
        PICTS
            1,0,0
        ENDPICTS
    ENDDOOR
ENDROOM

ROOM
    ID 91
    NAME "Breakout Room"
    PICT "breakout.gif"
    DOOR
        ID 1
        DEST 86
        OUTLINE  383,366  492,365  499,385  381,384
        LOC 235,254
    ENDDOOR
    DOOR
        ID 2
        DEST 86
        OUTLINE  225,105  291,105  289,163  226,163
        LOC 226,107
    ENDDOOR
ENDROOM

ROOM
    ID 87
    NAME "Event Room"
    PICT "patient.gif"
    PROP
        PROPID 0x31C9E002
        CRC 0xE2D0639A
        LOC 430,10
    ENDPROP
    PROP
        PROPID 0x3495AF9A
        CRC 0x208085BC
        LOC 435,64
    ENDPROP
    SPOT
        ID 1
        OUTLINE  243,2  253,3  253,12  244,11
        LOC 256,192
        SCRIPT
ON ENTER {
"Please wait while we help you join the Event now in progress" LOCALMSG
"Training Event" LAUNCHEVENT
}
        ENDSCRIPT
    ENDSPOT
ENDROOM

ROOM
    ID 88
    NAME "Small Conference"
    PICT "sm_conference.gif"
    PROP
        PROPID 0x34D901C3
        CRC 0xCC641709
        LOC 72,210
    ENDPROP
    PROP
        PROPID 0x34D90199
        CRC 0xE6EF03B6
        LOC 294,256
    ENDPROP
    PROP
        PROPID 0x34D901AD
        CRC 0xE7146696
        LOC 296,104
    ENDPROP
ENDROOM

ROOM
    ID 93
    NAME "Conference Room"
    PICT "conference_room.gif"
    DOOR
        ID 1
        DEST 86
        OUTLINE  384,365  496,365  503,382  377,383
        LOC 373,285
    ENDDOOR
ENDROOM

END
