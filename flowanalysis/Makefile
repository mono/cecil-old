MCS = mcs
BUILD.DIR = bin
MONO.CECIL.DLL = ../lib/Mono.Cecil.dll

all: init
	$(MCS) @Cecil.FlowAnalysis.sources /target:library /out:$(BUILD.DIR)/Cecil.FlowAnalysis.dll /r:$(BUILD.DIR)/Mono.Cecil.dll
	
init:
	mkdir -p $(BUILD.DIR)
	cp $(MONO.CECIL.DLL) $(BUILD.DIR)/
	find Cecil.FlowAnalysis -name "*.cs" > Cecil.FlowAnalysis.sources
	
clean:
	rm -fr $(BUILD.DIR)
	
rebuild: clean all

