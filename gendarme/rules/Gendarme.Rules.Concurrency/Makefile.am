SUBDIRS=Test

concurrency_rulesdir=$(pkglibdir)
concurrency_rules_SCRIPTS = Gendarme.Rules.Concurrency.dll
EXTRA_DIST = $(concurrency_rules_sources) $(concurrency_rules_sources_in) Gendarme.Rules.Concurrency.xml \
	Gendarme.Rules.Concurrency.mdp Gendarme.Rules.Concurrency.csproj
CLEANFILES = Gendarme.Rules.Concurrency.dll Test.Rules.Concurrency.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Concurrency.xml TestResult.xml

concurrency_rules_sources_in = ../../AssemblyInfo.cs.in
concurrency_rules_generated_sources = $(concurrency_rules_sources_in:.in=)
concurrency_rules_sources = DoubleCheckLockingRule.cs

concurrency_rules_build_sources = $(addprefix $(srcdir)/, $(concurrency_rules_sources))
concurrency_rules_build_sources += $(concurrency_rules_generated_sources)

Gendarme.Rules.Concurrency.dll: $(concurrency_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(concurrency_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Concurrency.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Concurrency.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

concurrency_test_sources = DoubleCheckLockingTest.cs
concurrency_test_build_sources = $(addprefix $(srcdir)/Test/, $(concurrency_test_sources))

Test.Rules.Concurrency.dll: $(concurrency_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Concurrency.dll -out:$@ $(concurrency_test_build_sources)

test: Gendarme.Rules.Concurrency.dll Test.Rules.Concurrency.dll

run-test: Test.Rules.Concurrency.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Concurrency.dll

self-test: test
	gendarme Gendarme.Rules.Concurrency.dll
