
MCS = mcs
KEY_FILE = ../../mcs/class/mono.snk
MONO.CECIL.DLL = ../lib/Mono.Cecil.dll
MCS_FLAGS = -keyfile:$(KEY_FILE) -r:$(MONO.CECIL.DLL)
LINKER = linker.exe

all:
	$(MCS) $(MCS_FLAGS) @linker.exe.sources /out:$(LINKER)

clean:
	rm -f $(LINKER)
