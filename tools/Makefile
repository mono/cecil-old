MCS = mcs
MONO.CECIL.DLL = ../lib/Mono.Cecil.dll

all: cecil-echo cecil-roundtrip

cecil-echo:
	$(MCS) cecil-echo.cs /r:$(MONO.CECIL.DLL)

cecil-roundtrip:
	$(MCS) cecil-roundtrip.cs /r:$(MONO.CECIL.DLL)

clean:
	rm cecil-echo.exe
	rm cecil-roundtrip.exe
	
rebuild: clean all
