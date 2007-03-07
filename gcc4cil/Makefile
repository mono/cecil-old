
MCS = gmcs
KEY_FILE = ../../mcs/class/mono.snk
MONO.CECIL.DLL = Mono.Cecil.dll
MCS_FLAGS = -keyfile:$(KEY_FILE) -r:$(MONO.CECIL.DLL) -debug
GCC4CIL = gcc4cil.exe

all:
	cp ../lib/$(MONO.CECIL.DLL) .
	$(MCS) $(MCS_FLAGS) @gcc4cil.exe.sources /out:$(GCC4CIL)

clean:
	rm -f $(MERGE)
	rm -f $(MONO.CECIL.DLL)
