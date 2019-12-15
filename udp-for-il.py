import sys
import select
import time
import socket

ap = ("192.168.2.10", 15007)
sk = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)
def sendOverUdp(line):
    bytes = str.encode(line)
    sk.sendto(bytes, ap)
    print(len(bytes))

def no_input():
    print('no input')

while True:
    line = sys.stdin.readline()
    if line:
        sendOverUdp(line)
    else:		
        time.sleep(1)		
else:
    no_input()
