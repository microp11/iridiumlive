# IridiumLive

*Welcome to iridiumLive!*

IridiumLive is a [Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-3.1) server application which allows for the real-time vizualization of the iridium satellites as they pass above your location. You install the application in your local network, and use the browser to interact with it.

At this time the following ports have been hard coded:
```
 7777: IridiumLive server port,
15007: udp port for receiving data from gr-iridium and iridium-toolkit.
```
The data will be provided by your personal install of gr-iridium and iridium-toolkit (see [gr-iridium](https://github.com/muccc/gr-iridium) and [iridium-toolkit](https://github.com/muccc/iridium-toolkit)).
1. On the same machine as gr-iridium download and extract the python udp transmitter [udp-for-il.py](). Please install in the same folder as gr-iridium.
2. Depending of your intended OS target, download and extract the appropriate IridiumLive archive. Install in a folder of its own. You can install either on the same machine or to a different one.
3. In the udp-for-il.py script, the **server_address** **(192.168.2.10)** denotes the machine where IridiumLive is installed. Modify it accordingly. This address is also where you will open your browser to later on:
```python
...
import socket

ap = ("localhost", 15007)
sk = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)
def sendOverUdp(line):
...
```
4. Instantiate the iridium receiver and pipe through decoder, filter and udp transmitter using the following command line:
```
user@computer:~/gr-iridium$ iridium-extractor --offline --multi-frame examples/hackrf.conf | ~/iridium-toolkit/iridium-parser.py -p /dev/stdin /dev/stdout | grep "sat:" | python udp-for-il.py
```
5. Only continue to next step if you see udp lines being transmitted:
```

```
6. Run IridiumLive. 
7. Open any browser at http://**server_address**:7777 and enjoy!
