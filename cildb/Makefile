include config.make

MCS = mcs
KEY_FILE = ../../mcs/class/mono.snk
MCS_FLAGS = -keyfile:$(KEY_FILE)

all:
	$(MCS) $(MCS_FLAGS) @Mono.CompilerServices.SymbolStore.dll.sources /target:library /out:Mono.CompilerServices.SymbolStore.dll

clean:
	rm -f *.dll
	rm -f symbolstore.pc

symbolstore.pc: symbolstore.pc.in
	sed -e 's,@prefix@,$(prefix),g' symbolstore.pc.in > $@.tmp
	mv $@.tmp $@

install: all symbolstore.pc
	mkdir -p $(prefix)/lib/Mono.CompilerServices.SymbolStore
	cp Mono.CompilerServices.SymbolStore.dll $(prefix)/lib/Mono.CompilerServices.SymbolStore
	cp symbolstore.pc $(prefix)/lib/pkgconfig/symbolstore.pc
