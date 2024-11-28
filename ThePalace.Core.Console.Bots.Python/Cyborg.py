def MAIN():
    proxy.ConsoleMsg("Hello World!!!")
    proxy.Connect("alluringhistoria.epalaces.com:9998")

def SIGNON():
    proxy.GotoRoom(1)

def ENTER():
    proxy.ConsoleMsg("ENTER!")

def CHAT():
    proxy.ConsoleMsg("CHAT!")

def INCHAT():
    proxy.ConsoleMsg("INCHAT!")
    proxy.ConsoleMsg(CHATSTR)

def CONNECTIONESTABLISHED():
    proxy.ConsoleMsg("Connection Established!!!")

def CONNECTIONERROR():
    proxy.ConsoleMsg("Connection Error...")

def DISCONNECT():
    proxy.ConsoleMsg("Disconnected!")

def NAVERROR():
    proxy.ConsoleMsg("Sorry, that Room ID doesn't exist!")

def ROOMLOAD():
    proxy.ConsoleMsg("ROOMLOAD!")

def ROOMREADY():
    proxy.ConsoleMsg("ROOMREADY!")

def SERVERINFO():
    proxy.ConsoleMsg("SERVERINFO!")

def SERVERMSG():
    proxy.ConsoleMsg("SERVERMSG!")

def LISTOFALLUSERS():
    proxy.ConsoleMsg("LISTOFALLUSERS!")

def LISTOFALLROOMS():
    proxy.ConsoleMsg("LISTOFALLROOMS!")
