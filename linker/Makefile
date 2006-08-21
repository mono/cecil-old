
MCS = mcs
KEY_FILE = ../../mcs/class/mono.snk
MONO.CECIL.DLL = Mono.Cecil.dll
MCS_FLAGS = -keyfile:$(KEY_FILE) -r:$(MONO.CECIL.DLL) -resource:Descriptors/corlib.xml,Mono.Linker.Descriptors.corlib.xml
LINKER = linker.exe

all:
	cp ../lib/$(MONO.CECIL.DLL) .
	$(MCS) $(MCS_FLAGS) @linker.exe.sources /out:$(LINKER)

clean:
	rm -f $(LINKER)
	rm -f $(MONO.CECIL.DLL)
