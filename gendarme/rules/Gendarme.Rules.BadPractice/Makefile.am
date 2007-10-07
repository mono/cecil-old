SUBDIRS=Test

badpractice_rulesdir=$(pkglibdir)
badpractice_rules_SCRIPTS = Gendarme.Rules.BadPractice.dll
EXTRA_DIST = $(badpractice_rules_sources) $(badpractice_rules_sources_in) Gendarme.Rules.BadPractice.xml \
	Gendarme.Rules.BadPractice.mdp Gendarme.Rules.BadPractice.csproj
CLEANFILES = Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.BadPractice.xml TestResult.xml

badpractice_rules_sources_in = ../../AssemblyInfo.cs.in
badpractice_rules_generated_sources = $(badpractice_rules_sources_in:.in=)
badpractice_rules_sources = CloneMethodShouldNotReturnNullRule.cs EqualShouldHandleNullArgRule.cs \
	ImplementingEqualsButNotGetHashCodeAndViceVersaRule.cs ToStringReturnsNullRule.cs

badpractice_rules_build_sources = $(addprefix $(srcdir)/, $(badpractice_rules_sources))
badpractice_rules_build_sources += $(badpractice_rules_generated_sources)

Gendarme.Rules.BadPractice.dll: $(badpractice_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(badpractice_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.BadPractice.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.BadPractice.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

badpractice_test_sources = CloneMethodShouldNotReturnNullTest.cs EqualShouldHandleNullArgTest.cs \
	ImplementingEqualsButNotGetHashCodeAndViceVersaTest.cs ToStringReturnsNullTest.cs
badpractice_test_build_sources = $(addprefix $(srcdir)/Test/, $(badpractice_test_sources))

Test.Rules.BadPractice.dll: $(badpractice_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.BadPractice.dll -out:$@ $(badpractice_test_build_sources)

test: Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll

run-test: Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.BadPractice.dll

self-test: test
	gendarme Gendarme.Rules.BadPractice.dll
