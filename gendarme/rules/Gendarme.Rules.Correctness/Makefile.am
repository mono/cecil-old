SUBDIRS=Test

correctness_rulesdir=$(pkglibdir)
correctness_rules_SCRIPTS = Gendarme.Rules.Correctness.dll
EXTRA_DIST = $(correctness_rules_sources) $(correctness_rules_sources_in) Gendarme.Rules.Correctness.xml
CLEANFILES = Gendarme.Rules.Correctness.dll Test.Rules.Correctness.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Correctness.xml TestResult.xml

correctness_rules_sources_in = ../../AssemblyInfo.cs.in
correctness_rules_generated_sources = $(correctness_rules_sources_in:.in=)
correctness_rules_sources = BadRecursiveInvocationRule.cs OpCodeConstants.cs \
			    NonNullAttribute.cs NonNullAttributeCollector.cs \
			    NullDerefAnalysis.cs NullDerefRule.cs \
			    NullDerefFrame.cs Nullity.cs \
			    graph/INode.cs graph/Node.cs \
			    graph/IEdge.cs graph/Edge.cs graph/IGraph.cs \
			    graph/Graph.cs dataflow/BasicBlock.cs \
			    dataflow/CFG.cs dataflow/CFGEdge.cs \
			    dataflow/Dataflow.cs graph/DFS.cs \
			    dataflow/IDataflowAnalysis.cs


correctness_rules_build_sources = $(addprefix $(srcdir)/, $(correctness_rules_sources))
correctness_rules_build_sources += $(correctness_rules_generated_sources)

Gendarme.Rules.Correctness.dll: $(correctness_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(correctness_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Correctness.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Correctness.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

correctness_test_sources = BadRecursiveInvocationTest.cs
correctness_test_build_sources = $(addprefix $(srcdir)/Test/, $(correctness_test_sources))

Test.Rules.Correctness.dll: $(correctness_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:nunit.framework.dll -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Correctness.dll -out:$@ $(correctness_test_build_sources)

test: Gendarme.Rules.Correctness.dll Test.Rules.Correctness.dll

run-test: Test.Rules.Correctness.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Correctness.dll
