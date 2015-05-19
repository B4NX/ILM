// Data.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	HANDLE hSerial;

	hSerial = CreateFile(L"COM1", GENERIC_READ, 0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);

	if (hSerial == INVALID_HANDLE_VALUE){
		if (GetLastError() == ERROR_FILE_NOT_FOUND){
			CloseHandle(hSerial);
			return 1;//Serial port not found
		}
		CloseHandle(hSerial);
		return 666;//Something else is dead
	}

	DCB dcbSerialParams = { 0 };
	dcbSerialParams.DCBlength = sizeof(dcbSerialParams);
	if (!GetCommState(hSerial, &dcbSerialParams)){
		CloseHandle(hSerial);
		return 2;//Error getting state
	}

	dcbSerialParams.BaudRate = CBR_9600;
	dcbSerialParams.ByteSize = 8;
	dcbSerialParams.StopBits = ONE5STOPBITS;
	dcbSerialParams.Parity = NOPARITY;

	if (!SetCommState(hSerial, &dcbSerialParams)){
		CloseHandle(hSerial);
		return 3;//Error setting state
	}

	COMMTIMEOUTS timeouts = { 0 };
	timeouts.ReadIntervalTimeout = 50;
	timeouts.ReadTotalTimeoutConstant = 50;
	timeouts.ReadTotalTimeoutMultiplier = 10;
	timeouts.WriteTotalTimeoutConstant = 50;
	timeouts.WriteTotalTimeoutMultiplier = 10;
	if (!SetCommTimeouts(hSerial, &timeouts)){
		CloseHandle(hSerial);
		return 4; //Error setting timeout
	}

	ofstream file;
	file.open("DataCollection");
	while (true){
		char szbuffer[1] = { 0 };
		DWORD dwBytesRead = 0;
		if (!ReadFile(hSerial, szbuffer, 1, &dwBytesRead, NULL)){
			CloseHandle(hSerial);
			return 5;
		}
		file << szbuffer;
		file << ",";
	}
	CloseHandle(hSerial);
	return 0;
}

