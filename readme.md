Nexile is a .NET 7 library and desktop utility for working with Path of Exile and various related API's and services like PoE Ninja.

## Features

* Nexile.Client - A C# library for working with the Path of Exile API's and services.
* Nexile - A windows desktop utility for:
    * Tracking incoming and outgoing trade offers.
    * Managing live searches and notifications.
    * Price checking items

## Building

* Clone the repository
* Initialize the secrets manager by running `sh secrets.sh init`
* If you wish to run the integration tests, you'll need to add your Session ID to the secrets manager by running `secrets.sh add sessionid <your session id>`. This will add your Session ID to the test projects that require it. Your SessionId is not kept in the repository, and will instead be stored in your local secrets manager and pulled at run-time before running the integration tests.