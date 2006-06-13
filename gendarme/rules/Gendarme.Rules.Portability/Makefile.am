SUBDIRS=Test

portability_rulesdir=$(pkglibdir)
portability_rules_SCRIPTS = Gendarme.Rules.Portability.dll
EXTRA_DIST = $(portability_rules_sources) $(portability_rules_sources_in) Gendarme.Rules.Portability.xml
CLEANFILES = Gendarme.Rules.Portability.dll Test.Rules.Portability.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Portability.xml TestResult.xml

portability_rules_sources_in = ../../AssemblyInfo.cs.in
portability_rules_generated_sources = $(portability_rules_sources_in:.in=)
portability_rules_sources = NewLineLiteralRule.cs

portability_rules_build_sources = $(addprefix $(srcdir)/, $(portability_rules_sources))
portability_rules_build_sources += $(portability_rules_generated_sources)

Gendarme.Rules.Portability.dll: $(portability_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(portability_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Portability.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Portability.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

portability_test_sources = NewLineLiteralTest.cs
portability_test_build_sources = $(addprefix $(srcdir)/Test/, $(portability_test_sources))

Test.Rules.Portability.dll: $(portability_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Portability.dll -out:$@ $(portability_test_build_sources)

test: Gendarme.Rules.Portability.dll Test.Rules.Portability.dll

run-test: Gendarme.Rules.Portability.dll Test.Rules.Portability.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Portability.dll

self-test: test
	gendarme Gendarme.Rules.Portability.dll
