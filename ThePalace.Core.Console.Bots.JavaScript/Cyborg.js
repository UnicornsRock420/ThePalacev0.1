function MAIN() {
    Console.WriteLine("Hello World!!!");
    proxy.SetName("Derp123");
    proxy.Connect("alluringhistoria.epalaces.com:9998");
}

function SIGNON() {
    //proxy.GotoRoom(1);
}

function ENTER() {
    //Console.WriteLine("ENTER!");

    Console.WriteLine("ENTERED: " + ROOMID);
}

function CHAT() {
    //Console.WriteLine("CHAT!");
}

function INCHAT() {
    //Console.WriteLine("INCHAT!");

    //Console.WriteLine(WHOCHAT);
    //Console.WriteLine(CHATSTR);
}

function CONNECTIONESTABLISHED() {
    Console.WriteLine("Connection Established!!!");
}

function CONNECTIONERROR() {
    Console.WriteLine("Connection Error...");
}

function DISCONNECT() {
    Console.WriteLine("Disconnected!");
}

function USERENTER() {
    Console.WriteLine("USERENTER!");
}

function USERMOVE() {
    Console.WriteLine("USERMOVE!");
}

function USERLEAVE() {
    Console.WriteLine("USERLEAVE!");
}

function NAMECHANGE() {
    Console.WriteLine("NAMECHANGE!");
}

function NAVERROR() {
    Console.WriteLine("Sorry, that Room ID doesn't exist!");
}

function ROOMLOAD() {
    //Console.WriteLine("ROOMLOAD!");
	
    Console.WriteLine("ENTERED: " + ROOMID);
}

function ROOMREADY() {
    Console.WriteLine("ROOMREADY!");
	
    Console.WriteLine("ENTERED: " + ROOMID);
}

function SERVERINFO() {
    Console.WriteLine("SERVERINFO!");
}

function SERVERMSG() {
    Console.WriteLine("SERVERMSG!");
}

function LISTOFALLUSERS() {
    Console.WriteLine("LISTOFALLUSERS!");
}

function LISTOFALLROOMS() {
    Console.WriteLine("LISTOFALLROOMS!");
}
