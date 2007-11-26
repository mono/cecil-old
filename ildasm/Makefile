MCS = gmcs

ASSEMBLIES = -r:Mono.Cecil

all: 
	$(MCS) -debug /target:exe /out:ildasm.exe $(ASSEMBLIES) @ildasm.exe.sources
