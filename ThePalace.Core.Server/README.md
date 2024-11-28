# ThePalace.Core.Server
.Net Core 2.2 - A fully functional ThePalace server replacement, with an additional WebSocket listener.

Authentication\\* - Validates user input IP Address/Reg Code/PUID/Username/Password/etc. against the ThePalace.Admin.Auth table.
<br />
Business\\* - Business logic layer. When a packet is received, then what? What logic executes to handle it? These are those classes.
<br />
Commands\\* - Server commands, mostly useable from the commandline too.
<br />
Core\\* - The heart of the server, the main Thread Controller for internal self maintenance.
<br />
Factories\\* - Similar in concept to Angular JS Factories. These classes can be Serialized like classes in the Protocols section, but they are a little more dynamic in behavior.
<br />
Network\\* - This is the network layer. Where connections, sessions, socket types, and outbound packet driver classes await. So in my server design, when a packet is received or sent, it's taken out of it's native structure and retained in a more common structure in memory. Then when a packet needs to go back out, depending on the connection type of the end-user Palace/WebSocket, the driver will format the outbound packet accordingly.
<br />
Protocols\\* - This is where the bulk of the protocol support to Serialize/Deserialize packet data into a common structure in memory takes place. However no Business logic exists in this section, that's within the Business section.
