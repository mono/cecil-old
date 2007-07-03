MONO = mono
OLIVE = ../../olive
MCS = ../../mcs
OUTPUT = output
LINKER = monolinker.exe
LINKER_DIRS = -d $(MCS)/class/lib/net_2_1 -d $(OLIVE)/class/lib/net_3_0
LINKER_OPTS = -o $(OUTPUT) -l none -c link
MASTER_INFOS = @masterinfos/silverlight.infos

all:
	$(MONO) $(LINKER) $(LINKER_DIRS) $(LINKER_OPTS) -i $(MASTER_INFOS)

clean:
	rm -rf $(OUTPUT)
