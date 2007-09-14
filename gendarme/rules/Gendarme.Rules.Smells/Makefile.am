SUBDIRS=Test

smells_rulesdir=$(pkglibdir)
smells_rules_SCRIPTS = Gendarme.Rules.Smells.dll
EXTRA_DIST = $(smells_rules_sources) $(smells_rules_sources_in) Gendarme.Rules.Smells.xml \
	Gendarme.Rules.Smells.mdp Gendarme.Rules.Smells.csproj
CLEANFILES = Gendarme.Rules.Smells.dll Test.Rules.Smells.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Smells.xml TestResult.xml

smells_rules_sources_in = ../../AssemblyInfo.cs.in
smells_rules_generated_sources = $(smells_rules_sources_in:.in=)
smells_rules_sources = AvoidCodeDuplicatedInSameClassRule.cs AvoidCodeDuplicatedInSiblingClassesRule.cs \
AvoidLargeClassesRule.cs AvoidLongMethodsRule.cs AvoidLongParameterListsRule.cs \
AvoidSpeculativeGeneralityRule.cs CodeDuplicatedLocator.cs Expression.cs \
ExpressionFillerVisitor.cs Utilities.cs

smells_rules_build_sources = $(addprefix $(srcdir)/, $(smells_rules_sources))
smells_rules_build_sources += $(smells_rules_generated_sources)

Gendarme.Rules.Smells.dll: $(smells_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -r:../Gendarme.Rules.Performance/Gendarme.Rules.Performance.dll -out:$@ $(smells_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Smells.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.Smells.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

smells_test_sources =  AvoidCodeDuplicatedInSameClassTest.cs AvoidCodeDuplicatedInSiblingClassesTest.cs \
AvoidLargeClassesTest.cs AvoidLongMethodsTest.cs AvoidLongParameterListsTest.cs \
AvoidSpeculativeGeneralityTest.cs

smells_test_build_sources = $(addprefix $(srcdir)/Test/, $(smells_test_sources))

Test.Rules.Smells.dll: $(smells_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Smells.dll -r:System.Windows.Forms.dll -out:$@ $(smells_test_build_sources)

test: Gendarme.Rules.Smells.dll Test.Rules.Smells.dll

run-test: Gendarme.Rules.Smells.dll Test.Rules.Smells.dll
	MONO_PATH=../../framework/:../Gendarme.Rules.Performance/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Smells.dll

self-test: test
	gendarme Gendarme.Rules.Smells.dll
