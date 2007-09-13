SUBDIRS=Test

naming_rulesdir=$(pkglibdir)
naming_rules_SCRIPTS = Gendarme.Rules.Naming.dll
EXTRA_DIST = $(naming_rules_sources) $(naming_rules_sources_in) Gendarme.Rules.Naming.xml \
	Gendarme.Rules.Naming.mdp Gendarme.Rules.Naming.csproj
CLEANFILES = Gendarme.Rules.Naming.dll Test.Rules.Naming.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Naming.xml TestResult.xml

naming_rules_sources_in = ../../AssemblyInfo.cs.in
naming_rules_generated_sources = $(naming_rules_sources_in:.in=)
naming_rules_sources = AttributeEndsWithAttributeSuffixRule.cs  EnumNotEndsWithEnumOrFlagsSuffixRule.cs \
UsePluralNameInFlagEnumsRule.cs UseSingularNameInEnumsUnlessAreFlagsRule.cs

naming_rules_build_sources = $(addprefix $(srcdir)/, $(naming_rules_sources))
naming_rules_build_sources += $(naming_rules_generated_sources)

Gendarme.Rules.Naming.dll: $(naming_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(naming_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Naming.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.Naming.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

naming_test_sources = AttributeEndsWithAttributeSuffixTest.cs EnumNotEndsWithEnumOrFlagsSuffixTest.cs \
UsePluralNameInFlagEnumsTest.cs UseSingularNameInEnumsUnlessAreFlagsTest.cs

naming_test_build_sources = $(addprefix $(srcdir)/Test/, $(naming_test_sources))

Test.Rules.Naming.dll: $(naming_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Naming.dll -out:$@ $(naming_test_build_sources)

test: Gendarme.Rules.Naming.dll Test.Rules.Naming.dll

run-test: Gendarme.Rules.Naming.dll Test.Rules.Naming.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Naming.dll

self-test: test
	gendarme Gendarme.Rules.Naming.dll
