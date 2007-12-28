SUBDIRS=Test

design_rulesdir=$(pkglibdir)
design_rules_SCRIPTS = ../../bin/Gendarme.Rules.Design.dll
EXTRA_DIST = $(design_rules_sources) $(design_rules_sources_in) Gendarme.Rules.Design.xml \
	Gendarme.Rules.Design.mdp 
CLEANFILES = $(design_rules_SCRIPTS) $(design_rules_SCRIPTS).mdb Test.Rules.Design.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Design.xml TestResult.xml

design_rules_sources_in = ../../AssemblyInfo.cs.in
design_rules_generated_sources = $(design_rules_sources_in:.in=)
design_rules_sources = DontDeclareProtectedFieldsInSealedClassRule.cs UsingCloneWithoutImplementingICloneableRule.cs \
	DefineAZeroValueRule.cs EnumsShouldDefineAZeroValueRule.cs FlagsShouldNotDefineAZeroValueRule.cs \
	AbstractTypesShouldNotHavePublicConstructorsRule.cs AvoidEmptyInterfaceRule.cs TypesShouldBeInsideNamespacesRule.cs \
	AvoidPropertiesWithoutGetAccessorRule.cs MainShouldNotBePublicRule.cs

design_rules_build_sources = $(addprefix $(srcdir)/, $(design_rules_sources))
design_rules_build_sources += $(design_rules_generated_sources)

../../bin/Gendarme.Rules.Design.dll: $(design_rules_build_sources)
	$(MCS) -debug -target:library -langversion:linq -pkg:mono-cecil -r:../../bin/Gendarme.Framework.dll -out:$@ $(design_rules_build_sources)
	cp Gendarme.Rules.*.xml ../../bin/

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

design_test_sources = DontDeclareProtectedFieldsInSealedClassTest.cs UsingCloneWithoutImplementingICloneableTest.cs \
	EnumsShouldDefineAZeroValueTest.cs FlagsShouldNotDefineAZeroValueTest.cs \
	AbstractTypesShouldNotHavePublicConstructorsTest.cs AvoidEmptyInterfaceTest.cs TypesShouldBeInsideNamespacesTest.cs \
	AvoidPropertiesWithoutGetAccessorTest.cs MainShouldNotBePublicTest.cs

design_test_build_sources = $(addprefix $(srcdir)/Test/, $(design_test_sources))

Test.Rules.Design.dll: $(design_rules_build_sources) $(design_rules_SCRIPTS)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../bin/Gendarme.Framework.dll \
		-r:$(design_rules_SCRIPTS) -out:$@ $(design_test_build_sources)

test: Test.Rules.Design.dll

run-test: test
	MONO_PATH=../../bin/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Design.dll

self-test: $(design_rules_SCRIPTS)
	mono --debug ../../bin/gendarme.exe $(design_rules_SCRIPTS)

