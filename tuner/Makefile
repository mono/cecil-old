MONO = mono
OLIVE = ../../olive
MCS = ../../mcs
OUTPUT = output
LINKER = monolinker.exe
LINKER_DIRS = -d $(MCS)/class/lib/net_2_1 -d $(OLIVE)/class/lib/net_3_0
LINKER_OPTS = -o $(OUTPUT) -l none -c link
MASTER_INFOS_WEB = http://mono.ximian.com/masterinfos/masterinfo-sl11a.zip
MASTER_INFOS_ZIP = masterinfos.zip
MASTER_INFOS_SL = @masterinfos/silverlight.infos

all: masterinfos
	$(MONO) $(LINKER) $(LINKER_DIRS) $(LINKER_OPTS) -i $(MASTER_INFOS_SL)
	
masterinfos:
	wget -O $(MASTER_INFOS_ZIP) $(MASTER_INFOS_WEB)
	unzip $(MASTER_INFOS_ZIP)

clean:
	rm -rf masterinfos*
	rm -rf $(OUTPUT)
