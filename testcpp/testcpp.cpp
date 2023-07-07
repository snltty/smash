#include <stdio.h>

#include <map>
#include <list>
#include <queue>
#include <regex>
#include <mutex>
#include <chrono>
#include <string>
#include <vector>
#include <thread>
#include <iostream>

#include <WinSock2.h>
#include <ws2ipdef.h>
#include <WS2tcpip.h>
#include <tlhelp32.h>
#include <mstcpip.h>
#include <Windows.h>

using namespace std;

int main() {

	unsigned char ips[16];
	unsigned char mask[16];

	inet_pton(AF_INET, "127.0.0.1", ips);
	inet_pton(AF_INET, "255.0.0.0", mask);

	for (int i = 0; i < 16; i++)
	{
		cout << (int)ips[i] << ",";
	}
	cout << endl;
	for (int i = 0; i < 16; i++)
	{
		cout << (int)mask[i] << ",";
	}
	return 0;
}