SHARED_SOURCES = \
	AssemblyResolver.cs	\
	CompletionInfo.cs	\
	MissingAttribute.cs	\
	MissingBase.cs		\
	MissingConstructor.cs	\
	MissingEvent.cs		\
	MissingField.cs		\
	MissingInterface.cs	\
	MissingMember.cs	\
	MissingMethod.cs	\
	MissingNameSpace.cs	\
	MissingNestedType.cs	\
	MissingProperty.cs	\
	MissingType.cs		\
	ToDoAssembly.cs		\
	Util.cs

REFERENCES=-r:Mono.Cecil.dll
CSC=gmcs -debug

all: CorCompare.exe mono-api-diff.exe mono-api-info.exe

CorCompare.exe: CorCompare.cs $(SHARED_SOURCES)
	$(CSC) -out:CorCompare.exe CorCompare.cs $(REFERENCES) $(SHARED_SOURCES)

mono-api-info.exe: mono-api-info.cs $(SHARED_SOURCES)
	$(CSC) -out:mono-api-info.exe mono-api-info.cs $(REFERENCES) $(SHARED_SOURCES)

mono-api-diff.exe: mono-api-diff.cs $(SHARED_SOURCES)
	$(CSC) -out:mono-api-diff.exe mono-api-diff.cs $(REFERENCES) $(SHARED_SOURCES)

clean: 
	rm *exe

