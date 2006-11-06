
MCS = gmcs
KEY_FILE = ../../mcs/class/mono.snk
MONO.CECIL.DLL = Mono.Cecil.dll
MCS_FLAGS = -keyfile:$(KEY_FILE) -r:$(MONO.CECIL.DLL)
MERGE = monomerge.exe

all:
	cp ../lib/$(MONO.CECIL.DLL) .
	$(MCS) $(MCS_FLAGS) @monomerge.exe.sources /out:$(MERGE)

clean:
	rm -f $(MERGE)
	rm -f $(MONO.CECIL.DLL)
