SUBDIRS=Test

design_rulesdir=$(pkglibdir)
design_rules_SCRIPTS = Gendarme.Rules.Design.dll
EXTRA_DIST = $(design_rules_sources) $(design_rules_sources_in) Gendarme.Rules.Design.xml \
	Gendarme.Rules.Design.mdp Gendarme.Rules.Design.csproj
CLEANFILES = Gendarme.Rules.Design.dll Test.Rules.Design.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Design.xml TestResult.xml

design_rules_sources_in = ../../AssemblyInfo.cs.in
design_rules_generated_sources = $(design_rules_sources_in:.in=)
design_rules_sources = DontDeclareProtectedFieldsInSealedClassRule.cs UsingCloneWithoutImplementingICloneableRule.cs

design_rules_build_sources = $(addprefix $(srcdir)/, $(design_rules_sources))
design_rules_build_sources += $(design_rules_generated_sources)

Gendarme.Rules.Design.dll: $(design_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(design_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Design.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.Design.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

design_test_sources = DontDeclareProtectedFieldsInSealedClassTest.cs UsingCloneWithoutImplementingICloneableTest.cs
design_test_build_sources = $(addprefix $(srcdir)/Test/, $(design_test_sources))

Test.Rules.Design.dll: $(design_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Design.dll -out:$@ $(design_test_build_sources)

test: Gendarme.Rules.Design.dll Test.Rules.Design.dll

run-test: Gendarme.Rules.Design.dll Test.Rules.Design.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Design.dll

self-test: test
	gendarme Gendarme.Rules.Design.dll
