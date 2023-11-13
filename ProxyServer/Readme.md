# Useful links
https://www.codeproject.com/Articles/13944/ASProxy-Surf-in-the-web-invisibly-using-ASP-NET-po
Look up how to make a transparent proxy in c#, and add an SSL certificate 
- dont forget to route browser through the proxy to test it. 

## Potential solutions
- https://stackoverflow.com/questions/593454/easiest-language-for-creating-a-windows-service/593803#593803
	- Create a service that listens on port 443 and port 80. 443 is Https, 80 is HTTP.
- 
- Proxy server Man in the Middle with Root-certificate.
	- 1. Establish a TCP connection with the client and the target server.
	1. Implement SSL/TLS handshake with the client and the target server to establish an encrypted connection.
	2. Terminate the SSL/TLS connection with the client and the target server on the proxy server.
	3. Inspect the decrypted data from the client and target server.
	4. Re-encrypt the data using the proxy server's own certificate and forward it to the respective party.
	5. Handle any necessary modifications or manipulations to the data as required.
	6. Close the connections when the communication is complete.
- FiddlerCore - Kan købes og bruges til at kigge på Http/Https connection

Portion of the solution
- 1: A proxy server where internet traffic is funneled through.
- 2. A Certificate portion for en/decrypting
- 3. Setting windows to use the proxy


## Currently testing:

1. Set up a proxy in .net that can listen to traffic using sockets. 
2. Set the proxy as a Man in the Middle and route traffic through. Do nothing with the traffic, just receive requests and send them through.
3. Make a Root Certificate and use it on the server
4. See if its possible to look into data sent, either output to console or on a file. The main thing will be being able to look at the data afterwards.


package sniffing with dotpcap
- windows: WinPcap must be installed
- Linux / Mac / Unix: libpcap must be installed.


## Main issues, and going further:
1. There are double-signed programs / encryptions, where its not just enough to have our own
	1. The main problem here can arrive from "the issuing certificate authority's digital signature". Even if we use Root certificate, we can run into situations where root certificate isn't enough.
2. Blocking is not effective. Throttling / monitoring likely is.
	1. Blocking will make the people being blocked look for a way to circumvent it, which is doable with a VPN. If they bypass the funnel, the packet sniffing isn't useful anymore, likewise any blocklist will not be useful as the request will be encrypted by the VPN.
3. Normally there are some things that are double-signed, specifically drivers. If we build upon NPCap, we can leverage them being double-signed, as the driver is tested and co-signed by Microsoft. 