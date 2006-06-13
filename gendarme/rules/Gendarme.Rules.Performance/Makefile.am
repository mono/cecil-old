SUBDIRS=Test

performance_rulesdir=$(pkglibdir)
performance_rules_SCRIPTS = Gendarme.Rules.Performance.dll
EXTRA_DIST = $(performance_rules_sources) $(performance_rules_sources_in) Gendarme.Rules.Performance.xml
CLEANFILES = Gendarme.Rules.Performance.dll Test.Rules.Performance.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Performance.xml TestResult.xml

performance_rules_sources_in = ../../AssemblyInfo.cs.in
performance_rules_generated_sources = $(performance_rules_sources_in:.in=)
performance_rules_sources = EmptyDestructorRule.cs IDisposableWithDestructorWithoutSuppressFinalizeRule.cs

performance_rules_build_sources = $(addprefix $(srcdir)/, $(performance_rules_sources))
performance_rules_build_sources += $(performance_rules_generated_sources)

Gendarme.Rules.Performance.dll: $(performance_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(performance_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Performance.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Performance.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

performance_test_sources = EmptyDestructorTest.cs IDisposableWithDestructorWithoutSuppressFinalizeTest.cs
performance_test_build_sources = $(addprefix $(srcdir)/Test/, $(performance_test_sources))

Test.Rules.Performance.dll: $(performance_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Performance.dll -out:$@ $(performance_test_build_sources)

test: Gendarme.Rules.Performance.dll Test.Rules.Performance.dll

run-test: Gendarme.Rules.Performance.dll Test.Rules.Performance.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Performance.dll

self-test: test
	gendarme Gendarme.Rules.Performance.dll
