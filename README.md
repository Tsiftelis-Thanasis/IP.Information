The solution contains the Api, the project IP.Information.Api, which contains the main controller.
The IP.Information.Application which contains a background service, the EF DB Context, Interfaces with the implementation
and the IP.Information.Contract which contains Dtos and Enums, even if I didn't create any.

The Api contains the main Controller, IPAddressesController and 2 endpoints, GetIPAddress and GetCountriesReport.
The GetIPAddress returns an IpAddressDto object. It will search in the cache, ICachingIPAddresses, if it's not found then in the DB store, IIPAddressStore, 
and then if it's not found then will search in the service https://ip2c.org/.
If it's found in the service then it's added in the Store and in the Cache.
If it's found in the store then it's added in the Cache.
The ICachingIPAddresses is responsible for the caching of the addresses. I am not forcing and update in the cache. It's updated when it's found in the Store or from the 
service call.
The IIPAddressStore is mostly responsible to get the addresses from Database.
The above implementation exists in IP.Information.Application. 
A BackgroundService is also running in this application, DBInfoUpdater. Which updates the table IPAdresses and the Countries and then it's also updating the Store.

The endpoint GetCountriesReport is using a simple SqlConnection which opens the endpoint is called.
Then runs the sql query with a SqlCommand and returns an IQuerable list of ReportDto.

Added logging of exceptions in some try catch cases.


