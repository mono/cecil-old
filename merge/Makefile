
MCS = gmcs
KEY_FILE = ../../mcs/class/mono.snk
MONO.CECIL.DLL = `pkg-config cecil  --variable=Libraries`
MCS_FLAGS = -keyfile:$(KEY_FILE) -r:$(MONO.CECIL.DLL)
MERGE = monomerge.exe

all:
	$(MCS) $(MCS_FLAGS) @monomerge.exe.sources /out:$(MERGE)

clean:
	rm -f $(MERGE)
