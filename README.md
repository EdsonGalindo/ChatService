# ChatService
A Chat Service Solution that includes a Chat Server and Chat Client application

Executing Chat Service instructions:

1 - Build the entire solution

2 - Open the Chat Server application executable, that can be found on path ChatServer\bin\Debug\ChatServer.exe

3 - Open the Chat Client application executable, that can be found on path ChatService.Client\bin\Debug\ChatService.Client.exe

4 - On Chat Client window write a nickname to be used on the chat session and press "Enter" to connect to the server

5 - After connected a message advising you entry wil be showed, so you will be able to send messages to anothers chat users to read


Aditional informations:

1 - The Chat Server can be finalized easily pressing "Enter" key

2 - The Chat client allows Exit from the session writing the "/exit" command

3 - To send a public message to some other user write "/p + Space + Nickname" followed by the message you wants to send

4 - The Chat Server is by default running on the host "127.0.0.1" and port "8182", but both can be changed editing the "Program.cs" files from
"ChatService.Server" and "ChatService.Client" projects of "ChatService" solution and then building again
