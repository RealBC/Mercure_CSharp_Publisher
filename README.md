# Mercure_CSharp_Publisher
C# Publisher example for Mercure Server + JWT generator (without useless nugets :) )

First you need to configure your Mercure server locally
See: https://mercure.rocks/docs/hub/install

# Mercure Server

Personnally i use http://127.0.0.1:3000/.well-known/mercure/ui/

For "Topics to get updates for*" my topic is "http://172.16.0.124:3000/apply"
Have a look in my C# main, you're free to change it.

# Dotnet Subscriber

Get donet running on your OS
See: https://dotnet.microsoft.com/en-us/download

Clone the repository

dotnet build

dotnet run

Enjoy !
